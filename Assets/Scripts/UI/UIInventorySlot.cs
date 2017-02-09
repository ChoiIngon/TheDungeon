using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIInventorySlot : UISlot {
	public override void OnSelect()
	{
		if (null == data) {
			return;
		}
		if (null == data.item) {
			return;
		}

		if (Item.Type.Equipment == data.item.type) {
			EquipmentItem equipment = (EquipmentItem)data.item;
			for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
				UIEquipmentSlot other = inventory.equipmentSlots [i];
				if (equipment.part == other.part) {
					other.arrow.gameObject.SetActive (true);
				} else {
					other.arrow.gameObject.SetActive (false);
				}
			}
		}
		inventory.itemInfo.slot = this;
	}
	public override void OnDrop() {
		if (null == data) {
			return;
		}
		if (null == data.item) {
			return;
		}
		if (Item.Type.Equipment == data.item.type) {
			EquipmentItem equipment = (EquipmentItem)data.item;

			for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
				UIEquipmentSlot equipmentSlot = inventory.equipmentSlots [i];
				if (equipment.part != equipmentSlot.part) {
					continue;
				}
				Rect rhs = clone.rectTransform.rect;
				Rect lhs = equipmentSlot.rectTransform.rect;
				rhs.position = (Vector2)clone.transform.position;
				lhs.position = (Vector2)equipmentSlot.transform.position;
				if (false == rhs.Overlaps (lhs)) {
					continue;
				}

				Player.Instance.inventory.Pull (data.index);
				EquipmentItem prev = Player.Instance.EquipItem (equipment, equipmentSlot.index);
				Inventory.Slot slot = new Inventory.Slot ();
				slot.index = equipmentSlot.index;
				slot.count = 1;
				slot.item = equipment;
				equipmentSlot.Init (slot);
				Player.Instance.inventory.Put (prev);
				this.outline.outline = false;
				for (int j = 0; j < inventory.equipmentSlots.Length; j++) {
					inventory.equipmentSlots [j].arrow.gameObject.SetActive (false);
				}

				return;
			}
		}
	}
}
