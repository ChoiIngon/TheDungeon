using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIPotionItem : UIItem
{
    public override void OnSelect()
	{
		inventory.item_info.buttons[(int)UIItemInfo.Action.Drink].gameObject.SetActive(true);
		inventory.item_info.actions[(int)UIItemInfo.Action.Drink] += () =>
		{
			PotionItem potionItem = GameManager.Instance.player.inventory.Remove(item_data.slot_index) as PotionItem;
			if (null == potionItem)
			{
				throw new System.InvalidCastException();
			}
			potionItem.Drink(GameManager.Instance.player);
			inventory.item_info.actions[(int)UIItemInfo.Action.Drink] = null;
			inventory.item_info.gameObject.SetActive(false);
		};
	}

	public override void OnEquipSlotDrop(UIEquipSlot slot)
	{
	}

	public override void OnItemSlotDrop(UIItemSlot slot)
	{
	}
	/*
public override void OnSlotSelectNotify(UIItem other)
{
	if (this == other)
	{
		outline.outline = true;
	}
	else
	{
		outline.outline = false;
	}

	inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].gameObject.SetActive (true);
	inventory.itemInfo.actions[(int)UIItemInfo.Action.Drop] += () => {
		inventory.Pull (data.index);
		inventory.TurnEquipGuideArrowOff();
		inventory.itemInfo.gameObject.SetActive (false);
	}
}

public override void OnSlotReleaseNotify(UIItem other)
{
	if(this == other)
	{
		return;
	}

	if(null == other.item_data)
	{
		return;
	}

	if(false == Contains(other))
	{
		return;
	}

	if(0 <= other.item_data.slot_index) // in inventory
	{
		GameManager.Instance.player.inventory.Swap(other.item_data.slot_index, slot_index);
	}
	else
	{
		EquipItem equipItem = other.item_data as EquipItem;
		GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
		GameManager.Instance.player.inventory.Add(equipItem);
	}

}
	*/
}
