using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIInventorySlot : UISlot {
	public Inventory.Slot data;
	public override void OnSelect()
	{
		if (null == data) {
			return;
		}
		if (null == data.item) {
			return;
		}
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (data.item.info.category == other.category) {
				other.arrow.gameObject.SetActive (true);
			} else {
				other.arrow.gameObject.SetActive (false);
			}
		}
		inventory.itemInfo.data = data.item;
	}
	public override void OnDrop() {
		if (null == data) {
			return;
		}
		if (null == data.item) {
			return;
		}
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (data.item.info.category != other.category) {
				continue;
			}
			Rect rhs = clone.rectTransform.rect;
			Rect lhs = other.rect;
			rhs.position = (Vector2)clone.transform.position;
			lhs.position = (Vector2)other.transform.position;
			if (false == rhs.Overlaps(lhs)) {
				continue;
			}

			for (int j = 0; j < inventory.equipmentSlots.Length; j++) {
				inventory.equipmentSlots [j].arrow.gameObject.SetActive(false);
			}
			ItemData item = Player.Instance.inventory.Pull (data.index);
			Player.Instance.EquipItem (item, other.index);
			other.item = item;
			other.Activate (true);
			this.outline.size = 0;
			break;
		}
	}
}
