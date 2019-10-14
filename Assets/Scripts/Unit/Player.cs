using UnityEngine;
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
	public class Meta
	{
		public Dictionary<StatType, RandomStatMeta> base_stats = new Dictionary<StatType, RandomStatMeta>();
		public Dictionary<StatType, RandomStatMeta> levelup_stats = new Dictionary<StatType, RandomStatMeta>();

		public void Init()
		{
			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT " + 
					"stat_type," +
					"base_min_value,base_max_value,base_interval," +
					"levelup_min_value,levelup_max_value,levelup_interval " +
				"FROM meta_player_stat"
			);
			while (true == reader.Read())
			{
				StatType statType = (StatType)reader.GetInt32("stat_type");
				base_stats.Add(statType, new RandomStatMeta() { type = statType, min_value = reader.GetFloat("base_min_value"), max_value = reader.GetFloat("base_max_value"), interval = reader.GetFloat("base_interval") });
				levelup_stats.Add(statType, new RandomStatMeta() { type = statType, min_value = reader.GetFloat("levelup_min_value"), max_value = reader.GetFloat("levelup_max_value"), interval = reader.GetFloat("levelup_interval") });
			}
		}
	}
	

	public Dictionary<Tuple<EquipItem.Part, int>, EquipItem> equip_items;
	public Inventory inventory;
	public int level;
	public int exp;
	public int coin = 0;
	public int start_item_count = 1;
	public Meta meta = new Meta();

	public override void Init()
	{
		base.Init();

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

		if (null != item.skill)
		{
			AddSkill(item.skill);
		}
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

		if (null != item.skill)
		{
			RemoveSkill(item.skill);
		}
		Debug.Log("unequip item(item_id:" + item.meta.id + ", part:" + item.part + ", equip_index:" + equip_index + ")");
        Util.EventSystem.Publish<ItemEquipEvent>(EventID.Item_Unequip, new ItemEquipEvent() { item = item, equip_index = equip_index });
        return item;
	}

	public void Load()
	{
		Debug.Log("data path:" + Application.persistentDataPath);

		AchieveManager.Instance.Init();
		//else 
		{
			stats.AddStat(meta.base_stats[StatType.Health].CreateInstance());
			stats.AddStat(meta.base_stats[StatType.Attack].CreateInstance());
			stats.AddStat(meta.base_stats[StatType.Defense].CreateInstance());
			stats.AddStat(meta.base_stats[StatType.Speed].CreateInstance());
			stats.AddStat(meta.base_stats[StatType.Critical].CreateInstance());

			CalculateStat();

			level = 1;
			exp = 0;
			cur_health = max_health;
		}
		for (int i = 0; i < start_item_count; i++)
		{
			EquipItem item = ItemManager.Instance.CreateRandomEquipItem();
			Equip(item);
		}
	}

	public void AddExp(int amount)
	{
		exp += amount;
		while (this.exp >= GetMaxExp())
		{
			exp -= GetMaxExp();
			level += 1;

			stats.AddStat(meta.levelup_stats[StatType.Health].CreateInstance());
			stats.AddStat(meta.levelup_stats[StatType.Attack].CreateInstance());
			stats.AddStat(meta.levelup_stats[StatType.Defense].CreateInstance());
			stats.AddStat(meta.levelup_stats[StatType.Speed].CreateInstance());
			stats.AddStat(meta.levelup_stats[StatType.Critical].CreateInstance());
			CalculateStat();
			cur_health = max_health;
			//Util.EventSystem.Publish<AddExpEvent>(EventID.Player_Add_Exp, 
		}
	}

	public int GetMaxExp()
	{
		return GetMaxExp(level);
	}

	public int GetMaxExp(int level)
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

	public override void CalculateStat()
	{
		base.CalculateStat();
		start_item_count = (int)stats.GetStat(StatType.StartItemCount) + 1;
		float weight = 0.0f;
		foreach (var itr in equip_items)
		{
			if (null != itr.Value)
			{
				EquipItem.Meta meta = (EquipItem.Meta)itr.Value.meta;
				weight += meta.weight;
			}
		}
		speed = Mathf.Round(speed * 100 / (100 + weight));
		speed = Stat.Truncate(speed, 0.01f);
	}
}
