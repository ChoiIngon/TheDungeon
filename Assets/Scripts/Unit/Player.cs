﻿using UnityEngine;
using System.Collections.Generic;

public class ItemEquipEvent
{
    public EquipItem item;
    public int equip_index;
}

public class AddExpEvent
{
	public int prev_level;
	public int curr_level;
	public int exp;
}

public class Player : Unit
{
	public class LevelStatMeta
	{
		public RandomStatMeta health;
	}
	public Dictionary<Tuple<EquipItem.Part, int>, EquipItem> equip_items;
	public Inventory inventory;
	public int level;
	public int exp;
	public LevelStatMeta level_stat_meta = new LevelStatMeta();

	public void Init()
	{
		equip_items = new Dictionary<Tuple<EquipItem.Part, int>, EquipItem>();
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Helmet, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 1), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Armor, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 1), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Shoes, 0), null);
		inventory = new Inventory();

		Load();

		stats.AddStat(new Stat.Data() { type = StatType.Health, value = 100.0f });
		stats.AddStat(new Stat.Data() { type = StatType.Attack, value = 60.0f });
		stats.AddStat(new Stat.Data() { type = StatType.Defense, value = 400.0f });
		stats.AddStat(new Stat.Data() { type = StatType.Speed, value = 150.0f });
		CalculateStat();
		cur_health = stats.GetStat(StatType.Health);
	}
    
	public EquipItem Equip(EquipItem item, int equip_index = 0)
	{
		if (null == item)
		{
			return null;
		}

        EquipItem prev = Unequip(item.part, equip_index);
		equip_items[new Tuple<EquipItem.Part, int>(item.part, equip_index)] = item;
        item.equip_index = equip_index;
		stats += item.main_stat;
		stats += item.sub_stat;

		CalculateStat();

        Debug.Log("equip item(item_id:" + item.meta.id + ", part:" + item.part + ", equip_index:" + equip_index + ")");
        Util.EventSystem.Publish<ItemEquipEvent>(EventID.Item_Equip, new ItemEquipEvent() { item = item, equip_index = equip_index } );
        return prev;
	}

	public EquipItem Unequip(EquipItem.Part part, int equip_index)
	{
		EquipItem item = equip_items[new Tuple<EquipItem.Part, int>(part, equip_index)];
        if (null == item)
        {
            return null;
        }

        item.equip_index = -1;
		equip_items[new Tuple<EquipItem.Part, int>(part, equip_index)] = null;

		stats -= item.main_stat;
		stats -= item.sub_stat;

		CalculateStat();
	
        Debug.Log("unequip item(item_id:" + item.meta.id + ", part:" + item.part + ", equip_index:" + equip_index + ")");
        Util.EventSystem.Publish<ItemEquipEvent>(EventID.Item_Unequip, new ItemEquipEvent() { item = item, equip_index = equip_index });
        return item;
	}

	public void Load()
	{
		Debug.Log("data path:" + Application.persistentDataPath);
		LoadEquipItem();        

		level_stat_meta.health = new RandomStatMeta { type = StatType.Health, min_value = 100.0f, max_value = 200.0f, interval = 10.0f };
    }

	public void AddExp(int amount)
	{
		exp += amount;
		while (this.exp > GetMaxExp())
		{
			exp -= GetMaxExp();
			level += 1;
			max_health += level_stat_meta.health.value;
			cur_health = max_health;
			//Util.EventSystem.Publish<AddExpEvent>(EventID.Player_Add_Exp, 
		}
	}

	public int GetMaxExp()
	{
		return (int)Mathf.Pow(level, 1.8f);
	}
	/*
	public EquipmentItem GetEquipment(EquipmentItem.Part category, int index) {
        if(equipments.ContainsKey(new Tuple<EquipmentItem.Part, int>(category, index)))
        {
            return null;
        }
		return equipments [new Tuple<EquipmentItem.Part, int> (category, index)];
	}
	*/

	private void LoadEquipItem()
	{
		string sub_stat_column = "";
		for (int i = 0; i < EquipItem.MAX_SUB_STAT_COUNT; i++)
		{
			sub_stat_column += "sub_stat_type_" + (i + 1) + " INTEGER NOT NULL DEFAULT 0," +
				"sub_stat_value_" + (i + 1) + " REAL NOT NULL DEFAULT 0,";
		}
		Database.Execute(Database.Type.UserData,
		   "CREATE TABLE IF NOT EXISTS user_item_equip (" +
			   "item_seq INTEGER NOT NULL," +
			   "item_id TEXT NOT NULL," +
			   "equip_index INTEGER NOT NULL," +
			   "main_stat_type INTEGER NOT NULL," +
			   "main_stat_value REAL NOT NULL," +
			   sub_stat_column +
			   "PRIMARY KEY('item_seq')" +
		   ")"
	   );

		sub_stat_column = "";
		for (int i = 0; i < EquipItem.MAX_SUB_STAT_COUNT; i++)
		{
			sub_stat_column += "sub_stat_type_" + (i + 1) + ", sub_stat_value_" + (i + 1) + " ,";
		}
		Util.Database.DataReader reader = Database.Execute(Database.Type.UserData,
			"SELECT item_seq, item_id,  " +
				"main_stat_type, main_stat_value, " +
				sub_stat_column +
				"equip_index " +
			"FROM user_item_equip"
		);
		while (true == reader.Read())
		{
			string itemID = reader.GetString("item_id");
			EquipItem.Meta meta = ItemManager.Instance.FindMeta<EquipItem.Meta>(itemID);
			EquipItem item = meta.CreateInstance() as EquipItem;
			item.item_seq = reader.GetInt32("item_seq");
			item.main_stat.AddStat(new Stat.Data() { type = (StatType)reader.GetInt32("main_stat_type"), value = reader.GetFloat("main_stat_value") });

			for (int i = 0; i < EquipItem.MAX_SUB_STAT_COUNT; i++)
			{
				if (0 == reader.GetInt32("sub_stat_type_" + (i + 1)))
				{
					continue;
				}
				item.sub_stat.AddStat(new Stat.Data() { type = (StatType)reader.GetInt32("sub_stat_type" + (i + 1)), value = reader.GetFloat("sub_stat_value_" + (i + 1)) });
			}
		}
	}
}
