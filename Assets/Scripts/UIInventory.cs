using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class UIInventory : MonoBehaviour {
	public UIItemInfo itemInfo;
	public UIInventorySlot[] inventorySlots;
	public UIEquipmentSlot[] equipmentSlots;
	public Button close;
	public Text gold;
	public void Init() {
		{
			inventorySlots = new UIInventorySlot[Inventory.MAX_SLOT_COUNT];
			Transform tr = transform.FindChild ("InventorySlots");
			for (int i = 0; i < Inventory.MAX_SLOT_COUNT; i++) {
				UIInventorySlot slot = tr.GetChild (i).GetComponent<UIInventorySlot> ();
				slot.inventory = this;
				slot.data = Player.Instance.inventory.slots [i];
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

		close = transform.FindChild ("Close").GetComponent<Button> ();
		close.onClick.AddListener (() => {
			gameObject.SetActive(false);

			Player.Instance.inventory.Put (ItemManager.Instance.CreateItem("ITEM_WEAPON_" + Random.Range(0, 5).ToString()));

			ArmorItemInfo armor = new ArmorItemInfo ();
			Player.Instance.inventory.Put (armor.CreateInstance());
		});
	}

	public void OnEnable()
	{
		itemInfo.gameObject.SetActive (false);
	}
	public UISlot selected {
		set {
			for (int i = 0; i < inventorySlots.Length; i++) {
				inventorySlots [i].outline.size = 0;
			}
			for (int i = 0; i < 7; i++) {
				equipmentSlots [i].outline.size = 0;
			}
			if (null != value) {
				value.outline.size = 3;
			}
		}
	}
	public void ActivateInventorySlot(int index, bool flag)
	{
		UIInventorySlot slot = inventorySlots [index];
		slot.Activate (flag);
	}

	public void ActivateEquipmentSlot(ItemInfo.Category category, int index, bool flag)
	{
		for (int i = 0; i < 7; i++) {
			UIEquipmentSlot slot = equipmentSlots [i];
			if (slot.category == category && slot.index == index) {
				slot.Activate (flag);
				break;
			}
		}
	}
}
