﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class EquipItemStatMeta
{
	public StatType type;
	public float base_value;
	public float max_value;
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
		public EquipItemStatMeta main_stat = null;
		public List<EquipItemStatMeta> sub_stats = new List<EquipItemStatMeta>();
		public Skill.Meta main_skill_meta = null;
		public List<Skill.Meta> rand_skill_metas = new List<Skill.Meta>();

		public override Item CreateInstance()
		{
			EquipItem item = new EquipItem(this);
			item.grade = EquipItemManager.Instance.grade_gacha.Random();
			item.level = Mathf.Max(1, UnityEngine.Random.Range(GameManager.Instance.player.level - 5, GameManager.Instance.player.level + 2));
			item.main_stat.AddStat(item.CreateStat(main_stat));
			if (0 < sub_stats.Count)
			{
				for (int i = 0; i < (int)item.grade - (int)EquipItem.Grade.Normal; i++)
				{
					item.sub_stat.AddStat(item.CreateStat(sub_stats[UnityEngine.Random.Range(0, sub_stats.Count)]));
				}
			}

			if (null != main_skill_meta)
			{
				item.skills.Add(main_skill_meta.CreateInstance());
			}
			if (EquipItem.Grade.Rare <= item.grade && 0 < rand_skill_metas.Count)
			{
				Skill.Meta skillMeta = rand_skill_metas[UnityEngine.Random.Range(0, rand_skill_metas.Count)];
				if (main_skill_meta != skillMeta)
				{
					item.skills.Add(rand_skill_metas[UnityEngine.Random.Range(0, rand_skill_metas.Count)].CreateInstance());
				}
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
	public List<Skill> skills = new List<Skill>();

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

	private void InitItemMeta()
	{
		try
		{
			GoogleSheetReader sheetReader = new GoogleSheetReader(GameManager.GOOGLESHEET_ID, GameManager.GOOGLESHEET_API_KEY);
			if (false == sheetReader.Load("meta_item_equip"))
			{

			}

			foreach (GoogleSheetReader.Row row in sheetReader)
			{
				EquipItem.Meta meta = new EquipItem.Meta();
				meta.id = row["item_id"];
				meta.name = row["item_name"];
				meta.part = (EquipItem.Part)Enum.Parse(typeof(EquipItem.Part), row["equip_part"]);
				meta.price = Int32.Parse(row["price"]);
				meta.weight = float.Parse(row["weight"]);
				meta.type = Item.Type.Equipment;
				meta.main_stat = new EquipItemStatMeta()
				{
					type = (StatType)Enum.Parse(typeof(StatType), row["main_stat_type"]),
					base_value = float.Parse(row["main_base_value"]),
					max_value = 0.0f,
					rand_stat_meta = new RandomStatMeta()
					{
						type = (StatType)Enum.Parse(typeof(StatType), row["main_stat_type"]),
						min_value = 0,
						max_value = float.Parse(row["main_rand_value"]),
						interval = 0.01f
					}
				};
				meta.main_skill_meta = SkillManager.Instance.FindMeta<Skill.Meta>(row["skill_id"]);
				meta.sprite_path = row["sprite_path"];
				meta.description = row["description"];
				item_metas.Add(meta);
				ItemManager.Instance.AddItemMeta(meta);
			}
		}
		catch (System.Net.WebException e)
		{
			GameManager.Instance.ui_textbox.on_close = () =>
			{
				Application.Quit();
			};
			GameManager.Instance.ui_textbox.AsyncWrite(e.Message, false);
		}
	}
	private void InitSubStatMeta()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT item_id, stat_type, base_value, max_value, rand_min_value, rand_max_value, interval FROM meta_item_equip_substat"
		);
		while (true == reader.Read())
		{
			EquipItem.Meta meta = ItemManager.Instance.FindMeta<EquipItem.Meta>(reader.GetString("item_id"));
			if (null == meta)
			{
				throw new System.Exception("can't find item meta(id:" + reader.GetString("item_id") + ")");
			}
			meta.sub_stats.Add(new EquipItemStatMeta()
			{
				type = (StatType)reader.GetInt32("stat_type"),
				base_value = reader.GetFloat("base_value"),
				max_value = reader.GetFloat("max_value"),
				rand_stat_meta = new RandomStatMeta()
				{
					type = (StatType)reader.GetInt32("stat_type"),
					min_value = reader.GetFloat("rand_min_value"),
					max_value = reader.GetFloat("rand_max_value"),
					interval = reader.GetFloat("interval"),
				}
			});
		}
	}
	private void InitSkillMeta()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT item_id, skill_name, skill_id, cooltime FROM meta_item_equip_skill"
		);
		while (true == reader.Read())
		{
			EquipItem.Meta meta = ItemManager.Instance.FindMeta<EquipItem.Meta>(reader.GetString("item_id"));
			if (null == meta)
			{
				throw new System.Exception("can't find item meta(id:" + reader.GetString("item_id") + ")");
			}
			meta.rand_skill_metas.Add(SkillManager.Instance.FindMeta<Skill.Meta>(reader.GetString("skill_id")));
		}
	}
	public void Init()
	{
		grade_gacha.SetWeight(Item.Grade.Low, 20);
		grade_gacha.SetWeight(Item.Grade.Normal, 18);
		grade_gacha.SetWeight(Item.Grade.High, 16);
		grade_gacha.SetWeight(Item.Grade.Magic, 14);
		grade_gacha.SetWeight(Item.Grade.Rare, 12);
		grade_gacha.SetWeight(Item.Grade.Legendary, 10);

		InitItemMeta();
		InitSubStatMeta();
		InitSkillMeta();
		//InitStatGacha();
	}

	public EquipItem CreateRandomItem()
	{
		EquipItem.Meta meta = GetRandomMeta();
		return meta.CreateInstance() as EquipItem;
	}

	public EquipItem.Meta GetRandomMeta()
	{
		return item_metas[UnityEngine.Random.Range(0, item_metas.Count)];
	}
}