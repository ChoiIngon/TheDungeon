using UnityEngine;


public class UIEquipItem : UIItem
{
    public override void Awake()
    {
        base.Awake();
        

        //Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
        //Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
    }

	protected override void OnDestroy()
	{
		//Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
		//Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
		base.OnDestroy();
	}

	public override void OnSelect()
	{
		if(null == item_data)
		{
			return;
		}

		EquipItem equipItem = (EquipItem)item_data;
		foreach(var itr in inventory.equip_slots)
		{
			UIEquipSlot slot = itr.Value;
			slot.SetActiveGuideArrow(false);
			if(slot.part != equipItem.part)
			{
				continue;
			}

			if(slot.equip_index == equipItem.equip_index)
			{
				continue;
			}

			slot.SetActiveGuideArrow(true);
		}
	}

	public override void OnEquipSlotDrop(UIEquipSlot slot)
	{
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
}
