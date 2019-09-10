using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

public class Monster : MonoBehaviour
{
	public class Meta
	{
		public class Reward
		{
			public int coin;
			public int exp;
		}
		public string id;
		public string name;
		public int level;
		public float health;
		public float defense;
		public float attack;
		public float speed;
		public string sprite_path;
		public Reward reward;

		public Meta()
		{
			reward = new Reward();
		}
	}

	public Meta meta;

	public Transform ui_monster_info;
	private new Text name;
	public UIGaugeBar health;

	public TrailRenderer trailPrefab;
	private new SpriteRenderer renderer;

	public GameObject damageEffectPrefab;
	public GameObject dieEffectPrefab;

	private Animator animator;
	
	void Start()
	{
		animator = UIUtil.FindChild<Animator>(transform, "Sprite");
		renderer = UIUtil.FindChild<SpriteRenderer>(transform, "Sprite");
		ui_monster_info = UIUtil.FindChild<Transform>(transform, "../UI/UIMonsterInfo");
		name = UIUtil.FindChild<Text>(ui_monster_info, "Name");
		health = UIUtil.FindChild<UIGaugeBar>(ui_monster_info, "Health");
		gameObject.SetActive(false);

		ui_monster_info.gameObject.SetActive(false);
	}

	public void Init(Meta meta)
	{
		gameObject.SetActive (true);
		ui_monster_info.gameObject.SetActive (true);
		name.text = meta.name;
		renderer.sprite = ResourceManager.Instance.Load<Sprite>(meta.sprite_path);
		renderer.color = Color.black;
		health.max = meta.health;
		health.current = meta.health;
	}

    /*
	public override void Attack(Unit defender)
	{
		animator.SetTrigger ("Attack");
		Stat stat = GetStat ();
		int attack = (int)Mathf.Max(1, stat.attack + Random.Range(0, stat.attack * 0.1f) - defender.stats.defense);
		defender.Damage (attack);
	}

	public override void Damage(int damage)
	{
        Health((int)health.current - damage);

        TrailRenderer trail = GameObject.Instantiate<TrailRenderer> (trailPrefab);
		trail.sortingLayerName = "Effect";
		trail.sortingOrder = 1;

		const float length = 4.0f;
		Vector3 direction = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), 0.0f);
		direction = length * direction.normalized;

		Vector3 start = -1.0f * direction;
		start.y += 0.5f;
		direction.y += 0.5f;
		trail.transform.localPosition = start;
		GameObject effect = GameObject.Instantiate<GameObject> (damageEffectPrefab);
		effect.transform.position = Vector3.Lerp(start, direction, Random.Range(0.4f, 0.6f));
		MeshRenderer renderer = effect.transform.Find ("Text").GetComponent<MeshRenderer> ();
		renderer.sortingLayerName = "Effect";

		TextMesh text = effect.transform.Find ("Text").GetComponent<TextMesh> ();
		text.text = "-" + damage.ToString ();
		iTween.MoveTo(trail.gameObject, direction, 0.3f);
		iTween.ShakePosition (gameObject, new Vector3 (0.3f, 0.3f, 0.0f), 0.1f);
	}

    public override void Health(int health)
    {
        StartCoroutine(this.health.DeferredValue((float)health, 0.2f));
    }

    */
    public IEnumerator Show(float time)
	{
		float color = 0.0f;
		yield return new WaitForSeconds (time * 0.6f);
		while (1.0f > color) {
			renderer.color = new Color (color, color, color, 1.0f);
			color += Time.deltaTime / (time - time*0.6f);
			yield return null;
		}
		renderer.color = Color.white;
	}
}
