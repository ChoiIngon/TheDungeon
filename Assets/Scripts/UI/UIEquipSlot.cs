using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipSlot : UIItemSlot
{
	public EquipItem.Part part = EquipItem.Part.Invalid;
	public int equip_index = 0; // left or right

	public override void OnSlotSelectNotify(UIItem item)
	{
	}

	public override void OnSlotReleaseNotify(UIItem item)
	{
		if (false == Overlaps(item))
		{
			return;
		}

		if (null == item.item_data)
		{
			return;
		}
		item.OnEquipSlotDrop(this);
	}
}
