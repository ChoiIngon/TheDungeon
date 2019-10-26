using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DungeonBattle : MonoBehaviour
{
	private TouchInput			touch_input;

	private Monster				monster;

	private UIGaugeBar			player_health;
	private Transform			player_damage_effect_spot;
	private Effect_PlayerDamage[] player_damage_effects;

	private UIButtonGroup		battle_buttons;
	private float				battle_speed = 1.1f;
	private float				wait_time_for_next_turn = 0.0f;

	private Button runaway_button;
	private bool battle_pause;
	private bool runaway;
	private int runaway_count;
	public enum BattleResult
	{
		Lose = 0,
		Win = 1,
		Runaway = 2
	}
	public BattleResult			battle_result = BattleResult.Lose; // 0 : lose, 1 : win, 2 : draw
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
		battle_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "../UI/Battle/SkillButtons");
		runaway_button = UIUtil.FindChild<Button>(transform, "../UI/Battle/Runaway");

		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}

		Util.EventSystem.Subscribe<Buff>(EventID.Buff_Effect, OnBuffEffect);
	}
	void Start()
	{
		touch_input.onTouchDown += (Vector3 position) =>
		{
			if (null == monster.meta)
			{
				return;
			}
			wait_time_for_next_turn = 0.0f;
		};
		touch_input.AddBlockCount();
		battle_buttons.gameObject.SetActive(false);

		UIUtil.AddPointerUpListener(runaway_button.gameObject, () =>
		{
			touch_input.AddBlockCount();
			battle_pause = true;
			float successChance = (GameManager.Instance.player.speed / monster.meta.speed) * 0.25f;
			if (successChance > Random.Range(0.0f, 1.0f))
			{
				runaway = true;
				battle_pause = false;
				SceneDungeon.log.Write("You runaway.");
			}
			else
			{
				battle_pause = false;
				SceneDungeon.log.Write("You trid to runaway. but failed..");
				if (0 == --runaway_count)
				{
					runaway_button.image.color = Color.gray;
				}
			}
			UIUtil.FindChild<Text>(runaway_button.transform, "Text").text = runaway_count.ToString() + "/" + 3.ToString();
			touch_input.ReleaseBlockCount();
		});
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Buff>(EventID.Buff_Effect, OnBuffEffect);
	}

	public IEnumerator BattleStart(Monster.Meta monsterMeta)
	{
		Util.EventSystem.Publish<float>(EventID.MiniMap_Hide, 0.0f);
		gameObject.SetActive(true);
		monster.gameObject.SetActive(true);
		battle_buttons.gameObject.SetActive(true);
		battle_buttons.Init();

		battle_pause = false;
		runaway = false;
		runaway_count = 3;
		runaway_button.image.color = Color.white;
		UIUtil.FindChild<Text>(runaway_button.transform, "Text").text = runaway_count.ToString() + "/" + "3";

		{
			UIButtonGroup.UIButton button = null;
			List<HealPotionItem> items = GameManager.Instance.player.inventory.GetItems<HealPotionItem>();
			if (0 < items.Count)
			{
				int itemCount = items.Count;
				int itemIndex = 0;
				battle_buttons.AddButton((itemCount - itemIndex).ToString() + "/" + itemCount, ResourceManager.Instance.Load<Sprite>("Item/item_potion_red"), () =>
				{
					HealPotionItem item = items[itemIndex++];
					GameManager.Instance.player.inventory.Remove(item.slot_index);
					item.Use(GameManager.Instance.player);
					button.title = (itemCount - itemIndex).ToString() + "/" + itemCount;

					if (0 == itemCount - itemIndex)
					{
						button.button.gameObject.SetActive(false);
					}
				});
				button = battle_buttons.buttons[battle_buttons.buttons.Count - 1];
			}
		}

		battle_buttons.Show(0.5f);

		monster.Init(monsterMeta);
		yield return StartCoroutine(monster.ColorTo(Color.black, Color.white, 1.0f));

		touch_input.ReleaseBlockCount();
		// attack per second
		float playerAPS = GameManager.Instance.player.speed / monster.meta.speed;
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;
		int playerDamageEffectIndex = 0;

		Unit defender = null;
		while (0.0f < monster.data.cur_health && 0.0f < GameManager.Instance.player.cur_health)
		{
			if (monsterTurn < playerTurn)
			{
				defender = monster.data;
				GameManager.Instance.player.OnBattleTurn();
				monster.data.OnBattleTurn();

				Unit.AttackResult result = GameManager.Instance.player.Attack(monster.data);
				if (0.0f < result.damage)
				{
					SceneDungeon.log.Write(GameText.GetText("DUNGEON/BATTLE/HIT", "You", monster.meta.name) + "(-" + (int)result.damage + ")");
					StartCoroutine(monster.OnDamage(result));
				}
				else
				{
					continue;
				}
				monsterTurn += monsterAPS + Random.Range(1.0f, 2.0f);
			}
			else
			{
				defender = GameManager.Instance.player;
				monster.data.OnBattleTurn();
				GameManager.Instance.player.OnBattleTurn();

				Unit.AttackResult result = monster.data.Attack(GameManager.Instance.player);
				if (0.0f < result.damage)
				{
					monster.animator.SetTrigger("Attack");
					StartCoroutine(GameManager.Instance.CameraFade(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.1f));
					iTween.ShakePosition(Camera.main.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);
					Effect_PlayerDamage effectPlayerDamage = player_damage_effects[playerDamageEffectIndex++];
					playerDamageEffectIndex = playerDamageEffectIndex % player_damage_effects.Length;
					effectPlayerDamage.gameObject.SetActive(false);
					effectPlayerDamage.transform.position = new Vector3(
						Random.Range(Screen.width / 2 - Screen.width / 2 * 0.85f, Screen.width / 2 + Screen.width / 2 * 0.9f),
						Random.Range(Screen.height / 2 - Screen.height / 2 * 0.85f, Screen.height / 2 + Screen.height / 2 * 0.9f),
						0.0f
					);
					effectPlayerDamage.gameObject.SetActive(true);

					SceneDungeon.log.Write("<color=red>" + GameText.GetText("DUNGEON/BATTLE/HIT", monster.meta.name, "You") + "(-" + (int)result.damage + ")</color>");
					GameManager.Instance.player.cur_health -= result.damage;
					player_health.current = GameManager.Instance.player.cur_health;
				}
				else
				{
					continue;
				}
				playerTurn += playerAPS;
			}

			wait_time_for_next_turn = 1.0f / battle_speed;
			while (0.0f < wait_time_for_next_turn)
			{
				wait_time_for_next_turn -= Time.deltaTime;
				yield return null;
			}
			if (true == runaway)
			{
				break;
			}
			while (true == battle_pause)
			{
				yield return null;
			}
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
		else if(true == runaway)
		{
			battle_result = BattleResult.Runaway;
		}

		monster.meta = null;
		monster.data = null;
		
		battle_buttons.gameObject.SetActive(false);
		monster.gameObject.SetActive(false);
		touch_input.AddBlockCount();
		gameObject.SetActive(false);
		Util.EventSystem.Publish(EventID.MiniMap_Show);
	}

	private void OnBuffEffect(Buff buff)
	{
		SceneDungeon.log.Write(buff.buff_id);
	}
}
