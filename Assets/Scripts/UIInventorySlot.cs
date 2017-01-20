using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIInventorySlot : UISlot {
	public override void OnSelect()
	{
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (item.info.category == other.category) {
				other.arrow.gameObject.SetActive (true);
			}
		}
	}
	public override void OnDrop() {
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (item.info.category != other.category) {
				continue;
			}
			if (false == clone.rectTransform.rect.Overlaps (other.rect)) {
				continue;
			}
			Player.Instance.EquipItem (item, other.index);
			other.SetItem (item);
			break;
		}
	}
}
