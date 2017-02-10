using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIEquipmentSlot : UISlot {
	public EquipmentItem.Part part;
	public int index;
	public Image arrow;

	public override void Start( )
	{
		base.Start ();
		arrow = transform.FindChild ("Arrow").GetComponent<Image> ();
	}

	public override void OnSelect()
	{
		if (null == data) {
			return;
		}
		if (null == data.item) {
			return;
		}

		EquipmentItem item = (EquipmentItem)data.item;
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (item.part == other.part && this != other) {
				other.arrow.gameObject.SetActive (true);
			} else {
				other.arrow.gameObject.SetActive (false);
			}
		}

		inventory.itemInfo.slot = this;

		inventory.itemInfo.grade.color = UISlot.GetGradeColor (item.grade);
		inventory.itemInfo.stats.text = "+" + item.mainStat.value + " " + item.mainStat.description + "\n";
		for (int i = 0; i < item.subStats.Count; i++) {
			inventory.itemInfo.stats.text += "+" + item.subStats[i].value + " " + item.subStats [i].description + "\n";
		}
		inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].gameObject.SetActive (true);
		inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].onClick.AddListener (() => {
			Player.Instance.UnequipItem(part, index);
			Init(null);
			inventory.itemInfo.gameObject.SetActive(false);
		});
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
			if (this == other) {
				continue;
			}
			EquipmentItem item = (EquipmentItem)data.item;
			if (item.part != other.part) {
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

			Inventory.Slot curr = data;
			Player.Instance.UnequipItem (part, index);
			this.Init (null);

			EquipmentItem prev = Player.Instance.EquipItem (item, other.index);
			other.Init(curr);

			Player.Instance.EquipItem (prev, index);
			if (null != prev) {
				Inventory.Slot slot = new Inventory.Slot ();
				slot.index = index;
				slot.count = 1;
				slot.item = prev;
				this.Init (slot);
			}

			this.outline.outline = false;
			return;
		}

		for (int i = 0; i < inventory.inventorySlots.Length; i++) {
			UIInventorySlot other = inventory.inventorySlots [i];

			Rect rhs = clone.rectTransform.rect;
			Rect lhs = other.rectTransform.rect;
			rhs.position = (Vector2)clone.transform.position;
			lhs.position = (Vector2)other.transform.position;
			if (false == rhs.Overlaps(lhs)) {
				continue;
			}

			if (false == Player.Instance.inventory.Put (data.item)) {
				return;
			}

			Player.Instance.UnequipItem (part, index);
			Init (null);

			this.outline.outline = false;
			for (int j = 0; j < inventory.equipmentSlots.Length; j++) {
				inventory.equipmentSlots [j].arrow.gameObject.SetActive(false);
			}
			return;
		}
	}

}
