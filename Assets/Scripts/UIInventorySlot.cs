using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIInventorySlot : UISlot {
	public Inventory.Slot data;
	public override void Activate(bool flag)
	{
		base.Activate (flag);
		if (true == flag) {
			icon.sprite = data.item.icon;
			grade.color = UISlot.GetGradeColor (data.item.grade);
		}
	}
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
		inventory.itemInfo.item = data.item;
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

				Player.Instance.inventory.Pull (data.index);
				EquipmentItem prev = Player.Instance.EquipItem (equipment, other.index);
				Player.Instance.inventory.Put (prev);
				other.item = equipment;
				other.Activate (true);
				this.outline.outline = false;
				for (int j = 0; j < inventory.equipmentSlots.Length; j++) {
					inventory.equipmentSlots [j].arrow.gameObject.SetActive (false);
				}

				return;
			}
		}
	}
}
