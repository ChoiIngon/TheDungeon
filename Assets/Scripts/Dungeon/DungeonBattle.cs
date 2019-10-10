using UnityEngine;
using System.Collections;

public class DungeonBattle : MonoBehaviour
{
	private TouchInput			touch_input;

	private Monster				monster;

	private UIGaugeBar			player_health;
	private Transform			player_damage_effect_spot;
	private Effect_PlayerDamage player_damage_effect_prefab;

	private UIButtonGroup		battle_buttons;
	private float				battle_speed = 1.5f;
	private float				wait_time_for_next_turn = 0.0f;

	public enum BattleResult
	{
		Lose = 0,
		Win = 1,
		Draw = 2
	}
	public BattleResult			battle_result = BattleResult.Lose; // 0 : lose, 1 : win, 2 : draw

	private void Awake()
	{
		monster = UIUtil.FindChild<Monster>(transform, "Monster");
		player_damage_effect_spot = UIUtil.FindChild<Transform>(transform, "../UI/BattleEffect");
		player_damage_effect_prefab = UIUtil.FindChild<Effect_PlayerDamage>(player_damage_effect_spot, "Effect_PlayerDamage");
		player_health = UIUtil.FindChild<UIGaugeBar>(transform, "../UI/Player/Health");
		battle_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "../UI/Monster/BattleButtonGroup");
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
	}
	void Start()
	{
		battle_buttons.Init();

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

	public IEnumerator BattleStart(Monster.Meta monsterMeta)
	{
		gameObject.SetActive(true);
		monster.gameObject.SetActive(true);
		battle_buttons.gameObject.SetActive(true);
		battle_buttons.Show(0.5f);

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
				PlayerAttack(1.0f);
				monsterTurn += monsterAPS + Random.Range(0, monsterAPS * 0.1f);
			}
			else
			{
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
					GameManager.Instance.player.cur_health -= result.damage;
					player_health.current = GameManager.Instance.player.cur_health;
				}
				playerTurn += playerAPS + Random.Range(0, playerAPS * 0.1f);
			}

			GameManager.Instance.player.OnBattleTurn();
			monster.data.OnBattleTurn();
			wait_time_for_next_turn = 1.0f / battle_speed;
			while (0.0f < wait_time_for_next_turn)
			{
				wait_time_for_next_turn -= Time.deltaTime;
				yield return null;
			}
		}

		if (0.0f < monster.data.cur_health)
		{
			battle_result = BattleResult.Lose;
			// return to main
		}
		else
		{
			monster.Die();
			battle_result = BattleResult.Win;
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
			StartCoroutine(monster.OnDamage(result));
		}
	}
}
