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

	public Part		part;
	public EquipmentItemStat mainStat;
	public List<EquipmentItemStat> subStats = new List<EquipmentItemStat> ();
	public ItemEnchantmemt enchantment;

	public EquipmentItem() : base(Item.Type.Equipment){}
	public override List<string> Actions() {
		List<string> actions = base.Actions ();
		actions.Add ("EQUIP");
		return actions;
	}

	public Player.Stat GetStat(Player.Stat stat)
	{
		Player.Stat result = new Player.Stat ();
		result += mainStat.GetStat (stat);
		for (int i = 0; i < subStats.Count; i++) {
			/*
			if (null != subStats [i].trigger && false == subStats[i].trigger.IsAvailable(stat)) {
				continue;
			}
			*/
			result += subStats [i].GetStat (stat);
		}
		return result;
	}

	public virtual void Equip(int index)
	{
	}
	public override Item CreateInstance()
	{
		EquipmentItem item = new EquipmentItem ();
		item.id = id;
		item.name = name;
		item.icon = icon;
		item.price = price;
		item.grade = grade;
		item.part = part;
		item.mainStat = mainStat;
		List<EquipmentItemStat> tmp = new List<EquipmentItemStat> (subStats); 
		for (int i = 0; i < ((int)grade - (int)Grade.Normal); i++) {
			if (0 == tmp.Count) {
				break;
			}
			int index = Random.Range (0, tmp.Count);
			item.subStats.Add (tmp [index]);
			tmp.RemoveAt (index);
		}
		return item;
	}
}