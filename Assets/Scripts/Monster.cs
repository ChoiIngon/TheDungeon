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
	public SpriteRenderer renderer;
	public Animator animator;
	public UIMonster ui;
	void Start () {
		renderer = transform.FindChild ("Sprite").GetComponent<SpriteRenderer>();
		animator = transform.FindChild ("Sprite").GetComponent<Animator> ();
		gameObject.SetActive (false);
		ui.gameObject.SetActive (false);
	}

	public void Init(Info info)
	{
		this.info = info;
		data = new Data ();
		data.health = info.health;
		renderer.sprite = info.sprite;
		ui.Init (info);
		gameObject.SetActive (true);
		ui.gameObject.SetActive (true);
		StartCoroutine (Attack ());
	}

	public IEnumerator Attack()
	{
		while (0.0f < data.health) {
			yield return new WaitForSeconds (100.0f / info.speed);
			animator.SetTrigger ("Attack");
			data.health -= 10.0f;
			ui.health.current = data.health;
		}

		gameObject.SetActive (false);
		ui.gameObject.SetActive (false);
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
