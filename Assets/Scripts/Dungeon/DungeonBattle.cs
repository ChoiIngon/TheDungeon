﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonBattle : MonoBehaviour
{
	private TouchInput			touch_input;

	private Monster				monster;

	private UIGaugeBar			player_health;
	private Transform			player_damage_effect_spot;
	private Effect_PlayerDamage player_damage_effect_prefab;

	private UIBattleLog			battle_log;
	private UIButtonGroup		battle_buttons;
	private float				battle_speed = 1.1f;
	private float				wait_time_for_next_turn = 0.0f;
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
		player_damage_effect_prefab = UIUtil.FindChild<Effect_PlayerDamage>(player_damage_effect_spot, "Effect_PlayerDamage");
		player_health = UIUtil.FindChild<UIGaugeBar>(transform, "../UI/Player/Health");
		battle_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "../UI/Battle/SkillButtons");
		battle_log = UIUtil.FindChild<UIBattleLog>(transform, "../UI/Battle/BattleLog");

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
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Buff>(EventID.Buff_Effect, OnBuffEffect);
	}

	public IEnumerator BattleStart(Monster.Meta monsterMeta)
	{
		gameObject.SetActive(true);
		monster.gameObject.SetActive(true);
		battle_buttons.gameObject.SetActive(true);
		battle_buttons.Init();

		bool runaway = false;
		bool pause = false;
		int runawayCount = 3;

		{
			UIButtonGroup.UIButton button = null;
			List<HealPotionItem> items = GameManager.Instance.player.inventory.GetItems<HealPotionItem>();
			if (0 < items.Count)
			{
				int itemCount = items.Count;
				int itemIndex = 0;
				battle_buttons.AddButton((itemCount - itemIndex).ToString() + "/" + itemCount, ResourceManager.Instance.Load<Sprite>("Item/item_potion_002"), () =>
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

		{
			UIButtonGroup.UIButton button = null;
			battle_buttons.AddButton("run", ResourceManager.Instance.Load<Sprite>("Skill/skill_icon_run"), () =>
			{
				touch_input.AddBlockCount();
				pause = true;

				float successChance = (GameManager.Instance.player.speed / monster.meta.speed) * 0.5f;
				if (successChance > Random.Range(0.0f, 1.0f))
				{
					pause = false;
					runaway = true;
					battle_log.AsyncWrite("You runaway.");
				}
				else
				{
					pause = false;
					battle_log.AsyncWrite("You trid to runaway. but failed..");
					if (0 < runawayCount--)
					{
						button.button.gameObject.SetActive(false);
					}
				}				
				touch_input.ReleaseBlockCount();
			});
			button = battle_buttons.buttons[battle_buttons.buttons.Count - 1];
		}

		battle_buttons.Show(0.5f);

		battle_log.Init();
		monster.Init(monsterMeta);
		yield return StartCoroutine(monster.ColorTo(Color.black, Color.white, 1.0f));

		touch_input.ReleaseBlockCount();
		// attack per second
		float playerAPS = GameManager.Instance.player.speed / monster.meta.speed;
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;

		while (0.0f < monster.data.cur_health && 0.0f < GameManager.Instance.player.cur_health)
		{
			if (monsterTurn < playerTurn)
			{
				GameManager.Instance.player.OnBattleTurn();
				monster.data.OnBattleTurn();

				PlayerAttack(1.0f);
				monsterTurn += monsterAPS + Random.Range(1.0f, 2.0f);
			}
			else
			{
				monster.data.OnBattleTurn();
				GameManager.Instance.player.OnBattleTurn();

				Unit.AttackResult result = monster.data.Attack(GameManager.Instance.player);
				if (0.0f < result.damage)
				{
					monster.animator.SetTrigger("Attack");
					StartCoroutine(GameManager.Instance.CameraFade(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.1f));
					iTween.ShakePosition(Camera.main.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);
					Effect_PlayerDamage effectPlayerDamage = GameObject.Instantiate<Effect_PlayerDamage>(player_damage_effect_prefab);
					effectPlayerDamage.gameObject.SetActive(true);
					effectPlayerDamage.transform.SetParent(player_damage_effect_spot, false);
					effectPlayerDamage.transform.position = new Vector3(
						Random.Range(Screen.width / 2 - Screen.width / 2 * 0.85f, Screen.width / 2 + Screen.width / 2 * 0.9f),
						Random.Range(Screen.height / 2 - Screen.height / 2 * 0.85f, Screen.height / 2 + Screen.height / 2 * 0.9f),
						0.0f
					);
					battle_log.AsyncWrite("<color=red>" + GameText.GetText("DUNGEON/BATTLE/HIT", monster.meta.name, "You") + "(-" + (int)result.damage + ")</color>");
					GameManager.Instance.player.cur_health -= result.damage;
					player_health.current = GameManager.Instance.player.cur_health;
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
			while (true == pause)
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
	}

	private void PlayerAttack(float damageRate)
	{
		Unit.AttackResult result = GameManager.Instance.player.Attack(monster.data);
		result.damage *= damageRate;
		if (0.0f < result.damage)
		{
			battle_log.AsyncWrite(GameText.GetText("DUNGEON/BATTLE/HIT", "You", monster.meta.name) + "(-" + (int)result.damage + ")");
			StartCoroutine(monster.OnDamage(result));
		}
	}

	private void OnBuffEffect(Buff buff)
	{
		battle_log.AsyncWrite(buff.buff_id);
	}
}
