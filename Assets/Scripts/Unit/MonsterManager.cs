using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class MonsterManager : Util.Singleton<MonsterManager>
{
	public void Init()
	{
	}

	public Monster.Meta FindMeta(string id)
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			Query() + " WHERE monster_id = '" + id + "'"
		);

		while (true == reader.Read())
		{
			return CreateMonsterMeta(reader);
		}
		throw new System.Exception("can not find monster meta(monster_id:" + id + ")");
	}

	public Monster.Meta GetRandomMonster(int level)
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			Query() + " WHERE monster_level <= " + level + " ORDER BY random() LIMIT 1"
		);

		while (true == reader.Read())
		{
			return CreateMonsterMeta(reader);
		}
		throw new System.Exception("can not find monster meta(monster_level:" + level + ")");
	}

	private string Query()
	{
		return "SELECT monster_id, monster_name, monster_level, health, attack, defense, speed, sprite_path, reward_coin, reward_exp, reward_item_chance FROM meta_monster";
	}

	private Monster.Meta CreateMonsterMeta(Util.Database.DataReader reader)
	{
		Monster.Meta meta = new Monster.Meta();
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
		meta.reward.item_chance = reader.GetInt32("reward_item_chance");
		return meta;
	}
}