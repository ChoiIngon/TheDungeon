﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeyItem : UIItem
{
	public override void OnSelect()
	{
		inventory.item_info.Clear();
		inventory.item_info.SetItemIcon(this);
		inventory.item_info.SetItemName(data.meta.name);
		inventory.item_info.SetDescription(data.meta.description);
		inventory.item_info.SetButtonListener(UIItemInfo.Action.Drop, () =>
		{
			ProgressManager.Instance.Update(ProgressType.SellKey, "", 1);
			GameManager.Instance.player.inventory.Remove(data.slot_index);
			inventory.item_info.Clear();
		});
	}

	public override void OnEquipSlotDrop(UIEquipSlot slot)
	{
	}

	public override void OnItemSlotDrop(UIItemSlot slot)
	{
		if (slot.slot_index == data.slot_index)
		{
			return;
		}
		int prevSlotIndex = data.slot_index;
		GameManager.Instance.player.inventory.Remove(data.slot_index);
		Item prev = GameManager.Instance.player.inventory.Remove(slot.slot_index);
		if (null != prev)
		{
			GameManager.Instance.player.inventory.Add(prev, prevSlotIndex);
		}
		GameManager.Instance.player.inventory.Add(data, slot.slot_index);
	}

	public override void OnDrop()
	{
	}
}
