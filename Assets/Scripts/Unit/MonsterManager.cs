using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Util.Singleton<MonsterManager> {
	[System.Serializable]
	public class MonsterInfo : Monster.Info
	{
		public string sprite_path;
	}
	[System.Serializable]
	public class Config
	{
		public MonsterInfo[] monsters;
	}
	private Dictionary<string, Monster.Info> infoDict;
	private List<Monster.Info> infoList;
	public Config config;
	public IEnumerator Init() {
		infoDict = new Dictionary<string, Monster.Info> ();
		infoList = new List<Monster.Info> ();

		yield return NetworkManager.Instance.HttpRequest ("info_monster.php", (string json) => {
			config = JsonUtility.FromJson<Config>(json);
			foreach (MonsterInfo monster in config.monsters) {
				Monster.Info info = new Monster.Info ();
				info.id = monster.id;
				info.name = monster.name;
				info.level = monster.level;
				info.health = monster.health;
				info.attack = monster.attack;
				info.defense = monster.defense;
				info.speed = monster.speed;
				info.sprite = ResourceManager.Instance.Load<Sprite>(monster.sprite_path);
				info.reward.coin = monster.reward.coin;
				info.reward.exp = monster.reward.exp;

				infoDict.Add (info.id, info);
				infoList.Add (info);
			}
		});
		/*
		CSVReader reader = new CSVReader ("Config/info_monster");

		foreach (CSVReader.Row row in reader) {
			Monster.Info info = new Monster.Info ();
			info.id = row ["MONSTER_ID"];
			info.name = row ["MONSTER_NAME"];
			info.level = int.Parse (row ["MONSTER_LEVEL"]);
			info.health = int.Parse (row ["HEALTH"]);
			info.attack = float.Parse (row ["ATTACK"]);
			info.defense = float.Parse (row ["DEFENSE"]);
			info.speed = float.Parse (row ["SPEED"]);
			info.sprite = ResourceManager.Instance.Load<Sprite>(row ["SPRITE_PATH"]);
			info.reward.coin = int.Parse (row ["REWARD_COIN"]);
			info.reward.exp = int.Parse (row ["REWARD_EXP"]);

			infoDict.Add (info.id, info);
			//infoList.Add (info);
		}
		Debug.Log ("init complete MonsterManager");
		*/
	}

	public Monster.Info FindInfo(string id)
	{
		#if UNITY_EDITOR
		Assert.AreNotEqual(false, infoDict.ContainsKey (id));
		#endif
		if (false == infoDict.ContainsKey (id)) {
			return null;
		}
		return infoDict[id];
	}

	public Monster.Info GetRandomMonster(int level) {
		#if UNITY_EDITOR
		Assert.AreNotEqual(0, infoList.Count);
		#endif
		Monster.Info info = infoList[Random.Range(0, infoList.Count)];
		return info;
	}
}