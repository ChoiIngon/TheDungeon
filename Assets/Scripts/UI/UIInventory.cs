using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    public UIPlayerInfo player_info;
    public UIItemInfo item_info;

	public UISlotInventory[] inventory_slots = new UISlotInventory[Inventory.MAX_SLOT_COUNT];
	public Dictionary<Tuple<EquipItem.Part, int>, UISlotEquip> equip_slots = new Dictionary<Tuple<EquipItem.Part, int>, UISlotEquip>();
	public Button close;

    public void Init()
    {
		gameObject.SetActive(false);
		Transform inventorySlots = UIUtil.FindChild<Transform>(transform, "InventorySlots");
        for (int i = 0; i < Inventory.MAX_SLOT_COUNT; i++)
        {
            UISlotInventory slot = inventorySlots.GetChild(i).GetComponent<UISlotInventory>();
            slot.slot_index = i;
            inventory_slots[i] = slot;
            //slots.Add(slot);
        }

        Transform equipSlots = UIUtil.FindChild<Transform>(transform, "EquipmentSlots");
        for (int i = 0; i < 7; i++)
        {
            UISlotEquip slot = equipSlots.GetChild(i).GetComponent<UISlotEquip>();
            equip_slots.Add(new Tuple<EquipItem.Part, int>(slot.part, slot.equip_index), slot);
            //slots.Add(slot);
        }
        
        close = UIUtil.FindChild<Button>(transform, "Close");
        UIUtil.AddPointerUpListener(close.gameObject, () => { SetActive(false); });
        
        item_info = UIUtil.FindChild<UIItemInfo>(transform, "ItemInfo");
        item_info.Init();
        item_info.gameObject.SetActive(false);

        player_info = UIUtil.FindChild<UIPlayerInfo>(transform, "PlayerInfo");
        player_info.Init();

		foreach (var item in GameManager.Instance.player.inventory.items)
		{
			if (null == item)
			{
				continue;
			}
			OnItemAdd(item);
		}

		foreach (var itr in GameManager.Instance.player.equip_items)
		{
			if (null == itr.Value)
			{
				continue;
			}
			OnItemEquip(new ItemEquipEvent() { equip_index = itr.Key.second, item = itr.Value });	
		}
        Util.EventSystem.Subscribe<Item>(EventID.Inventory_Add, OnItemAdd);
        Util.EventSystem.Subscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
        Util.EventSystem.Subscribe<UISlot>(EventID.Inventory_Slot_Select, OnSlotSelectNotify);
        Util.EventSystem.Subscribe<UISlot>(EventID.Inventory_Slot_Release, OnSlotReleaseNotify);

        Debug.Log("init complete UIInventory");
    }

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Unsubscribe<UISlot>(EventID.Inventory_Slot_Select, OnSlotSelectNotify);
		Util.EventSystem.Unsubscribe<UISlot>(EventID.Inventory_Slot_Release, OnSlotReleaseNotify);
	}
	public void SetActive(bool flag)
    {
        gameObject.SetActive(flag);
        if(true == flag)
        {
            item_info.gameObject.SetActive(false);
            Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
            Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
			Util.EventSystem.Publish(EventID.Inventory_Open);
        }
        else
        {
            Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
            Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
			Util.EventSystem.Publish(EventID.Inventory_Close);
		}
    }
	
    private void OnItemAdd(Item item)
    {
        Debug.Log("OnItemAdd(item_id:" + item.meta.id + ", item_seq:" + item.item_seq + ", slot_index:" + item.slot_index + ")");
        inventory_slots[item.slot_index].Init(item);
    }

    private void OnItemRemove(Item item)
    {
        if(0 > item.slot_index || Inventory.MAX_SLOT_COUNT <= item.slot_index)
        {
            throw new System.Exception("out of range inventory index(index:" + item.slot_index + ")");
        }
        Debug.Log("OnItemRemove(item_id:" + item.meta.id + ", item_seq:" + item.item_seq + ", slot_index:" + item.slot_index + ")");

        inventory_slots[item.slot_index].Init(null);
    }

    private void OnItemEquip(ItemEquipEvent evt)
    {
        player_info.Refresh();
        var key = new Tuple<EquipItem.Part, int>(evt.item.part, evt.equip_index);
        if (false == equip_slots.ContainsKey(key))
        {
            throw new System.Exception("invalid item equip slot(equip_part:" + evt.item.part + ", equip_index:" + evt.item.equip_index +")");
        }

        UISlotEquip slot = equip_slots[key];
        slot.Init(evt.item);
    }

    private void OnItemUnequip(ItemEquipEvent evt)
    {
        player_info.Refresh();
        var key = new Tuple<EquipItem.Part, int>(evt.item.part, evt.equip_index);
        if (false == equip_slots.ContainsKey(key))
        {
            throw new System.Exception("invalid item equip slot(equip_part:" + evt.item.part + ", equip_index:" + evt.item.equip_index + ")");
        }

        UISlotEquip slot = equip_slots[key];
        slot.Init(null);
    }

    private void OnSlotSelectNotify(UISlot slot)
    {
        item_info.slot = slot;

        item_info.description.text = slot.item.description;

        item_info.buttons[(int)UIItemInfo.Action.Drop].gameObject.SetActive(true);
        item_info.actions[(int)UIItemInfo.Action.Drop] += () => {
            GameManager.Instance.player.inventory.Remove(slot.item.slot_index);
            item_info.actions[(int)UIItemInfo.Action.Drop] = null;
            item_info.gameObject.SetActive(false);
        };
    }

    private void OnSlotReleaseNotify(UISlot slot)
    {
    }
}
