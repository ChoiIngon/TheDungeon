using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UISlotInventory : UISlot
{
    public override void OnSlotSelectNotify(UISlot other)
    {
        /*
         * if (null == data) {
			return;
		}
		if (null == data.item) {
			return;
		}

		inventory.TurnEquipGuideArrowOff ();
		inventory.itemInfo.slot = this;

		switch (data.item.type) {
		case Item.Type.Equipment:
			{
				EquipmentItem item = (EquipmentItem)data.item;
				inventory.TurnEquipGuideArrowOn (item.part);
				inventory.itemInfo.stats.text = item.mainStat.value + " " + item.mainStat.description + "\n";
				for (int i = 0; i < item.subStats.Count; i++) {
					inventory.itemInfo.stats.text += "<color=green> +" + item.subStats[i].value + " " + item.subStats [i].description + "</color>\n";
				}
			}
			break;
		case Item.Type.Potion:
			{
				PotionItem item = (PotionItem)data.item;
				inventory.itemInfo.buttons [(int)UIItemInfo.Action.Use].gameObject.SetActive (true);
				inventory.itemInfo.actions[(int)UIItemInfo.Action.Use] += () => {
					item.Use (DungeonMain.Instance.player);
					inventory.Pull (data.index);
					inventory.itemInfo.gameObject.SetActive (false);
				};
			}
			break;
		}

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

        if(false == Overlaps(other))
        {
            return;
        }

        if(0 <= other.item.slot_index) // in inventory
        {
            GameManager.Instance.player.inventory.Swap(other.item.slot_index, item.slot_index);
        }
        else
        {
            EquipItem equipItem = other.item as EquipItem;
            GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
            GameManager.Instance.player.inventory.Add(equipItem);
        }
        /*
         *if (null == data) {
               return;
           }
           if (null == data.item) {
               return;
           }

           for (int i = 0; i < inventory.inventorySlots.Length; i++) {
               UIInventorySlot other = inventory.inventorySlots [i];
               if (index == other.index) {
                   continue;
               }

               Rect rhs = clone.rectTransform.rect;
               Rect lhs = other.rectTransform.rect;
               rhs.position = (Vector2)clone.transform.position;
               lhs.position = (Vector2)other.transform.position;
               if (false == rhs.Contains(lhs.center)) {
                   continue;
               }

               Player.Instance.inventory.Swap (index, other.index);

               other.OnSelect ();
               other.outline.outline = true;
               outline.outline = false;
               return;
           }
           if (Item.Type.Equipment == data.item.type) {
               EquipmentItem equipment = (EquipmentItem)data.item;

               for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
                   UIEquipmentSlot other = inventory.equipmentSlots [i];
                   if (equipment.part != other.part) {
                       continue;
                   }
                   Rect rhs = clone.rectTransform.rect;
                   Rect lhs = other.rectTransform.rect;
                   rhs.position = (Vector2)clone.transform.position;
                   lhs.position = (Vector2)other.transform.position;
                   if (false == rhs.Overlaps (lhs)) {
                       continue;
                   }

                   inventory.Pull (data.index);
                   EquipmentItem prev = DungeonMain.Instance.player.EquipItem (equipment, other.index);

                   inventory.Put (prev);
                   Inventory.Slot slot = new Inventory.Slot ();
                   slot.index = other.index;
                   slot.count = 1;
                   slot.item = equipment;
                   other.Init (slot);
                   other.OnSelect ();
                   inventory.TurnEquipGuideArrowOff ();
                   outline.outline = false;
                   return;
               }
           }*/
    }
}
