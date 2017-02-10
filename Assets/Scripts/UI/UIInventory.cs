﻿using UnityEngine;
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
		});
		gameObject.SetActive (false);
	}

	public void OnEnable()
	{
		itemInfo.gameObject.SetActive (false);
		DungeonMain.Instance.enableInput = false;
	}
	public void OnDisable()
	{
		//ItemManager.Instance.CreateItem("ITEM_WEAPON_" + Random.Range(0, 10).ToString()).Pickup();
		//ItemManager.Instance.CreateItem("ITEM_ARMOR_" + Random.Range(0, 10).ToString()).Pickup();
		//ItemManager.Instance.CreateItem("ITEM_RING_" + Random.Range(0, 10).ToString()).Pickup();
		//ItemManager.Instance.CreateItem("ITEM_SHIELD_" + Random.Range(0, 10).ToString()).Pickup();
		//ItemManager.Instance.CreateItem("ITEM_POTION_HEALING").Pickup();
		//ItemManager.Instance.CreateItem("ITEM_POTION_POISON").Pickup();
		DungeonMain.Instance.enableInput = true;
	}
	public UISlot selected {
		set {
			for (int i = 0; i < inventorySlots.Length; i++) {
				inventorySlots [i].outline.outline = false;
			}
			for (int i = 0; i < 7; i++) {
				equipmentSlots [i].outline.outline = false;
			}
			if (null != value) {
				value.outline.outline = true;
			}
		}
	}
	public void Put(Inventory.Slot data)
	{
		inventorySlots [data.index].Init (data);
	}
	public void Pull(int index)
	{
		inventorySlots [index].Init (null);
	}
	public void TurnEquipGuideArrowOn(EquipmentItem.Part part, int index = -1)
	{
		for (int i = 0; i < equipmentSlots.Length; i++) {
			UIEquipmentSlot other = equipmentSlots [i];
			if (part == other.part && index != other.index) {
				other.arrow.gameObject.SetActive (true);
			} else {
				other.arrow.gameObject.SetActive (false);
			}
		}
	}
	public void TurnEquipGuideArrowOff()
	{
		for (int i = 0; i < equipmentSlots.Length; i++) {
			equipmentSlots [i].arrow.gameObject.SetActive (false);
		}
	}
}
