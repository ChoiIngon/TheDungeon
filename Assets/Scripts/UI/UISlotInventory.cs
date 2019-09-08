using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UISlotInventory : UISlot
{
    public int slot_index = -1;
    public override void OnSlotSelectNotify(UISlot other)
    {
        if (this == other)
        {
            outline.outline = true;
        }
        else
        {
            outline.outline = false;
        }
        /*
         
		inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].gameObject.SetActive (true);
		inventory.itemInfo.actions[(int)UIItemInfo.Action.Drop] += () => {
			inventory.Pull (data.index);
			inventory.TurnEquipGuideArrowOff();
			inventory.itemInfo.gameObject.SetActive (false);
		}
        */
    }

    public override void OnSlotReleaseNotify(UISlot other)
    {
        if(this == other)
        {
            return;
        }

        if(null == other.item)
        {
            return;
        }

        if(false == Contains(other))
        {
            return;
        }

        if(0 <= other.item.slot_index) // in inventory
        {
            GameManager.Instance.player.inventory.Swap(other.item.slot_index, slot_index);
        }
        else
        {
            EquipItem equipItem = other.item as EquipItem;
            GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
            GameManager.Instance.player.inventory.Add(equipItem);
        }
    }
}
