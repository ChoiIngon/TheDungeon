using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeyItem : UIItem
{
	public override void OnSelect()
	{
		inventory.item_info.Clear();
		inventory.item_info.SetItemIcon(this);
		inventory.item_info.SetItemName(item_data.meta.name);
		inventory.item_info.SetDescription(item_data.meta.description);
		inventory.item_info.SetButtonListener(UIItemInfo.Action.Drop, () =>
		{
			GameManager.Instance.player.inventory.Remove(item_data.slot_index);
			inventory.item_info.Clear();
		});
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
