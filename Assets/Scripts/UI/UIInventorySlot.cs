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

		inventory.itemInfo.slot = this;

		switch (data.item.type) {
		case Item.Type.Equipment:
			{
				EquipmentItem item = (EquipmentItem)data.item;
				for (int i = 0; i < inventory.equipmentSlots.Length; i++) {
					UIEquipmentSlot other = inventory.equipmentSlots [i];
					if (item.part == other.part) {
						other.arrow.gameObject.SetActive (true);
					} else {
						other.arrow.gameObject.SetActive (false);
					}
				}

				inventory.itemInfo.grade.color = UISlot.GetGradeColor (item.grade);
				inventory.itemInfo.stats.text = "+" + item.mainStat.value + " " + item.mainStat.description + "\n";
				for (int i = 0; i < item.subStats.Count; i++) {
					inventory.itemInfo.stats.text += "+" + item.subStats [i].value + " " + item.subStats [i].description + "\n";
				}
			}
			break;
		case Item.Type.Potion:
			{
				PotionItem item = (PotionItem)data.item;

				inventory.itemInfo.buttons [(int)UIItemInfo.Action.Use].gameObject.SetActive (true);
				inventory.itemInfo.buttons [(int)UIItemInfo.Action.Use].onClick.AddListener (() => {
					item.Use (Player.Instance);
					Player.Instance.inventory.Pull (data.index);
					inventory.itemInfo.gameObject.SetActive (false);
				});
			}
			break;
		}

		inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].gameObject.SetActive (true);
		inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].onClick.AddListener (() => {
			Player.Instance.inventory.Pull(data.index);
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

				inventory.itemInfo.slot = equipmentSlot;
				return;
			}
		}
	}
}
