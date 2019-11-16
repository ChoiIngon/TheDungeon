using UnityEngine;
using UnityEngine.UI;

public class UIEquipItem : UIItem
{
    public override void OnSelect()
	{
		EquipItem equipItem = (EquipItem)data;
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
		inventory.item_info.SetItemName(data.meta.name + "\n" + "<size=" + (inventory.item_info.name.fontSize * 0.8) + ">Lv." + equipItem.level + "</size>");

		string description = data.meta.description + "\n\n";

		foreach (Stat.Data stat in equipItem.main_stat.GetStats())
		{
			Stat.Meta meta = Stat.GetMeta(stat.type);
			description += "<color=white> " + meta.ToString(stat.value) + "</color>\n";
		}

		foreach (Stat.Data stat in equipItem.sub_stat.GetStats())
		{
			Stat.Meta meta = Stat.GetMeta(stat.type);
			description += "<color=#4eb105> " + meta.ToString(stat.value) + "</color>\n";
		}

		if (null != equipItem.skill)
		{
			description += "<color=red> " + equipItem.skill.meta.description + "</color>\n";
		}
		inventory.item_info.SetDescription(description);

		Text text = UIUtil.FindChild<Text>(inventory.item_info.buttons[(int)UIItemInfo.Action.Equip_0].transform, "Text");
		if (EquipItem.Part.Hand == equipItem.part || EquipItem.Part.Ring == equipItem.part)
		{
			text.text = "[Equip:L]";
		}
		else
		{
			text.text = "[Equip]";
		}
		inventory.item_info.SetButtonListener(UIItemInfo.Action.Equip_0, () =>
		{
			if (0 <= equipItem.slot_index)
			{
				GameManager.Instance.player.inventory.Remove(equipItem.slot_index);
			}
			if (true == equipItem.equip)
			{
				GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			}
			EquipItem prevItem = GameManager.Instance.player.Equip(equipItem, 0);
			if (null != prevItem)
			{
				GameManager.Instance.player.inventory.Add(prevItem);
			}
			OnSelect();
			foreach (var itr in inventory.equip_slots)
			{
				itr.Value.SetActiveGuideArrow(false);
			}
		});
		inventory.item_info.SetButtonListener(UIItemInfo.Action.Equip_1, () =>
		{
			if (0 <= equipItem.slot_index)
			{
				GameManager.Instance.player.inventory.Remove(equipItem.slot_index);
			}
			if (true == equipItem.equip)
			{
				GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			}
			EquipItem prevItem = GameManager.Instance.player.Equip(equipItem, 1);
			if (null != prevItem)
			{
				GameManager.Instance.player.inventory.Add(prevItem);
			}
			OnSelect();
			foreach (var itr in inventory.equip_slots)
			{
				itr.Value.SetActiveGuideArrow(false);
			}
		});
		inventory.item_info.SetButtonListener(UIItemInfo.Action.Unequip, () =>
		{
			EquipItem item = GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			GameManager.Instance.player.inventory.Add(item);
			OnSelect();
			foreach (var itr in inventory.equip_slots)
			{
				itr.Value.SetActiveGuideArrow(false);
			}
		});
		inventory.item_info.SetButtonListener(UIItemInfo.Action.Drop, () =>
		{
			if (0 <= data.slot_index)
			{
				GameManager.Instance.player.inventory.Remove(data.slot_index);
			}
			else
			{
				GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			}
			inventory.item_info.Clear();
		});

		if (true == equipItem.equip)
		{
			if (EquipItem.Part.Hand == equipItem.part || EquipItem.Part.Ring == equipItem.part)
			{
				if (0 == equipItem.equip_index)
				{
					inventory.item_info.buttons[(int)UIItemInfo.Action.Equip_0].gameObject.SetActive(false);
				}
				if (1 == equipItem.equip_index)
				{
					inventory.item_info.buttons[(int)UIItemInfo.Action.Equip_1].gameObject.SetActive(false);
				}
			}
			else
			{
				inventory.item_info.buttons[(int)UIItemInfo.Action.Equip_0].gameObject.SetActive(false);
				inventory.item_info.buttons[(int)UIItemInfo.Action.Equip_1].gameObject.SetActive(false);
			}
			inventory.item_info.buttons[(int)UIItemInfo.Action.Drop].gameObject.SetActive(false);
		}
		else
		{
			if (EquipItem.Part.Hand == equipItem.part || EquipItem.Part.Ring == equipItem.part)
			{
				// 
			}
			else
			{
				inventory.item_info.buttons[(int)UIItemInfo.Action.Equip_1].gameObject.SetActive(false);
			}
			inventory.item_info.buttons[(int)UIItemInfo.Action.Unequip].gameObject.SetActive(false);
		}
	}

	public override void OnEquipSlotDrop(UIEquipSlot slot)
	{
		foreach (var itr in inventory.equip_slots)
		{
			itr.Value.SetActiveGuideArrow(false);
		}

		EquipItem equipItemData = (EquipItem)data;
		if (slot.part != equipItemData.part)
		{
			return;
		}

		Stat prev = GameManager.Instance.player.stats + new Stat();
		EquipItem unequipItem = null;
		if (null != slot.item)
		{
			if (slot.item.data == data)
			{
				return;
			}
			unequipItem = GameManager.Instance.player.Unequip(slot.part, slot.equip_index);
			GameManager.Instance.player.inventory.Add(unequipItem);
		}

		bool alreadyEquiped = equipItemData.equip;
		if (true == equipItemData.equip)
		{
			GameManager.Instance.player.Unequip(equipItemData.part, equipItemData.equip_index);
		}
		else
		{
			GameManager.Instance.player.inventory.Remove(equipItemData.slot_index);
		}
		GameManager.Instance.player.Equip((EquipItem)data, slot.equip_index);

		/*
		if (false == alreadyEquiped)
		{
			string changedStat = DiffStat(prev, GameManager.Instance.player.stats);

			if (null != equipItemData.skill)
			{
				changedStat += "<color=white> " + equipItemData.skill.meta.description + "</color>";
			}

			if (null != unequipItem && null != unequipItem.skill)
			{
				changedStat += "<color=red> " + unequipItem.skill.meta.description + "</color>";
			}
			GameManager.Instance.ui_textbox.AsyncWrite(changedStat, false);
		}
		*/
		OnSelect();
	}

	public override void OnItemSlotDrop(UIItemSlot slot)
	{
		foreach (var itr in inventory.equip_slots)
		{
			itr.Value.SetActiveGuideArrow(false);
		}

		EquipItem equipItem = (EquipItem)data;
		if (true == equipItem.equip)
		{
			Stat prevStat = GameManager.Instance.player.stats + new Stat();
			GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			GameManager.Instance.player.inventory.Add(equipItem);

			string changedStat = DiffStat(prevStat, GameManager.Instance.player.stats);
			if (null != equipItem.skill)
			{
				changedStat += "<color=red> -" + equipItem.skill.meta.description + "</color>";
			}
			GameManager.Instance.ui_textbox.AsyncWrite(changedStat, false);
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
		OnSelect();
	}

	public override void OnDrop()
	{
		EquipItem equipItem = (EquipItem)data;
		if (true == equipItem.equip)
		{
			foreach (var itr in inventory.equip_slots)
			{
				itr.Value.SetActiveGuideArrow(false);
			}

			Stat prev = GameManager.Instance.player.stats + new Stat();
			GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			GameManager.Instance.player.inventory.Add(equipItem);

			string changedStat = DiffStat(prev, GameManager.Instance.player.stats);
			if (null != equipItem.skill)
			{
				changedStat += "<color=red> " + equipItem.skill.meta.description + "</color>";
			}
			GameManager.Instance.ui_textbox.AsyncWrite(changedStat, false);
		}
		OnSelect();
	}

	private string DiffStat(Stat prev, Stat after)
	{
		Stat curr = after - prev;
		string statText = "";
		foreach (Stat.Data stat in curr.GetStats())
		{
			Stat.Meta meta = Stat.GetMeta(stat.type);

			if (0.0f < stat.value)
			{
				statText += meta.ToString(stat.value) + "\n";
			}
			else if (0.0f > stat.value)
			{
				statText += "<color=red> " + meta.ToString(stat.value) + "</color>\n";
			}
		}
		return statText;
	}
}
