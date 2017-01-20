using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class UIInventory : MonoBehaviour {
	public Text itemName;
	[HideInInspector]

	public UIInventorySlot[] inventorySlots;
	public UIEquipmentSlot[] equipmentSlots;
	public Button button;
	public Text gold;
	void Start() {
		{
			inventorySlots = new UIInventorySlot[Inventory.MAX_SLOT_COUNT];
			Transform tr = transform.FindChild ("InventorySlots");
			for (int i = 0; i < Inventory.MAX_SLOT_COUNT; i++) {
				UIInventorySlot slot = tr.GetChild (i).GetComponent<UIInventorySlot> ();
				slot.inventory = this;
				inventorySlots [i] = slot;
			}
		}
		{
			equipmentSlots = new UIEquipmentSlot[7];
			Transform tr = transform.FindChild ("EquipmentSlots");
			for (int i = 0; i < 7; i++) {
				UIEquipmentSlot slot = tr.GetChild (i).GetComponent<UIEquipmentSlot> ();
				slot.inventory = this;
				equipmentSlots [i] = slot;
			}
		}

		button = transform.FindChild ("Close").GetComponent<Button> ();
		button.onClick.AddListener (() => {
			gameObject.SetActive(false);
		});
		WeaponItemInfo item = new WeaponItemInfo ();
		Player.Instance.inventory.Put (item.CreateInstance());
	}

	public void SetSelectedSlot(UISlot slot)
	{
		for (int i = 0; i < inventorySlots.Length; i++) {
			inventorySlots [i].outline.size = 0;
		}
		slot.outline.size = 3;
	}

	public void Put(Inventory.Slot data)
	{
		UIInventorySlot slot = inventorySlots [data.index];
		slot.item = data.item;
		slot.Activate (true);
	}

	public void Equip(ItemInfo.Category category, int index)
	{
	}
	public void Unequip(ItemInfo.Category category, int index)
	{
		
	}
}
