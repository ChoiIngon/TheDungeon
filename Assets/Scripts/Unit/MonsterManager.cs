using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Util.Singleton<MonsterManager>
{
	private Dictionary<string, DungeonMonster.Meta> metas = new Dictionary<string, DungeonMonster.Meta>();
	private Util.WeightRandom<DungeonMonster.Meta> monster_gacha = new Util.WeightRandom<DungeonMonster.Meta>();

	public void Init()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT monster_id, monster_name, monster_level, health, attack, defense, speed, sprite_path, reward_coin, reward_exp FROM meta_monster"
		);

		while (true == reader.Read())
		{
			DungeonMonster.Meta meta = new DungeonMonster.Meta();
			meta.id = reader.GetString("monster_id");
			meta.name = reader.GetString("monster_name");
			meta.level = reader.GetInt32("monster_level");
			meta.health = reader.GetFloat("health");
			meta.attack = reader.GetFloat("attack");
			meta.defense = reader.GetFloat("defense");
			meta.speed = reader.GetFloat("speed");
			meta.sprite_path = reader.GetString("sprite_path");
			meta.reward.coin = reader.GetInt32("reward_coin");
			meta.reward.exp = reader.GetInt32("reward_exp");
			metas.Add(meta.id, meta);
		}
	}
	public DungeonMonster.Meta FindMeta(string id)
	{
		if (false == metas.ContainsKey(id))
		{
			throw new System.Exception("can not find monster meta(monster_id:" + id + ")");
		}
		return metas[id];
	}
	
	/*
	public Monster.Info GetRandomMonster(int level) {
		#if UNITY_EDITOR
		Assert.AreNotEqual(0, infoList.Count);
		#endif
		Monster.Info info = infoList[Random.Range(0, infoList.Count)];
		return info;
	}
	*/
}