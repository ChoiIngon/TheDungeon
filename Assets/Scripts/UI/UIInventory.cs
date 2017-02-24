using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour {
	public UIPlayerInfo playerInfo;
	public UIItemInfo itemInfo;
	public UIInventorySlot[] inventorySlots;
	public UIEquipmentSlot[] equipmentSlots;
	public Button close;
	public void Init() {
		{
			inventorySlots = new UIInventorySlot[Inventory.MAX_SLOT_COUNT];
			Transform tr = transform.FindChild ("InventorySlots");
			for (int i = 0; i < Inventory.MAX_SLOT_COUNT; i++) {
				UIInventorySlot slot = tr.GetChild (i).GetComponent<UIInventorySlot> ();
				slot.inventory = this;
				slot.index = i;
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
			
		EventTrigger trigger = close.gameObject.AddComponent<EventTrigger>();
		var entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback.AddListener (( data) => {
			gameObject.SetActive(false);
		});
		trigger.triggers.Add (entry);

		itemInfo.Init ();
		playerInfo.Init ();
		gameObject.SetActive (false);
	}
	public void OnEnable()
	{
		DungeonMain.Instance.state = DungeonMain.State.Popup;
		itemInfo.gameObject.SetActive (false);
	}
	public void OnDisable()
	{
		TurnEquipGuideArrowOff ();
		for (int i = 0; i < inventorySlots.Length; i++) {
			inventorySlots [i].outline.outline = false;
		}
		for (int i = 0; i < 7; i++) {
			equipmentSlots [i].outline.outline = false;
		}
		DungeonMain.Instance.state = DungeonMain.State.Idle;
	}
	public Inventory.Slot Put(Item item)
	{
		Inventory.Slot slot = Player.Instance.inventory.Put (item);
		if (null == slot)
		{
			return null;
		}
		inventorySlots [slot.index].Init (slot);
		return slot;
	}
	public Item Pull(int index)
	{
		Item item = Player.Instance.inventory.Pull (index);
		if (null == item) {
			return null;
		}
		inventorySlots [index].Init (null);
		return item;
	}
	public void TurnEquipGuideArrowOn(EquipmentItem.Part part, int index = -1)
	{
		for (int i = 0; i < equipmentSlots.Length; i++) {
			UIEquipmentSlot other = equipmentSlots [i];
			if (part == other.part && index != other.index) {
				other.arrow.gameObject.SetActive (true);
				iTween.MoveBy(other.arrow.gameObject, 
					iTween.Hash("y", 20, "easeType", "linear", "loopType", "pingPong", "delay", 0.0f, "time", 0.5f)
				);
			} else {
				iTween.Stop (other.arrow.gameObject);
                other.arrow.transform.localPosition = Vector3.zero;
                other.arrow.gameObject.SetActive (false);
			}
		}
	}
	public void TurnEquipGuideArrowOff()
	{
		for (int i = 0; i < equipmentSlots.Length; i++) {
			GameObject arrow = equipmentSlots [i].arrow.gameObject;
			iTween.Stop (arrow);
			arrow.transform.localPosition = Vector3.zero;
			arrow.SetActive (false);
		}
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
}
