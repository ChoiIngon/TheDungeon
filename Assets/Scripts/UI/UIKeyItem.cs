using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeyItem : UIItem
{
	public override void OnSelect()
	{
	}

	public override void OnEquipSlotDrop(UIEquipSlot slot)
	{
	}

	public override void OnItemSlotDrop(UIItemSlot slot)
	{
		if (slot.slot_index == item_data.slot_index)
		{
			return;
		}
		int prevSlotIndex = item_data.slot_index;
		GameManager.Instance.player.inventory.Remove(item_data.slot_index);
		Item prev = GameManager.Instance.player.inventory.Remove(slot.slot_index);
		if (null != prev)
		{
			GameManager.Instance.player.inventory.Add(prev, prevSlotIndex);
		}
		GameManager.Instance.player.inventory.Add(item_data, slot.slot_index);
	}

	public override void OnDrop()
	{
	}
}
