using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIInventory : MonoBehaviour {
	public Text itemName;

	public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
	public UIInventorySlot uiSlotPrefab;
	[HideInInspector]
	public UIInventorySlot selected;

	public Transform slots;
	//public Button[] buttons;
	public Text weight;
	public Text gold;
	void Start() {
		/*
		for (int i=0; i<Inventory.MAX_SLOT_COUNT; i++) {
			UIInventorySlot slot = Instantiate<UIInventorySlot>(uiSlotPrefab);
			slot.transform.SetParent(slots, false);
		}
		*/
		selected = null;
	}
	public void SetSeletedItemInfo(ItemData data) {
	}
}
