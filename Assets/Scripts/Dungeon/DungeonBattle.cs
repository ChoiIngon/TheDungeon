using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DungeonBattle : MonoBehaviour
{
	private TouchInput			touch_input;	private Monster				monster;

	private UIGaugeBar			player_health;
	private Transform			player_damage_effect_spot;
	private Effect_PlayerDamage[] player_damage_effects;

	public UISkillButton			skill_button_prefab;
	private Transform				skill_button_spot;
	private List<UISkillButton>		skill_buttons;
	private UISkillButton			runaway_button;

	public UIDamageText damage_text_prefab;
	private List<UIDamageText> damage_texts;

	private bool battle_pause;
	
	private Coroutine show_damage_text_coroutine;

	public enum BattleResult
	{
		Invalid = 0,
		Win = 1,
		Lose = 2,
		Runaway = 3
	}
	public BattleResult			battle_result = BattleResult.Invalid; // 0 : lose, 1 : win, 2 : draw
	private void Awake()
	{
		monster = UIUtil.FindChild<Monster>(transform, "Monster");
		player_damage_effect_spot = UIUtil.FindChild<Transform>(transform, "../UI/BattleEffect");
		player_damage_effects = new Effect_PlayerDamage[2];
		for (int i = 0; i < player_damage_effects.Length; i++)
		{
			player_damage_effects[i] = GameObject.Instantiate<Effect_PlayerDamage>(UIUtil.FindChild<Effect_PlayerDamage>(player_damage_effect_spot, "Effect_PlayerDamage"));
			player_damage_effects[i].transform.SetParent(player_damage_effect_spot, false);
		}
		player_health = UIUtil.FindChild<UIGaugeBar>(transform, "../UI/Player/Health");
		skill_button_spot = UIUtil.FindChild<Transform>(transform, "../UI/Battle/SkillButtons");
		runaway_button = UIUtil.FindChild<UISkillButton>(transform, "../UI/SideButtons/RunawayButton");
		runaway_button.gameObject.SetActive(false);
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
		Util.EventSystem.Subscribe<Buff>(EventID.Buff_Effect, OnBuffEffect);
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
		touch_input.block_count++;
		skill_button_spot.gameObject.SetActive(false);
		damage_texts = new List<UIDamageText>();
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Buff>(EventID.Buff_Effect, OnBuffEffect);
	}

	public IEnumerator BattleStart(Monster.Meta monsterMeta)
	{
		Util.EventSystem.Publish(EventID.Dungeon_Battle_Start);
		Util.EventSystem.Publish<float>(EventID.MiniMap_Hide, 0.0f);

		gameObject.SetActive(true);
		battle_pause = false;

		InitButtons();

		monster.gameObject.SetActive(true);
		monster.Init(monsterMeta);
		// attack per second
		float playerAPS = GameManager.Instance.player.speed / monster.meta.speed;
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;
		int playerDamageEffectIndex = 0;

		GameManager.Instance.player.on_attack = null;
		GameManager.Instance.player.on_attack += (Unit.AttackResult result) =>
		{
			SceneDungeon.log.Write(GameText.GetText("DUNGEON/BATTLE/HIT", "You", monster.meta.name) + "(-" + (int)result.damage + ")");
		};
		GameManager.Instance.player.on_defense = null;
		GameManager.Instance.player.on_defense += (Unit.AttackResult result) =>
		{
			StartCoroutine(GameManager.Instance.CameraFade(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.1f));
			iTween.ShakePosition(Camera.main.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);
			Effect_PlayerDamage effectPlayerDamage = player_damage_effects[playerDamageEffectIndex++];
			playerDamageEffectIndex = playerDamageEffectIndex % player_damage_effects.Length;
			effectPlayerDamage.gameObject.SetActive(false);
			effectPlayerDamage.transform.position = new Vector3(
				Random.Range(Screen.width / 2 - Screen.width / 2 * 0.6f, Screen.width / 2 + Screen.width / 2 * 0.6f),
				Random.Range(Screen.height / 2 - Screen.height / 2 * 0.3f, Screen.height / 2 + Screen.height / 2 * 0.9f),
				0.0f
			);
			effectPlayerDamage.gameObject.SetActive(true);

			UIDamageText damageText = GameObject.Instantiate<UIDamageText>(damage_text_prefab);
			damageText.gameObject.SetActive(false);
			damageText.Init(result);
			damageText.transform.SetParent(player_health.transform, false);
			damageText.transform.localPosition = new Vector3(player_health.rect.x + player_health.rect.width * player_health.gauge.fillAmount, player_health.rect.y + player_health.rect.height / 2, 0.0f);
			damage_texts.Add(damageText);
			player_health.current = GameManager.Instance.player.cur_health;
		};

		monster.data.on_attack = null;
		monster.data.on_attack += (Unit.AttackResult result) =>
		{
			monster.animator.SetTrigger("Attack");
			SceneDungeon.log.Write("<color=red>" + GameText.GetText("DUNGEON/BATTLE/HIT", monster.meta.name, "You") + "(-" + (int)result.damage + ")</color>");
		};

		monster.data.on_defense = null;
		monster.data.on_defense += (Unit.AttackResult result) =>
		{
			StartCoroutine(monster.OnDamage(result));
			UIDamageText damageText = GameObject.Instantiate<UIDamageText>(damage_text_prefab);
			damageText.gameObject.SetActive(false);
			damageText.Init(result);
			damageText.transform.SetParent(monster.ui_health.transform, false);
			damageText.transform.localPosition = new Vector3(monster.ui_health.rect.x + monster.ui_health.rect.width * monster.ui_health.gauge.fillAmount, monster.ui_health.rect.y + monster.ui_health.rect.height / 2, 0.0f);
			damage_texts.Add(damageText);
			monster.ui_health.current = monster.data.cur_health;
		};

		yield return StartCoroutine(monster.ColorTo(Color.black, Color.white, 1.0f));

		while (true)
		{
			Unit attacker = null;
			Unit defender = null;
			if (monsterTurn < playerTurn)
			{
				attacker = GameManager.Instance.player;
				defender = monster.data;
				monsterTurn += monsterAPS + Random.Range(1.0f, 2.0f);

				battle_pause = true;
				EnableButton(true);
				CooldownButton(1.0f);
			}
			else
			{
				attacker = monster.data;
				defender = GameManager.Instance.player;
				playerTurn += playerAPS;
				
				battle_pause = false;
				EnableButton(false);
			}

			if (0 < attacker.GetBuffCount(Buff.Type.Stun) || 0 < attacker.GetBuffCount(Buff.Type.Fear))
			{
				//yield return new WaitForSeconds(0.5f);
				continue;
			}

			while (true == battle_pause)
			{
				yield return null;
			}

			EnableButton(false);

			attacker.OnBattleTurn();
			defender.OnBattleTurn();
			attacker.Attack(defender);

			if (0 < attacker.GetBuffCount(Buff.Type.Runaway))
			{
				break;
			}

			if (null == show_damage_text_coroutine)
			{
				show_damage_text_coroutine = StartCoroutine(ShowDamageText());
			}
			
			yield return new WaitForSeconds(0.5f);
			if (monster.data == attacker)
			{
				while (monster.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
				{
					yield return null;
				}
			}

			if (0.0f >= attacker.cur_health || 0.0f >= defender.cur_health)
			{
				break;
			}
		}

		while (0 < damage_texts.Count)
		{
			yield return null;
		}

		if (0.0f >= monster.data.cur_health)
		{
			monster.Die();
			battle_result = BattleResult.Win;
		}
		else if (0.0f >= GameManager.Instance.player.cur_health)
		{
			battle_result = BattleResult.Lose;
		}
		else if(0 < GameManager.Instance.player.GetBuffCount(Buff.Type.Runaway))
		{
			battle_result = BattleResult.Runaway;
		}
		
		monster.meta = null;
		monster.data = null;

		runaway_button.gameObject.SetActive(false);
		monster.gameObject.SetActive(false);
		touch_input.block_count++;
		gameObject.SetActive(false);
		Util.EventSystem.Publish(EventID.MiniMap_Show);
		Util.EventSystem.Publish<BattleResult>(EventID.Dungeon_Battle_Finish, battle_result);
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

	private void EnableButton(bool flag)
	{
		foreach (UISkillButton skillButton in skill_buttons)
		{
			skillButton.skill_icon.gameObject.SetActive(flag);
			skillButton.enabled = flag;
		}
	}

	private void CooldownButton(float turn)
	{
		foreach (UISkillButton skillButton in skill_buttons)
		{
			Skill skill = skillButton.skill;
			if (0 < skill.cooltime)
			{
				skill.cooltime -= turn;
				skill.cooltime = Mathf.Max(0.0f, skill.cooltime);
			}
			StartCoroutine(skillButton.Refresh());
		}

		Skill_Runaway runawaySkill = GameManager.Instance.player.skills["SKILL_RUNAWAY"].skill_data as Skill_Runaway;
		foreach (UISkillButton skillButton in skill_buttons)
		{
			if (runawaySkill == skillButton.skill)
			{
				skillButton.title.text = runawaySkill.meta.skill_name + "(" + runawaySkill.remain_count + "/" + (runawaySkill.meta as Skill_Runaway.Meta).max_count + ")";
				if (0 == runawaySkill.remain_count)
				{
					skillButton.skill_icon.gameObject.SetActive(false);
					skillButton.enabled = false;
				}

				break;
			}
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
				battle_pause = false;
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
				battle_pause = false;
			});
			skill_buttons.Add(runaway_button);
		}
	}
}
