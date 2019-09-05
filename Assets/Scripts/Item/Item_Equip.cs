using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipItem : Item {
	public enum Part {
		Helmet,
		Hand,
		Armor,
		Ring,
		Shoes
	}

	[System.Serializable]
	public new class Meta : Item.Meta
	{
		public Part part;
		public override Item CreateInstance()
		{
			return new EquipItem(this);
		}
	}

	public class Stat
	{
		public enum StatType
		{
			Invalid,
			AddAttackValue,
			AddAttackRate,
			AddDefenseValue,
			AddDefenseRate,
			AddCriticalChance,
			AddCriticalDamageRate,
		}
		public StatType type;
		public float value;
	}

	public class Encant {
	}

	public Part part;
	public int 	level;
	public List<Stat> main_stats;
	public List<Stat> sub_stats;
	public EquipItem(EquipItem.Meta meta) : base(meta)
	{
		part = meta.part;
	}

	/*
	
	public EquipmentItemStat mainStat;
	public List<EquipmentItemStat> subStats = new List<EquipmentItemStat> ();
	public ItemEnchantmemt enchantment;

	
	public override List<string> Actions() {
		List<string> actions = base.Actions ();
		actions.Add ("EQUIP");
		actions.Add ("UNEQUIP");
		return actions;
	}

	public Unit.Stat GetStat(Unit.Stat stat)
	{
        Unit.Stat result = new Unit.Stat ();
		result += mainStat.GetStat (stat);
		for (int i = 0; i < subStats.Count; i++) {
			result += subStats [i].GetStat (stat);
		}
		return result;
	}

	*/
}