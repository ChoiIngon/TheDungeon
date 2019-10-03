using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ItemManager : Util.Singleton<ItemManager>
{
	private Dictionary<string, Item.Meta> metas = new Dictionary<string, Item.Meta>();
	private List<EquipItem.Meta> equip_item_metas = new List<EquipItem.Meta>();
	private List<Skill.Meta> skill_metas = new List<Skill.Meta>();
	private Util.WeightRandom<Item.Grade> grade_gacha = new Util.WeightRandom<Item.Grade>();
	private Util.WeightRandom<EquipItemStatMeta> stat_type_gacha = new Util.WeightRandom<EquipItemStatMeta>();

	public void Init()
	{
		metas = new Dictionary<string, Item.Meta>();

		grade_gacha.SetWeight(Item.Grade.Low,		6);
		grade_gacha.SetWeight(Item.Grade.Normal,	5);
		grade_gacha.SetWeight(Item.Grade.High,		4);
		grade_gacha.SetWeight(Item.Grade.Magic,		3);
		grade_gacha.SetWeight(Item.Grade.Rare,		2);
		grade_gacha.SetWeight(Item.Grade.Legendary, 1);

		stat_type_gacha.SetWeight(new EquipItemStatMeta()
		{
			type = StatType.CoinBonus,
			base_value = 0.05f,
			rand_stat_meta = new RandomStatMeta()
			{
				type = StatType.CoinBonus,
				min_value = 0.0f,
				max_value = 0.1f,
				interval = 0.01f
			}
		}, 1);

		stat_type_gacha.SetWeight(new EquipItemStatMeta()
		{
			type = StatType.ExpBonus,
			base_value = 0.05f,
			rand_stat_meta = new RandomStatMeta()
			{
				type = StatType.ExpBonus,
				min_value = 0.0f,
				max_value = 0.1f,
				interval = 0.01f
			}
		}, 1);

		skill_metas.Add(new Skill_Stun.Meta() { skill_id = "SKILL_STUN", description = "20%의 확률로 5턴 동안 대상을 기절 시킴" });
		InitEquipItem();
		InitPotionItemInfo();
		InitKeyItemInfo();
	}

	public Item CreateItem(string id)
	{
		Item item = FindMeta<Item.Meta>(id).CreateInstance();
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

		if (0 < skill_metas.Count)
		{
			if (EquipItem.Grade.Rare <= item.grade)
			{
				item.skill = skill_metas[skill_metas.Count - 1].CreateInstance();
			}
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

	public T FindMeta<T>(string id) where T : Item.Meta
	{
		if (false == metas.ContainsKey(id))
		{
			throw new System.Exception("can't find item meta(id:" + id + ")");
		}
		return metas[id] as T;
	}

	
	private void InitEquipItem()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT item_id, item_name, equip_part, price, sprite_path, description, main_stat_type, base_value, rand_value FROM meta_item_equip");
		while(true == reader.Read())
		{
			EquipItem.Meta meta = new EquipItem.Meta();
            meta.id = reader.GetString("item_id");
            meta.name = reader.GetString("item_name");
            meta.type = Item.Type.Equipment;
			meta.main_stat = new EquipItemStatMeta()
			{
				type = (StatType)reader.GetInt32("main_stat_type"),
				base_value = reader.GetFloat("base_value"),
				rand_stat_meta = new RandomStatMeta()
				{
					type = (StatType)reader.GetInt32("main_stat_type"),
					min_value = 0,
					max_value = reader.GetFloat("rand_value"),
					interval = 0.01f
				}
			};
			meta.part = (EquipItem.Part)reader.GetInt32("equip_part");
			meta.price = reader.GetInt32("price");
            meta.sprite_path = reader.GetString("sprite_path");
            meta.description = reader.GetString("description");
            equip_item_metas.Add(meta);
		}
	}

	private void InitPotionItemInfo()
	{
		{
			HealPotionItem.Meta meta = new HealPotionItem.Meta ();
			meta.potion_type = PotionItem.PotionType.Heal;
			meta.type = Item.Type.Potion;
			meta.id = "ITEM_POTION_HEALING";
			meta.name = "Healing Potion";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_002";
			meta.description = "An elixir that will instantly return you to full health and cure poison.";
			metas.Add (meta.id, meta);
		}

		{
			StranthPotionItem.Meta meta = new StranthPotionItem.Meta();
			meta.potion_type = PotionItem.PotionType.Strength;
			meta.type = Item.Type.Potion;
			meta.id = "ITEM_POTION_STRENGTH";
			meta.name = "Strength Potion";
			meta.price = 100;
			meta.sprite_path = "Item/item_potion_001";
			meta.description = "An elixir that will permenently increase strength.";
			metas.Add(meta.id, meta);
		}
	}

	private void InitKeyItemInfo()
	{
		{
			KeyItem.Meta meta = new KeyItem.Meta();
			meta.type = Item.Type.Key;
			meta.id = "ITEM_KEY";
			meta.name = "Key";
			meta.price = 0;
			meta.sprite_path = "Item/item_key";
			meta.description = "Very important for going to next dungeon levell";
			metas.Add(meta.id, meta);
		}
	}

	private Stat.Data CreateStat(int level, EquipItemStatMeta meta)
	{
		Stat.Data data = new Stat.Data();
		data.type = meta.type;
		data.value = meta.base_value;
		for (int i = 0; i < level; i++)
		{
			data.value += meta.rand_stat_meta.value;
		}
		data.value = Mathf.Round(data.value * 100) / 100.0f;
		return data;
	}
}