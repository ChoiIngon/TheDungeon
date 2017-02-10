using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentItem : Item {
	public enum Part {
		Helmet,
		Hand,
		Armor,
		Ring,
		Shoes
	}

	public int 	level;
	public Part	part;
	public EquipmentItemStat mainStat;
	public List<EquipmentItemStat> subStats = new List<EquipmentItemStat> ();
	public ItemEnchantmemt enchantment;

	public EquipmentItem() : base(Item.Type.Equipment){}
	public override List<string> Actions() {
		List<string> actions = base.Actions ();
		actions.Add ("EQUIP");
		actions.Add ("UNEQUIP");
		return actions;
	}

	public Player.Stat GetStat(Player.Stat stat)
	{
		Player.Stat result = new Player.Stat ();
		result += mainStat.GetStat (stat);
		for (int i = 0; i < subStats.Count; i++) {
			result += subStats [i].GetStat (stat);
		}
		return result;
	}

	[System.Serializable]
	public new class Info : Item.Info
	{
		public int 	part;
		public EquipmentItemStat.Info main_stat;
		public override Item CreateInstance()
		{
			EquipmentItem item = new EquipmentItem ();
			item.id = id;
			item.name = name;
			item.icon = ResourceManager.Instance.Load<Sprite> (icon);
			item.price = price;
			item.part = (EquipmentItem.Part)part;
			item.description = description;
			return item;
		}
	}
}