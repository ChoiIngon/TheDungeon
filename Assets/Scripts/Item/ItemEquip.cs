using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		Shoes
	}

	[System.Serializable]
	public new class Meta : Item.Meta
	{
		public Part part = Part.Invalid;
		public EquipItemStatMeta main_stat = new EquipItemStatMeta();

		public override Item CreateInstance()
		{
			return new EquipItem(this);
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

    public override string description
    {
		get
		{
			string desc = "";
            desc += meta.description + "\n\n";

			foreach (Stat.Data stat in main_stat.GetStats())
			{
				Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT stat_name, description FROM meta_stat where stat_type=" + (int)stat.type);
				while (true == reader.Read())
				{
					desc += " -" + string.Format(reader.GetString("description"), stat.value) + "\n";
				}
			}

			foreach (Stat.Data stat in sub_stat.GetStats())
			{
				Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT stat_name, description FROM meta_stat where stat_type=" + (int)stat.type);
				while (true == reader.Read())
				{
					desc += "<color=green> -" + string.Format(reader.GetString("description"), stat.value) + "</color>\n";
				}
			}

			if (null != skill)
			{
				desc += "<color=red> -" + skill.meta.description + "</color>";
			}
			return desc;
		}
    }
}