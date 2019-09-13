using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

public class DungeonBattle : MonoBehaviour
{
	private float battle_speed = 1.5f;
	private Monster.Meta monster_meta;
	private Unit monster;
	private Transform monster_ui;
	private Text monster_name;
	private UIGaugeBar monster_health;
	private SpriteRenderer monster_sprite;
	private Animator monster_animator;
	private Transform damage_effect_spot;
	private Effect_MonsterDamage monster_damage_effect_prefab;
	private Transform monster_death_effect_prefab;
	private UIGaugeBar player_health;
	private Effect_PlayerDamage player_damage_effect_prefab;
	private UIButtonGroup battle_buttons;

	public bool battle_result = false;

	void Start()
	{
		monster_ui = UIUtil.FindChild<Transform>(transform, "../UI/Monster");
		monster_name = UIUtil.FindChild<Text>(monster_ui, "Name");
		monster_health = UIUtil.FindChild<UIGaugeBar>(monster_ui, "Health");
		monster_sprite = UIUtil.FindChild<SpriteRenderer>(transform, "Monster");
		monster_animator = UIUtil.FindChild<Animator>(transform, "Monster");

		damage_effect_spot = UIUtil.FindChild<Transform>(transform, "../UI/Battle");
		monster_damage_effect_prefab = UIUtil.FindChild<Effect_MonsterDamage>(damage_effect_spot, "Effect_MonsterDamage");
		monster_death_effect_prefab = UIUtil.FindChild<Transform>(damage_effect_spot, "Effect_MonsterDeath/BloodDroplets");

		player_health = UIUtil.FindChild<UIGaugeBar>(transform, "../UI/Player/Health");
		player_damage_effect_prefab = UIUtil.FindChild<Effect_PlayerDamage>(damage_effect_spot, "Effect_PlayerDamage");

		battle_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "../UI/Battle/BattleButtonGroup");

		monster_ui.gameObject.SetActive(false);
		monster_sprite.gameObject.SetActive(false);
		battle_buttons.gameObject.SetActive(false);
	}

	public IEnumerator BattleStart(Monster.Meta monsterMeta)
	{
		monster_meta = monsterMeta;
		monster = new Unit()
		{
			max_health = monster_meta.health,
			cur_health = monster_meta.health,
			attack = monster_meta.attack,
			defense = monster_meta.defense,
			speed = monster_meta.speed,
			critical = 0.0f
		};
		monster_health.max = monster.max_health;
		monster_health.current = monster.cur_health;
		monster_ui.gameObject.SetActive(true);
		monster_sprite.gameObject.SetActive(true);

		battle_buttons.gameObject.SetActive(true);
		//battle_buttons.names [0].text = "Heal(" + GamePlayer.Instance.inventory.GetItems<HealingPotionItem> ().Count.ToString() + ")";

		yield return StartCoroutine(ShowMonster(0.5f));

		// attack per second
		float playerAPS = GameManager.Instance.player.speed / monster_meta.speed;
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;

		while (0.0f < monster.cur_health && 0.0f < GameManager.Instance.player.cur_health)
		{
			float waitTime = 0.0f;
			if (monsterTurn < playerTurn)
			{
				int attackCount = 1;
				if (0 == Random.Range(0, 5))
				{
					attackCount = 2;
					waitTime = 1.0f / battle_speed / 2;
				}
				if (0 == Random.Range(0, 10))
				{
					attackCount = 3;
					waitTime = 1.0f / battle_speed / 3;
				}

				for (int i = 0; i < attackCount; i++)
				{
					iTween.ShakePosition(monster_sprite.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);
					Effect_MonsterDamage effect = GameObject.Instantiate<Effect_MonsterDamage>(monster_damage_effect_prefab);

					int damage = CalculateDamage(GameManager.Instance.player, monster);
					effect.damage = damage;
					effect.gameObject.SetActive(true);
					monster.cur_health -= damage;
					monster_health.current = monster.cur_health;
				}
				monsterTurn += monsterAPS + Random.Range(0, monsterAPS * 0.1f);
			}
			else
			{
				monster_animator.SetTrigger("Attack");
				int damage = CalculateDamage(monster, GameManager.Instance.player);

				StartCoroutine(GameManager.Instance.CameraFade(Color.white, new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.1f));
				iTween.ShakePosition(Camera.main.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);
				Effect_PlayerDamage bloodMark = GameObject.Instantiate<Effect_PlayerDamage>(player_damage_effect_prefab);
				bloodMark.gameObject.SetActive(true);
				bloodMark.transform.SetParent(damage_effect_spot, false);
				bloodMark.transform.position = new Vector3(
					Random.Range(Screen.width / 2 - Screen.width / 2 * 0.85f, Screen.width / 2 + Screen.width / 2 * 0.9f),
					Random.Range(Screen.height / 2 - Screen.height / 2 * 0.85f, Screen.height / 2 + Screen.height / 2 * 0.9f),
					0.0f
				);
				playerTurn += playerAPS + Random.Range(0, playerAPS * 0.1f);
				GameManager.Instance.player.cur_health -= damage;
				player_health.current = GameManager.Instance.player.cur_health;
			}
			yield return new WaitForSeconds(1.0f / battle_speed);
		}

		battle_buttons.gameObject.SetActive(false);

		if (0.0f < monster.cur_health)
		{
			battle_result = false;
		}
		else
		{
			Transform deathEffect = GameObject.Instantiate<Transform>(monster_death_effect_prefab);
			deathEffect.gameObject.SetActive(true);
			Object.Destroy(deathEffect.gameObject, 1.0f);
			monster_sprite.gameObject.SetActive(false);
			monster_ui.gameObject.SetActive(false);
			battle_result = true;
		}
		battle_buttons.gameObject.SetActive(false);
	}

	private IEnumerator ShowMonster(float time)
	{
		float color = 0.0f;
		while (1.0f > color)
		{
			color += Time.deltaTime / time;
			monster_sprite.color = new Color(color, color, color, 1.0f);
			yield return null;
		}
		monster_sprite.color = Color.white;
		yield break;
	}

	private int CalculateDamage(Unit attacker, Unit defender)
	{
		float attack = attacker.attack + Random.Range(-attacker.attack * 0.1f, attacker.attack * 0.1f);
		float defense = defender.defense + Random.Range(-defender.defense * 0.1f, defender.defense * 0.1f);
		float damage = Mathf.Max(1, attack - defense);

		if (attacker.critical >= Random.Range(0.0f, 100.0f))
		{
			damage *= 3;
			// critical effect
		}
		return (int)damage;
	}
}
