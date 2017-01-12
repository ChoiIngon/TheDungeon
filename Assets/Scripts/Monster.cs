using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : MonoBehaviour {

	[System.Serializable]
	public class Info
	{
		[System.Serializable]
		public class Reward
		{
			public int gold;
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
	[System.Serializable]
	public class Data
	{
		public float health;
	}
	// Use this for initialization

	public Info info;
	public Data data;
	public Animator animator;

	public TrailRenderer trailPrefab;
	public GameObject damagePrefab;
	public GaugeBar health;

	void Start () {
		animator = transform.FindChild ("Sprite").GetComponent<Animator> ();
		gameObject.SetActive (false);
		{
			MeshRenderer meshRenderer = transform.FindChild ("Name").GetComponent<MeshRenderer> ();
			meshRenderer.sortingLayerName = "UI";
		}


		health = transform.FindChild ("Health").GetComponent<GaugeBar> ();
	}

	public void Init(Info info)
	{
		this.info = info;
		data = new Data ();
		data.health = info.health;
		health.max = info.health;
		health.current = data.health;
		transform.FindChild ("Sprite").GetComponent<SpriteRenderer>().sprite = info.sprite;
		//ui.Init (info);
		TextMesh text = transform.FindChild ("Name").GetComponent<TextMesh> ();
		text.text = info.name;
		gameObject.SetActive (true);
		StartCoroutine (Battle ());
	}

	public IEnumerator Battle()
	{
		yield return new WaitForSeconds (1.0f);
		while (0.0f < data.health) {
			
			//animator.SetTrigger ("Attack");
			int attackCount = 1;
			float waitTime = 0.0f;
			if (0 == Random.Range (0, 3)) {
				attackCount += 1;
				waitTime = 0.5f;
			}
			if (0 == Random.Range (0, 5)) {
				attackCount += 1;
				waitTime = 0.2f;
			}

			for (int i = 0; i < attackCount; i++) {
				Damage (10);
				GameObject.Instantiate<GameObject> (damagePrefab);
				yield return new WaitForSeconds (waitTime);
			}

			yield return new WaitForSeconds (80.0f / info.speed - waitTime);

		}
		GameObject.Instantiate<GameObject> (damagePrefab);
		gameObject.SetActive (false);

	}

	public void Damage(int damage)
	{
		TrailRenderer trail = GameObject.Instantiate<TrailRenderer> (trailPrefab);
		trail.sortingLayerName = "Monster";
		trail.sortingOrder = 1;

		data.health -= damage;
		health.current = data.health;
		//ui.health.current = data.health;

		const float length = 6.0f;
		Vector3 direction = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), 0.0f);
		direction = length * direction.normalized;

		Vector3 start = -1.0f * direction;
		start.y += 1.0f;
		direction.y += 1.0f;
		trail.transform.localPosition = start;
		iTween.MoveTo(trail.gameObject, direction, 0.3f);
		iTween.ShakePosition (gameObject, new Vector3 (0.3f, 0.3f, 0.0f), 0.3f);
	}

	public static Dictionary<string, Info> infos = new Dictionary<string, Info> ();
	public static void Init()
	{
		TextAsset text = (TextAsset)ResourceManager.Instance.Load ("info_monster");
		CSVReader reader = new CSVReader ("Config/info_monster");

		foreach (CSVReader.Row row in reader) {
			Info info = new Info ();
			info.id = row ["MONSTER_ID"];
			info.name = row ["MONSTER_NAME"];
			info.level = int.Parse (row ["MONSTER_LEVEL"]);
			info.health = int.Parse (row ["HEALTH"]);
			info.defense = float.Parse (row ["DEFENSE"]);
			info.speed = float.Parse (row ["SPEED"]);
			info.sprite = (Sprite)ResourceManager.Instance.Load(row ["SPRITE_PATH"]);
			info.reward.gold = int.Parse (row ["REWARD_GOLD"]);
			info.reward.exp = int.Parse (row ["REWARD_EXP"]);

			infos.Add (info.id, info);
		}
	}

	public static Info FindInfo(string id)
	{
		if (false == infos.ContainsKey (id)) {
			return null;
		}

		return infos[id];
	}
}
