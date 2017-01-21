using UnityEngine;
using System.Collections;

public class ArmorItemInfo : ItemInfo {
	public int defense;

	public ArmorItemInfo() {
		category = ItemInfo.Category.Armor;
	}

	public override ItemData CreateInstance()
	{
		ArmorItemData data = new ArmorItemData ();
		data.info = this;
		return data;
	}
};

public class ArmorItemData : EquipmentItemData {
	/*
	public override Character.Status GetStatus()
	{
		WeaponItemInfo weapon = (WeaponItemInfo)this.info;
		Character.Status status = new Character.Status ();
		status.attack = weapon.attack;
		status.speed = weapon.speed;
		return status;
	}
	*/
}
