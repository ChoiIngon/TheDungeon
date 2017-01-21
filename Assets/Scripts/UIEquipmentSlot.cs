using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIEquipmentSlot : UISlot {
	public ItemInfo.Category category;
	public Image arrow;
	public ItemData item;
	new void Start( )
	{
		base.Start ();
		arrow = transform.FindChild ("Arrow").GetComponent<Image> ();
	}

	public void Equip(ItemData item)
	{
		this.item = item;
		Activate(true);
	}
}
