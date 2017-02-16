using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

public class Monster : Unit {
	[System.Serializable]
	public class Info
	{
		[System.Serializable]
		public class Reward
		{
			public int coin;
			public int exp;
		}
		public string id;
		public string name;
		public int level;
		public int health;
		public float defense;
		public float attack;
		public float speed;
		public Sprite sprite;
		public Reward reward;

		public Info()
		{
			reward = new Reward();
		}
	}

	public Transform ui;
	private new Text name;
	public UIGaugeBar health;

	public Info info;
	public TrailRenderer trailPrefab;
	public GameObject damageEffectPrefab;
	public GameObject dieEffectPrefab;

	private Animator animator;
	private new SpriteRenderer renderer;

	void Start () {
		animator = transform.FindChild ("Sprite").GetComponent<Animator> ();
		renderer = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ();
		name = ui.FindChild ("Name").GetComponent<Text> ();
		health = ui.FindChild ("Health").GetComponent<UIGaugeBar> ();

		#if UNITY_EDITOR
		Assert.AreNotEqual(null, animator);
		Assert.AreNotEqual(null, renderer);
		Assert.AreNotEqual(null, name);
		Assert.AreNotEqual(null, health);
		Assert.AreNotEqual(null, ui);
		#endif
		gameObject.SetActive (false);
		ui.gameObject.SetActive (false);
	}

	public void Init(Info info_)
	{
		gameObject.SetActive (true);
		ui.gameObject.SetActive (true);

		info = info_;
		name.text = info.name;
		renderer.sprite = info.sprite;
		renderer.color = Color.black;
		stats.health = info.health;
		stats.attack = info.attack;
		stats.defense = info.defense;
		stats.speed = info.speed;
		stats.coinBonus = 0.0f;
		stats.critcal = 0.0f; //ToDo
		stats.expBonus = 0.0f;

		health.max = info.health;
		health.current = health.max;
	}

	public void OnDisable()
	{
		ui.gameObject.SetActive (false);
	}
	 
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

		TextMesh text = effect.transform.FindChild ("Text").GetComponent<TextMesh> ();
		text.text = "-" + damage.ToString ();
		iTween.MoveTo(trail.gameObject, direction, 0.3f);
		iTween.ShakePosition (gameObject, new Vector3 (0.3f, 0.3f, 0.0f), 0.1f);
	}

    public override void Health(int health)
    {
        StartCoroutine(this.health.DeferredValue((float)health, 0.2f));
    }

	public IEnumerator Show(float time)
	{
		float color = 0.0f;
		yield return new WaitForSeconds (time / 3.0f);
		while (1.0f > color) {
			renderer.color = new Color (color, color, color, 1.0f);
			color += Time.deltaTime / (time - time/3);
			yield return null;
		}
		renderer.color = Color.white;
	}
}
