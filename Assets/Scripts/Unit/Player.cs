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
	public class LevelStatMeta
	{
		public RandomStatMeta health;
	}

	private LevelStatMeta level_stat_meta = new LevelStatMeta()
	{
		health = new RandomStatMeta { type = StatType.Health, min_value = 100.0f, max_value = 200.0f, interval = 10.0f }
	};

	private RandomStatMeta base_health = new RandomStatMeta() { type = StatType.Health, max_value = 2700, min_value = 2300, interval = 50 };
	//private RandomStatMeta base_health = new RandomStatMeta() { type = StatType.Health, max_value = 1, min_value = 10, interval = 1 };
	private RandomStatMeta base_attack = new RandomStatMeta() { type = StatType.Attack, max_value = 1100, min_value = 900, interval = 10 };
	private RandomStatMeta base_defense = new RandomStatMeta() { type = StatType.Defense, max_value = 400, min_value = 600, interval = 5 };
	private RandomStatMeta base_speed = new RandomStatMeta() { type = StatType.Speed, max_value = 150, min_value = 50, interval = 5 };
	private RandomStatMeta base_critical = new RandomStatMeta() { type = StatType.Critical, max_value = 1.0f, min_value = 5.0f, interval = 0.1f };

	public Dictionary<Tuple<EquipItem.Part, int>, EquipItem> equip_items;
	public Inventory inventory;
	public int level;
	public int exp;
	public int coin = 0;
	public int start_item_count = 10;

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
		//if(data exists) {
		//	LoadEquipItem();
		//}
		//else 
		{
			stats.AddStat(base_health.CreateInstance());
			stats.AddStat(base_attack.CreateInstance());
			stats.AddStat(base_defense.CreateInstance());
			stats.AddStat(base_speed.CreateInstance());
			stats.AddStat(base_critical.CreateInstance());
			CalculateStat();

			level = 1;
			exp = 0;
			cur_health = max_health;

			for (int i = 0; i < start_item_count; i++)
			{
				EquipItem item = ItemManager.Instance.CreateRandomEquipItem(level);
				inventory.Add(item);
			}
		}
	}

	public void AddExp(int amount)
	{
		exp += amount;
		while (this.exp >= GetMaxExp())
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
}
