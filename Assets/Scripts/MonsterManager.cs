using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Singleton<MonsterManager> {
	private Dictionary<string, Monster.Info> infoDict;
	private List<Monster.Info> infoList;
	public void Init() {
		infoDict = new Dictionary<string, Monster.Info> ();
		infoList = new List<Monster.Info> ();
		CSVReader reader = new CSVReader ("Config/info_monster");

		foreach (CSVReader.Row row in reader) {
			Monster.Info info = new Monster.Info ();
			info.id = row ["MONSTER_ID"];
			info.name = row ["MONSTER_NAME"];
			info.level = int.Parse (row ["MONSTER_LEVEL"]);
			info.health = int.Parse (row ["HEALTH"]);
			info.defense = float.Parse (row ["DEFENSE"]);
			info.speed = float.Parse (row ["SPEED"]);
			info.sprite = ResourceManager.Instance.Load<Sprite>(row ["SPRITE_PATH"]);
			info.reward.coin = int.Parse (row ["REWARD_COIN"]);
			info.reward.exp = int.Parse (row ["REWARD_EXP"]);

			infoDict.Add (info.id, info);
			infoList.Add (info);
		}
		Debug.Log ("init complete MonsterManager");
	}

	public Monster.Info FindInfo(string id)
	{
		if (false == infoDict.ContainsKey (id)) {
			return null;
		}
		return infoDict[id];
	}

	public Monster.Info GetRandomMonster(int level) {
		Monster.Info info = infoList[Random.Range(0, infoList.Count)];
		Debug.Log ("MonsterManager.GetRandomMonster(moster_id:" + info.id + ")");
		return info;
	}
}