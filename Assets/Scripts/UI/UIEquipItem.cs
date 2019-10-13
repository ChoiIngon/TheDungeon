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

		inventory.item_info.Clear();
		inventory.item_info.SetItemIcon(this);
		inventory.item_info.SetItemName(item_data.meta.name + "\n" + "<size=" + (inventory.item_info.name.fontSize * 0.8) + ">Lv." + equipItem.level + "</size>");

		string description = item_data.meta.description + "\n\n";

		foreach (Stat.Data stat in equipItem.main_stat.GetStats())
		{
			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT stat_name, description FROM meta_stat where stat_type=" + (int)stat.type);
			while (true == reader.Read())
			{
				description += "<color=white> -" + string.Format(reader.GetString("description"), stat.value) + "</color>\n";
			}
		}

		foreach (Stat.Data stat in equipItem.sub_stat.GetStats())
		{
			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT stat_name, description FROM meta_stat where stat_type=" + (int)stat.type);
			while (true == reader.Read())
			{
				description += "<color=green> -" + string.Format(reader.GetString("description"), stat.value) + "</color>\n";
			}
		}

		if (null != equipItem.skill)
		{
			description += "<color=red> -" + equipItem.skill.meta.description + "</color>";
		}
		inventory.item_info.SetDescription(description);
		//inventory.item_info.description.text = item_data.description;
		inventory.item_info.SetButtonListener(UIItemInfo.Action.Drop, () =>
		{
			if (0 <= item_data.slot_index)
			{
				GameManager.Instance.player.inventory.Remove(item_data.slot_index);
			}
			else
			{
				GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			}
			inventory.item_info.Clear();
		});

	}

	public override void OnEquipSlotDrop(UIEquipSlot slot)
	{
		foreach (var itr in inventory.equip_slots)
		{
			itr.Value.SetActiveGuideArrow(false);
		}

		EquipItem equipItemData = (EquipItem)item_data;
		if (slot.part != equipItemData.part)
		{
			return;
		}

		if (null != slot.item)
		{
			EquipItem unequipItem = GameManager.Instance.player.Unequip(slot.part, slot.equip_index);
			GameManager.Instance.player.inventory.Add(unequipItem);
		}

		if (true == equipItemData.equip)
		{
			GameManager.Instance.player.Unequip(equipItemData.part, equipItemData.equip_index);
		}
		else
		{
			GameManager.Instance.player.inventory.Remove(equipItemData.slot_index);
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
