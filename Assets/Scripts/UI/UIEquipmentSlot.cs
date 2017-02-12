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
		inventory.TurnEquipGuideArrowOn (part, index);
		inventory.itemInfo.slot = this;
		inventory.itemInfo.stats.text = "+" + item.mainStat.value + " " + item.mainStat.description + "\n";
		for (int i = 0; i < item.subStats.Count; i++) {
			inventory.itemInfo.stats.text += "+" + item.subStats[i].value + " " + item.subStats [i].description + "\n";
		}
		inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].gameObject.SetActive (true);
		inventory.itemInfo.actions[(int)UIItemInfo.Action.Drop] += () => {
			Player.Instance.UnequipItem(part, index);
			Init(null);
			inventory.TurnEquipGuideArrowOff();
			inventory.itemInfo.gameObject.SetActive(false);
		};
	}

	public override void OnDrop() {
		if (null == data) {
			return;
		}
		if (null == data.item) {
			return;
		}

		EquipmentItem item = (EquipmentItem)data.item;
		for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
			UIEquipmentSlot other = inventory.equipmentSlots [i];
			if (this == other) {
				continue;
			}
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

			Player.Instance.UnequipItem (part, index);
			EquipmentItem prev = Player.Instance.EquipItem (item, other.index);
			other.Init(data);
			Init (null);

			if (null != prev) {
				Player.Instance.EquipItem (prev, index);
				Inventory.Slot slot = new Inventory.Slot ();
				slot.index = index;
				slot.count = 1;
				slot.item = prev;
				this.Init (slot);
			}

			other.OnSelect ();
			inventory.TurnEquipGuideArrowOff ();
			outline.outline = false;
			return;
		}
			
		if (Inventory.MAX_SLOT_COUNT <= Player.Instance.inventory.count) {
			inventory.TurnEquipGuideArrowOff ();
			outline.outline = false;
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

			Inventory.Slot slot = Player.Instance.inventory.Put (data.item);
			if (null == slot) {
				return;
			}

			Player.Instance.UnequipItem (part, index);
			Init (null);

			other = inventory.inventorySlots [slot.index];
			other.OnSelect ();
			inventory.TurnEquipGuideArrowOff ();
			outline.outline = false;
			return;
		}
	}

}
