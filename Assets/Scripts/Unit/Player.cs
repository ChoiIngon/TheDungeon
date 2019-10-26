using UnityEngine;
using System.Collections.Generic;

public class ItemEquipEvent
{
	public EquipItem prev_item;
	public EquipItem curr_item;
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
	public int max_stamina;
	public int cur_stamina;
	private int _coin = 0;
	public int coin
	{
		get { return _coin; }
	}
	
	public int start_item_count = 1;
	public Meta meta = new Meta();

	public int enemy_slain_count;
	public int collect_item_count;
	public int collect_coin_count;
	public int open_box_count;
	public int move_count;
	public int total_exp;
	public int start_time;

	public override void Init()
	{
		base.Init();

		enemy_slain_count = 0;
		collect_item_count = 0;
		collect_coin_count = 0; 
		open_box_count = 0;
		move_count = 0;
		total_exp = 0;
		start_time = 0;

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
        
        Util.EventSystem.Publish<ItemEquipEvent>(EventID.Item_Equip, new ItemEquipEvent() { prev_item = prev, curr_item = item, equip_index = equip_index } );
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
		
        Util.EventSystem.Publish<ItemEquipEvent>(EventID.Item_Unequip, new ItemEquipEvent() { curr_item = item, equip_index = equip_index });
        return item;
	}

	public void Load()
	{
		Debug.Log("data path:" + Application.persistentDataPath);
		AchieveManager.Instance.Init();
		LoadPlayer();
		//LoadEquipItem();
		//LoadExpendableItem();
		LoadStats();
		for (int i = 0; i < start_item_count; i++)
		{
			EquipItem item = ItemManager.Instance.CreateRandomEquipItem();
			Equip(item);
		}
		CalculateStat();
		cur_health = max_health;
		cur_stamina = max_stamina;
	}
	private void LoadPlayer()
	{
		level = 1;
		exp = 0;
		cur_health = 0;

		Database.Execute(Database.Type.UserData,
			"CREATE TABLE IF NOT EXISTS user_data (" +
				"player_coin INTEGER NOT NULL DEFAULT 0," +
				"total_play_time INTEGER NOT NULL DEFAULT 0" +
			")"
		);

		Util.Database.DataReader reader = Database.Execute(Database.Type.UserData,
			"SELECT player_coin, total_play_time FROM user_data"
		);

		int rowCount = 0;
		while (true == reader.Read())
		{
			rowCount++;
			_coin = reader.GetInt32("player_coin");
			Util.EventSystem.Publish(EventID.CoinAmountChanged);
		}

		if (0 == rowCount)
		{
			Database.Execute(Database.Type.UserData,
				"INSERT INTO user_data(player_coin, total_play_time) VALUES (0, 0)"
			);
			level = 1;
			exp = 0;
			cur_health = 0;
			_coin = 0;
		}
	}
	private void LoadStats()
	{
		stats.AddStat(meta.base_stats[StatType.Health].CreateInstance());
		stats.AddStat(meta.base_stats[StatType.Attack].CreateInstance());
		stats.AddStat(meta.base_stats[StatType.Defense].CreateInstance());
		stats.AddStat(meta.base_stats[StatType.Speed].CreateInstance());
		stats.AddStat(meta.base_stats[StatType.Critical].CreateInstance());
		stats.AddStat(new Stat.Data() { type = StatType.Stamina, value = 100.0f });
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
			ProgressManager.Instance.Update(ProgressType.PlayerLevel, "", level);
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

		max_stamina = (int)(stats.GetStat(StatType.Stamina) + (stats.GetStat(StatType.Stamina) * stats.GetStat(StatType.Stamina_Rate) / 100));
		Util.EventSystem.Publish(EventID.Player_Stat_Change);
	}

	public void ChangeCoin(int amount, bool notifyEvent = true)
	{
		if (0 == amount)
		{
			return;
		}

		if (0 > amount)
		{
			if (coin < amount)
			{
				throw new System.Exception("not enough coin");
			}
		}
		else
		{
			ProgressManager.Instance.Update(ProgressType.CollectCoin, "", amount);
		}
		_coin += amount;
		if (true == notifyEvent)
		{
			Util.EventSystem.Publish(EventID.CoinAmountChanged);
		}
	}
}
