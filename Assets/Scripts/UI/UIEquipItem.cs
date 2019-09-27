using UnityEngine;


public class UIEquipItem : UIItem
{
    public override void OnSelect()
	{
		EquipItem equipItem = (EquipItem)item_data;
		foreach(var itr in inventory.equip_slots)
		{
			UIEquipSlot slot = itr.Value;
			if(slot.part != equipItem.part)
			{
				slot.SetActiveGuideArrow(false);
				continue;
			}

			if(slot.equip_index == equipItem.equip_index)
			{
				slot.SetActiveGuideArrow(false);
				continue;
			}

			slot.SetActiveGuideArrow(true);
		}
	}

	public override void OnEquipSlotDrop(UIEquipSlot slot)
	{
		foreach (var itr in inventory.equip_slots)
		{
			itr.Value.SetActiveGuideArrow(false);
		}

		EquipItem equipItem = (EquipItem)item_data;
		if (slot.part != equipItem.part)
		{
			return;
		}

		if (null != slot.item)
		{
			EquipItem unequipItem = GameManager.Instance.player.Unequip(slot.part, slot.equip_index);
			GameManager.Instance.player.inventory.Add(unequipItem);
		}

		if (true == equipItem.equip)
		{
			GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
		}
		else
		{
			GameManager.Instance.player.inventory.Remove(equipItem.slot_index);
		}
		GameManager.Instance.player.Equip((EquipItem)item_data, slot.equip_index);
	}

	public override void OnItemSlotDrop(UIItemSlot slot)
	{
		foreach (var itr in inventory.equip_slots)
		{
			itr.Value.SetActiveGuideArrow(false);
		}

		EquipItem equipItem = (EquipItem)item_data;
		if (true == equipItem.equip)
		{
			GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			GameManager.Instance.player.inventory.Add(equipItem);
		}
		else
		{
			if(slot.slot_index == equipItem.slot_index)
			{
				return;
			}
			int prevSlotIndex = equipItem.slot_index;
			GameManager.Instance.player.inventory.Remove(equipItem.slot_index);
			Item prev = GameManager.Instance.player.inventory.Remove(slot.slot_index);
			if (null != prev)
			{
				GameManager.Instance.player.inventory.Add(prev, prevSlotIndex);
			}
			GameManager.Instance.player.inventory.Add(equipItem, slot.slot_index);
		}
	}

	public override void OnDrop()
	{
		EquipItem equipItem = (EquipItem)item_data;
		if (true == equipItem.equip)
		{
			GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			GameManager.Instance.player.inventory.Add(equipItem);
		}
	}
}
