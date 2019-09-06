using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
public class ItemManager : Util.Singleton<ItemManager>
{
	private Dictionary<string, Item.Meta> metas = new Dictionary<string, Item.Meta>();
	private List<EquipItem.Meta> equip_item_metas = new List<EquipItem.Meta>();
	private Util.WeightRandom<Item.Grade> grade_gacha = new Util.WeightRandom<Item.Grade>();
	private Util.WeightRandom<EquipItemStatMeta> stat_type_gacha = new Util.WeightRandom<EquipItemStatMeta>();

	public void Init()
	{
		metas = new Dictionary<string, Item.Meta>();

		grade_gacha.SetWeight(Item.Grade.Low, 300);
		grade_gacha.SetWeight(Item.Grade.Normal, 200);
		grade_gacha.SetWeight(Item.Grade.High, 150);
		grade_gacha.SetWeight(Item.Grade.Magic, 125);
		grade_gacha.SetWeight(Item.Grade.Rare, 100);
		grade_gacha.SetWeight(Item.Grade.Legendary, 030);

		stat_type_gacha.SetWeight(new EquipItemStatMeta() { type = StatType.CoinBonus, base_value = 0.05f, rand_value = 0.001f }, 1);
		stat_type_gacha.SetWeight(new EquipItemStatMeta() { type = StatType.ExpBonus, base_value = 0.05f, rand_value = 0.001f }, 1);

		InitEquipItem();
	}

	public Item CreateItem(string id)
	{
		Item item = FindInfo<Item.Meta>(id).CreateInstance();
		Analytics.CustomEvent("CreateItem", new Dictionary<string, object>
		{
			{"id", item.meta.id },
			{"type", item.meta.type.ToString()},
			{"name", item.meta.name },
			{"grade", item.grade.ToString()}
		});
		return item;
	}

	public EquipItem CreateRandomEquipItem(int level)
	{
		EquipItem.Meta meta = equip_item_metas[Random.Range(0, equip_item_metas.Count)];
		EquipItem item = meta.CreateInstance() as EquipItem;
		item.grade = grade_gacha.Random();
		item.level = level;
		item.main_stat.AddStat(CreateStat(level, meta.main_stat));
		for (int i = 0; i < (int)item.grade - (int)EquipItem.Grade.Normal; i++)
		{
			item.sub_stat.AddStat(CreateStat(level, stat_type_gacha.Random()));
		}

		Analytics.CustomEvent("CreateItem", new Dictionary<string, object>
		{
			{"id", item.meta.id },
			{"type", item.meta.type.ToString()},
			{"name", item.meta.name },
			{"grade", item.grade.ToString()},
			{"level", item.level}
		});
		return item;
	}

	public T FindInfo<T>(string id) where T : Item.Meta
	{
		if (false == metas.ContainsKey(id))
		{
			throw new System.Exception("can't find item meta(id:" + id + ")");
		}
		return metas[id] as T;
	}

	
	/*
	private void InitPotionItemInfo()
	{
		{
			HealingPotionItem.Info info = new HealingPotionItem.Info ();
			info.id = "ITEM_POTION_HEALING";
			info.name = "Healing Potion";
			info.price = 100;
			info.icon = "item_potion_002";
			info.grade = (int)Item.Grade.Normal;
			info.description = "An elixir that will instantly return you to full health and cure poison.";
			infos.Add (info.id, info);
		}
		{
			PoisonPotionItem.Info info = new PoisonPotionItem.Info ();
			info.id = "ITEM_POTION_POISON";
			info.name = "Poison Potion";
			info.price = 100;
			info.icon = "item_potion_003";
			info.grade = (int)Item.Grade.Normal;
			infos.Add (info.id, info);
		}
	}

	
	*/
	void InitEquipItem()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT item_id, item_name, equip_part, price, description, main_stat_type, base_value, rand_value FROM meta_item_equip");
		while(true == reader.Read())
		{
			EquipItem.Meta meta = new EquipItem.Meta();
			meta.description = reader.GetString("description");
			meta.id = reader.GetString("item_id");
			meta.main_stat = new EquipItemStatMeta() { type = (StatType)reader.GetInt32("main_stat_type"), base_value = reader.GetFloat("base_value"), rand_value = reader.GetFloat("rand_value") };
			meta.name = reader.GetString("item_name");
			meta.part = (EquipItem.Part)reader.GetInt32("equip_part");
			meta.price = reader.GetInt32("price");
			meta.type = Item.Type.Equipment;
			equip_item_metas.Add(meta);
		}
	}

	private Stat.Data CreateStat(int level, EquipItemStatMeta meta)
	{
		Stat.Data data = new Stat.Data();
		data.type = meta.type;
		data.value = meta.base_value;
		for (int i = 0; i < level; i++)
		{
			data.value += Random.Range(0, meta.rand_value);
		}
		data.value = Mathf.Round(data.value * 100) / 100.0f;
		return data;
	}
}