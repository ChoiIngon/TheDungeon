using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIEquipmentSlot : UISlot {
	public ItemInfo.Category category;
	public Image arrow;
	public ItemData item;
	new void Start( )
	{
		base.Start ();
		arrow = transform.FindChild ("Arrow").GetComponent<Image> ();
	}

	public void Equip(ItemData item)
	{
		this.item = item;
		if (null == item) {
			return;
		}
		Activate (true);
	}

	public void Unequip()
	{
		Activate (false);
		item = null;
	}

	public override void OnSelect()
	{
		if (null == item) {
			return;
		}
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (item.info.category == other.category && this != other) {
				other.arrow.gameObject.SetActive (true);
			} else {
				other.arrow.gameObject.SetActive (false);
			}
		}
		inventory.itemInfo.data = item;
	}

	public override void OnDrop() {
		if (null == item) {
			return;
		}
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (this == other) {
				continue;
			}
			if (item.info.category != other.category) {
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

			EquipmentItemData curr = Player.Instance.UnequipItem (item.info.category, index);
			this.Unequip ();

			EquipmentItemData prev = Player.Instance.EquipItem (curr, other.index);
			other.Equip(curr);

			Player.Instance.EquipItem (prev, index);
			this.Equip(prev);

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

			if (false == Player.Instance.inventory.Put (item)) {
				return;
			}

			Player.Instance.UnequipItem (item.info.category, index);
			Unequip ();

			this.outline.outline = false;
			for (int j = 0; j < inventory.equipmentSlots.Length; j++) {
				inventory.equipmentSlots [j].arrow.gameObject.SetActive(false);
			}
			return;
		}
	}
	public override void Activate(bool flag)
	{
		base.Activate (flag && null != item);
		if (true == flag && null != item) {
			icon.sprite = item.info.icon;
			grade.color = UISlot.GetGradeColor (item.info.grade);
		}
	}
}
