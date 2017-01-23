using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UIInventorySlot : UISlot {
	public Inventory.Slot data;
	void OnEnable()
	{
		Controller.Instance.SetState (Controller.State.Popup);
	}
	void OnDisable() {
		Controller.Instance.SetState (Controller.State.Idle);
	}
	public override void Activate(bool flag)
	{
		base.Activate (flag);
		if (true == flag) {
			icon.sprite = data.item.info.icon;
			grade.color = UISlot.GetGradeColor (data.item.info.grade);
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
			Rect lhs = other.rectTransform.rect;
			rhs.position = (Vector2)clone.transform.position;
			lhs.position = (Vector2)other.transform.position;
			if (false == rhs.Overlaps(lhs)) {
				continue;
			}

			for (int j = 0; j < inventory.equipmentSlots.Length; j++) {
				inventory.equipmentSlots [j].arrow.gameObject.SetActive(false);
			}
			ItemData item = Player.Instance.inventory.Pull (data.index);
			EquipmentItemData prev = Player.Instance.EquipItem (item, other.index);
			Player.Instance.inventory.Put (prev);
			other.item = item;
			other.Activate (true);
			this.outline.outline = false;
			return;
		}
	}
}
