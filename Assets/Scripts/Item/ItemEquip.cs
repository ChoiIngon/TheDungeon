using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class EquipItemStatMeta
{
	public StatType type;
	public float base_value;
	public RandomStatMeta rand_stat_meta;
}

public class EquipItem : Item
{
    public const int MAX_SUB_STAT_COUNT = (int)Item.Grade.Legendary - (int)Item.Grade.Normal;
	public enum Part
	{
		Invalid,
		Helmet,
		Hand,
		Armor,
		Ring,
		Shoes,
		Max
	}

	public new class Meta : Item.Meta
	{
		public float weight;
		public Part part = Part.Invalid;
		public EquipItemStatMeta main_stat = new EquipItemStatMeta();
		
		public override Item CreateInstance()
		{
			EquipItem item = new EquipItem(this);
			item.grade = EquipItemManager.Instance.grade_gacha.Random();
			item.level = Mathf.Max(1, Random.Range(GameManager.Instance.player.level - 5, GameManager.Instance.player.level + 2));
			item.main_stat.AddStat(item.CreateStat((item.meta as Meta).main_stat));
			for (int i = 0; i < (int)item.grade - (int)EquipItem.Grade.Normal; i++)
			{
				item.sub_stat.AddStat(item.CreateStat(EquipItemManager.Instance.sub_stat_gacha[(int)item.part].Random()));
			}

			if (EquipItem.Grade.Rare <= item.grade)
			{
				item.skill = SkillManager.Instance.CreateRandomInstance();
			}

			Analytics.CustomEvent("CreateItem", new Dictionary<string, object>
			{
				{ "id", item.meta.id },
				{ "type", item.meta.type.ToString()},
				{ "name", item.meta.name },
				{ "grade", item.grade.ToString()},
				{ "level", item.level}
			});

			return item;
		}
	}

	public int 	level = 0;
	public Part part = Part.Invalid;
	
	public bool equip
    {
        get {
            return equip_index != -1;
        }
    }
    public int  equip_index = -1;
	public Stat main_stat = new Stat();
	public Stat sub_stat = new Stat();
	public Skill skill;

	public EquipItem(EquipItem.Meta meta) : base(meta)
	{
		part = meta.part;
	}

	private Stat.Data CreateStat(EquipItemStatMeta meta)
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

public class EquipItemManager : Util.Singleton<EquipItemManager>
{
	public List<EquipItem.Meta> item_metas = new List<EquipItem.Meta>();
	public Util.WeightRandom<Item.Grade> grade_gacha = new Util.WeightRandom<Item.Grade>();
	public Util.WeightRandom<EquipItemStatMeta>[] sub_stat_gacha = new Util.WeightRandom<EquipItemStatMeta>[(int)EquipItem.Part.Max];

	public void Init()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT item_id, item_name, equip_part, price, weight, main_stat_type, main_base_value, main_rand_value, sprite_path, description FROM meta_item_equip"
		);
		while (true == reader.Read())
		{
			EquipItem.Meta meta = new EquipItem.Meta();
			meta.id = reader.GetString("item_id");
			meta.name = reader.GetString("item_name");
			meta.part = (EquipItem.Part)reader.GetInt32("equip_part");
			meta.price = reader.GetInt32("price");
			meta.weight = reader.GetFloat("weight");
			meta.type = Item.Type.Equipment;
			meta.main_stat = new EquipItemStatMeta()
			{
				type = (StatType)reader.GetInt32("main_stat_type"),
				base_value = reader.GetFloat("main_base_value"),
				rand_stat_meta = new RandomStatMeta()
				{
					type = (StatType)reader.GetInt32("main_stat_type"),
					min_value = 0,
					max_value = reader.GetFloat("main_rand_value"),
					interval = 0.01f
				}
			};
			meta.sprite_path = reader.GetString("sprite_path");
			meta.description = reader.GetString("description");
			item_metas.Add(meta);
			ItemManager.Instance.AddItemMeta(meta);
		}

		grade_gacha.SetWeight(Item.Grade.Low, 6);
		grade_gacha.SetWeight(Item.Grade.Normal, 5);
		grade_gacha.SetWeight(Item.Grade.High, 4);
		grade_gacha.SetWeight(Item.Grade.Magic, 3);
		grade_gacha.SetWeight(Item.Grade.Rare, 2);
		grade_gacha.SetWeight(Item.Grade.Legendary, 1);

		InitStatGacha();
	}

	public EquipItem CreateRandomItem()
	{
		EquipItem.Meta meta = GetRandomMeta();
		return meta.CreateInstance() as EquipItem;
	}

	public EquipItem.Meta GetRandomMeta()
	{
		return item_metas[Random.Range(0, item_metas.Count)];
	}
	private void InitStatGacha()
	{
		for (int i = 0; i < (int)EquipItem.Part.Max; i++)
		{
			sub_stat_gacha[i] = new Util.WeightRandom<EquipItemStatMeta>();
		}

		// StatType.CoinBonus
		{
			EquipItemStatMeta itemStatMeta = new EquipItemStatMeta()
			{
				type = StatType.CoinBonus,
				base_value = 3.0f,
				rand_stat_meta = new RandomStatMeta()
				{
					type = StatType.CoinBonus,
					min_value = 0.0f,
					max_value = 0.5f,
					interval = 0.1f
				}
			};

			sub_stat_gacha[(int)EquipItem.Part.Helmet].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Hand].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Armor].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Ring].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Shoes].SetWeight(itemStatMeta, 1);
		}
		// StatType.ExpBonus 
		{
			EquipItemStatMeta itemStatMeta = new EquipItemStatMeta()
			{
				type = StatType.ExpBonus,
				base_value = 5.0f,
				rand_stat_meta = new RandomStatMeta()
				{
					type = StatType.CoinBonus,
					min_value = 0.0f,
					max_value = 5.0f,
					interval = 0.1f
				}
			};
			sub_stat_gacha[(int)EquipItem.Part.Helmet].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Hand].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Armor].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Ring].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Shoes].SetWeight(itemStatMeta, 1);
		}
		// StatType.Health_Rate
		{
			EquipItemStatMeta itemStatMeta = new EquipItemStatMeta()
			{
				type = StatType.Health_Rate,
				base_value = 1.0f,
				rand_stat_meta = new RandomStatMeta()
				{
					type = StatType.Health_Rate,
					min_value = 0.0f,
					max_value = 0.5f,
					interval = 0.01f
				}
			};
			sub_stat_gacha[(int)EquipItem.Part.Armor].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Ring].SetWeight(itemStatMeta, 1);
		}
		// StatType.Attack_Rate
		{
			EquipItemStatMeta itemStatMeta = new EquipItemStatMeta()
			{
				type = StatType.Attack_Rate,
				base_value = 1.0f,
				rand_stat_meta = new RandomStatMeta()
				{
					type = StatType.Attack_Rate,
					min_value = 0.0f,
					max_value = 0.5f,
					interval = 0.1f
				}
			};
			sub_stat_gacha[(int)EquipItem.Part.Hand].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Ring].SetWeight(itemStatMeta, 1);
		}
		// StatType.Defense_Rate
		{
			EquipItemStatMeta itemStatMeta = new EquipItemStatMeta()
			{
				type = StatType.Defense_Rate,
				base_value = 1.0f,
				rand_stat_meta = new RandomStatMeta()
				{
					type = StatType.Defense_Rate,
					min_value = 0.0f,
					max_value = 0.5f,
					interval = 0.1f
				}
			};
			sub_stat_gacha[(int)EquipItem.Part.Armor].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Ring].SetWeight(itemStatMeta, 1);
		}
		// StatType.Speed_Rate
		  {
			EquipItemStatMeta itemStatMeta = new EquipItemStatMeta()
			{
				type = StatType.Speed_Rate,
				base_value = 1.0f,
				rand_stat_meta = new RandomStatMeta()
				{
					type = StatType.Speed_Rate,
					min_value = 0.0f,
					max_value = 0.5f,
					interval = 0.1f
				}
			};
			sub_stat_gacha[(int)EquipItem.Part.Shoes].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Hand].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Ring].SetWeight(itemStatMeta, 1);
		}
		{
			EquipItemStatMeta itemStatMeta = new EquipItemStatMeta()
			{
				type = StatType.Critical,
				base_value = 1.0f,
				rand_stat_meta = new RandomStatMeta()
				{
					type = StatType.Critical,
					min_value = 0.0f,
					max_value = 0.5f,
					interval = 0.1f
				}
			};
			
			sub_stat_gacha[(int)EquipItem.Part.Hand].SetWeight(itemStatMeta, 1);
			sub_stat_gacha[(int)EquipItem.Part.Ring].SetWeight(itemStatMeta, 1);
		}
	}
}