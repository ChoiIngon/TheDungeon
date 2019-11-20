using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DungeonBattle : MonoBehaviour
{
	private TouchInput				touch_input;
	private Dungeon					dungeon;
	private Monster					monster;

	private UIGaugeBar				player_health;
	private Transform				player_damage_effect_spot;
	private Effect_PlayerDamage[]	player_damage_effects;

	public UISkillButton			skill_button_prefab;
	private Transform				skill_button_spot;
	private List<UISkillButton>		skill_buttons;
	private UISkillButton			runaway_button;

	public UIDamageText				damage_text_prefab;
	private List<UIDamageText>		damage_texts;

	private readonly float			second_per_turn = 0.5f;
	private int						turn_count = 0;
	private bool					battle_pause;

	private float player_attack_per_second = 0.0f;
	private float player_preemptive_score = 0.0f;
	private float enemy_attack_per_second = 0.0f;
	private float enemy_preemptive_score = 0.0f;

	private Coroutine show_damage_text_coroutine;

	public enum BattleResult
	{
		Invalid = 0,
		Win = 1,
		Lose = 2,
		Runaway = 3
	}
	public BattleResult				battle_result = BattleResult.Invalid; // 0 : lose, 1 : win, 2 : draw

	private void Awake()
	{
		dungeon = UIUtil.FindChild<Dungeon>(transform, "../Dungeon");
		monster = UIUtil.FindChild<Monster>(transform, "Monster");
		player_damage_effect_spot = UIUtil.FindChild<Transform>(transform, "../UI/BattleEffect");
		player_health = UIUtil.FindChild<UIGaugeBar>(transform, "../UI/Player/Health");
		skill_button_spot = UIUtil.FindChild<Transform>(transform, "../UI/Battle/SkillButtons");
		runaway_button = UIUtil.FindChild<UISkillButton>(transform, "../UI/SideButtons/RunawayButton");
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
		touch_input.block_count++;
	}

	void Start()
	{
		touch_input.on_touch_down += (Vector3 position) =>
		{
			if (null == monster.meta)
			{
				return;
			}
		};
		const int EFFECT_POOL_SIZE = 2;
		player_damage_effects = new Effect_PlayerDamage[EFFECT_POOL_SIZE];
		for (int i = 0; i < player_damage_effects.Length; i++)
		{
			player_damage_effects[i] = GameObject.Instantiate<Effect_PlayerDamage>(UIUtil.FindChild<Effect_PlayerDamage>(player_damage_effect_spot, "Effect_PlayerDamage"));
			player_damage_effects[i].transform.SetParent(player_damage_effect_spot, false);
		}
		skill_button_spot.gameObject.SetActive(false);
		runaway_button.gameObject.SetActive(false);
		damage_texts = new List<UIDamageText>();
		touch_input.block_count++;
		Util.EventSystem.Subscribe<Buff>(EventID.Buff_Effect, OnBuffEffect);
		Util.EventSystem.Subscribe(EventID.Inventory_Open, OnInventoryOpen);
		Util.EventSystem.Subscribe(EventID.Inventory_Close, OnInventoryClose);
	}

	private void OnEnable()
	{
		battle_pause = true;
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open, OnInventoryOpen);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close, OnInventoryClose);
		Util.EventSystem.Unsubscribe<Buff>(EventID.Buff_Effect, OnBuffEffect);
	}

	public IEnumerator BattleStart(Monster.Meta monsterMeta)
	{
		Util.EventSystem.Publish(EventID.Dungeon_Battle_Start);
		Util.EventSystem.Publish<float>(EventID.MiniMap_Hide, 0.0f);

		turn_count = 0;
		battle_pause = true;
		
		player_attack_per_second = Mathf.Max(2.1f, GameManager.Instance.player.speed / monsterMeta.speed);
		enemy_attack_per_second = 1.0f;
		player_preemptive_score = player_attack_per_second;
		enemy_preemptive_score = enemy_attack_per_second;
		battle_result = BattleResult.Invalid;

		monster.gameObject.SetActive(true);
		monster.Init(monsterMeta, dungeon.level / (dungeon.max_level + 1) + 1);

		InitButtons();
		GameManager.Instance.player.on_attack = null;
		GameManager.Instance.player.on_attack += (Unit.AttackResult result) =>
		{
			AudioManager.Instance.Play(AudioManager.BATTLE_ATTACK, false);
			SceneDungeon.log.Write(GameText.GetText("DUNGEON/BATTLE/HIT", "You", monster.meta.name) + "(-" + (int)result.damage + ")");
		};
		GameManager.Instance.player.on_defense = null;
		GameManager.Instance.player.on_defense += (Unit.AttackResult result) =>
		{
			StartCoroutine(GameManager.Instance.CameraFade(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.1f));
			iTween.ShakePosition(Camera.main.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);
			Effect_PlayerDamage effectPlayerDamage = player_damage_effects[Random.Range(0, player_damage_effects.Length)];
			effectPlayerDamage.gameObject.SetActive(false);
			effectPlayerDamage.transform.position = new Vector3(
				Random.Range(Screen.width / 2 - Screen.width / 2 * 0.6f, Screen.width / 2 + Screen.width / 2 * 0.6f),
				Random.Range(Screen.height / 2 - Screen.height / 2 * 0.3f, Screen.height / 2 + Screen.height / 2 * 0.9f),
				0.0f
			);
			effectPlayerDamage.gameObject.SetActive(true);

			UIDamageText damageText = GameObject.Instantiate<UIDamageText>(damage_text_prefab);
			damageText.gameObject.SetActive(false);
			damageText.Init(result, Color.red);
			damageText.transform.SetParent(player_health.transform, false);
			damageText.transform.localPosition = new Vector3(player_health.rect.x + player_health.rect.width * player_health.gauge.fillAmount, player_health.rect.y + player_health.rect.height / 2, 0.0f);
			damage_texts.Add(damageText);
			player_health.current = GameManager.Instance.player.cur_health;
		};

		monster.data.on_attack = null;
		monster.data.on_attack += (Unit.AttackResult result) =>
		{
			AudioManager.Instance.Play(AudioManager.BATTLE_ATTACK, false);
			monster.animator.SetTrigger("Attack");
			SceneDungeon.log.Write("<color=red>" + GameText.GetText("DUNGEON/BATTLE/HIT", monster.meta.name, "You") + "(-" + (int)result.damage + ")</color>");
		};

		monster.data.on_defense = null;
		monster.data.on_defense += (Unit.AttackResult result) =>
		{
			StartCoroutine(monster.OnDamage(result));
			UIDamageText damageText = GameObject.Instantiate<UIDamageText>(damage_text_prefab);
			damageText.gameObject.SetActive(false);
			damageText.Init(result, Color.white);
			if ("" == result.type)
			{
				damageText.life_time = 0.8f;
				damageText.transform.SetParent(monster.ui_health.transform, false);
				damageText.transform.localPosition = new Vector3(monster.ui_health.rect.x + monster.ui_health.rect.width * monster.ui_health.gauge.fillAmount, monster.ui_health.rect.y + monster.ui_health.rect.height / 2, 0.0f);
			}
			else
			{
				damageText.life_time = 3.0f;
				damageText.transform.SetParent(monster.damage_effect_spot, false);
				damageText.transform.localPosition = new Vector3(0.0f, 400.0f * GameManager.Instance.canvas.scaleFactor, 0.0f);
			}
			damage_texts.Add(damageText);
			monster.ui_health.current = monster.data.cur_health;
		};

		yield return StartCoroutine(monster.ColorTo(Color.black, Color.white, 1.0f));

		battle_pause = false;
		touch_input.block_count--;

		while (BattleResult.Invalid == battle_result)
		{
			yield return new WaitForSeconds(0.1f);
		}

		touch_input.block_count++;
		if (BattleResult.Win == battle_result)
		{
			yield return monster.Die();
		}
		else if (BattleResult.Lose == battle_result)
		{
			yield return StartCoroutine(GameManager.Instance.CameraFade(new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 0.0f), 1.0f));
		}
		
		monster.meta = null;
		monster.data = null;

		runaway_button.gameObject.SetActive(false);
		monster.gameObject.SetActive(false);
		touch_input.block_count++;
		
		Util.EventSystem.Publish(EventID.MiniMap_Show);
		Util.EventSystem.Publish<BattleResult>(EventID.Dungeon_Battle_Finish, battle_result);
	}

	float delta_time = 0;
	private void Update()
	{
		if (true == battle_pause || BattleResult.Invalid != battle_result)
		{
			return;
		}
		delta_time += Time.deltaTime;
		CooldownButton(Time.deltaTime * (1.0f / second_per_turn));
		if (second_per_turn > delta_time)
		{
			return;
		}

		delta_time -= second_per_turn;
		turn_count += 1;

		Unit attacker = null;
		Unit defender = null;
		if (enemy_preemptive_score < player_preemptive_score)
		{
			attacker = GameManager.Instance.player;
			defender = monster.data;
			enemy_preemptive_score += enemy_attack_per_second + Random.Range(0.8f, 1.6f);
		}
		else
		{
			attacker = monster.data;
			defender = GameManager.Instance.player;
			player_preemptive_score += player_attack_per_second;
		}

		if (0 < attacker.GetBuffCount(Buff.Type.Stun) || 0 < attacker.GetBuffCount(Buff.Type.Fear))
		{
			return;
		}

		attacker.OnBattleTurn();
		defender.OnBattleTurn();
		attacker.Attack(defender);
		
		if (null == show_damage_text_coroutine)
		{
			show_damage_text_coroutine = StartCoroutine(ShowDamageText());
		}

		if (0.0f >= monster.data.cur_health)
		{
			battle_result = BattleResult.Win;
		}
		else if (0.0f >= GameManager.Instance.player.cur_health)
		{
			battle_result = BattleResult.Lose;
		}
		else if (0 < GameManager.Instance.player.GetBuffCount(Buff.Type.Runaway))
		{
			battle_result = BattleResult.Runaway;
		}
	}

	private IEnumerator ShowDamageText()
	{
		if (0 == damage_texts.Count)
		{
			show_damage_text_coroutine = null;
			yield break;
		}

		UIDamageText damageText = damage_texts[0];
		damageText.transform.SetParent(player_health.transform);
		damageText.gameObject.SetActive(true);
		damage_texts.RemoveAt(0);

		if (0 < damage_texts.Count)
		{
			yield return new WaitForSeconds(0.25f);
		}
		StartCoroutine(ShowDamageText());
	}

	private void OnBuffEffect(Buff buff)
	{
		SceneDungeon.log.Write("on effect buff(" + buff.buff_name +")");
	}

	private void OnInventoryOpen()
	{
		touch_input.block_count++;
		battle_pause = true;
	}

	private void OnInventoryClose()
	{
		touch_input.block_count--;
		battle_pause = false;
	}

	private void EnableButton(bool flag)
	{
		foreach (UISkillButton skillButton in skill_buttons)
		{
			skillButton.skill_icon.gameObject.SetActive(flag);
			skillButton.enabled = flag;
		}
	}

	private void CooldownButton(float time)
	{
		foreach (UISkillButton skillButton in skill_buttons)
		{
			Skill skill = skillButton.skill;
			if (0 < skill.cooltime)
			{
				skill.cooltime -= time;
				skill.cooltime = Mathf.Max(0.0f, skill.cooltime);
			}

			skillButton.Cooldown();
		}

		Skill_Runaway runawaySkill = runaway_button.skill as Skill_Runaway;
		runaway_button.title.text = runawaySkill.meta.skill_name + "(" + runawaySkill.remain_count + "/" + (runawaySkill.meta as Skill_Runaway.Meta).max_count + ")";
		if (0 == runawaySkill.remain_count)
		{
			runaway_button.skill_icon.gameObject.SetActive(false);
			runaway_button.enabled = false;
		}
	}

	private void InitButtons()
	{
		while (0 < skill_button_spot.childCount)
		{
			Destroy(skill_button_spot.GetChild(0).gameObject);
			skill_button_spot.GetChild(0).transform.SetParent(null);
		}

		GameManager.Instance.player.RemoveSkill("SKILL_RUNAWAY");
		
		skill_button_spot.gameObject.SetActive(true);
		skill_buttons = new List<UISkillButton>();

		foreach (var itr in GameManager.Instance.player.skills)
		{
			UISkillButton skillButton = GameObject.Instantiate<UISkillButton>(skill_button_prefab);
			Skill skill = itr.Value.skill_data;
			skillButton.Init(skill, () => 
			{
				if (0 < skill.cooltime)
				{
					SceneDungeon.log.Write("can not use skill");
					return;
				}

				GameManager.Instance.player.current_skill = skill;
				skill.cooltime = skill.meta.cooltime;
				skillButton.skill_icon.fillAmount = 0.0f;
			});
			skill_buttons.Add(skillButton);
			skillButton.transform.SetParent(skill_button_spot, false);
		}

		runaway_button.gameObject.SetActive(true);
		GameManager.Instance.player.AddSkill(SkillManager.Instance.FindMeta<Skill_Runaway.Meta>("SKILL_RUNAWAY").CreateInstance());
		{
			Skill skill = GameManager.Instance.player.skills["SKILL_RUNAWAY"].skill_data;
			runaway_button.Init(skill, () =>
			{
				if (0 < skill.cooltime)
				{
					SceneDungeon.log.Write("can not use skill");
					return;
				}
				GameManager.Instance.player.current_skill = skill;
				skill.cooltime = skill.meta.cooltime;
				runaway_button.skill_icon.fillAmount = 0.0f;
			});
			skill_buttons.Add(runaway_button);
		}
	}
}
