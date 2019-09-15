using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    public UIPlayerInfo player_info;
    public UIItemInfo item_info;

	public List<UIItemSlot> slots = new List<UIItemSlot>();
	public UIItemSlot[] inventory_slots = new UIItemSlot[Inventory.MAX_SLOT_COUNT];
	public Dictionary<Tuple<EquipItem.Part, int>, UIEquipSlot> equip_slots = new Dictionary<Tuple<EquipItem.Part, int>, UIEquipSlot>();
	public Button close;

	public UIItem[] item_prefabs = new UIItem[(int)Item.Type.Max];
    public void Init()
    {
		item_prefabs[(int)Item.Type.Equipment] = UIUtil.FindChild<UIEquipItem>(transform, "ItemPrefabs/EquipItem");
		item_prefabs[(int)Item.Type.Potion] = UIUtil.FindChild<UIPotionItem>(transform, "ItemPrefabs/PotionItem");

		gameObject.SetActive(false);
		
		Transform inventorySlots = UIUtil.FindChild<Transform>(transform, "ItemSlots");
		for (int i = 0; i < Inventory.MAX_SLOT_COUNT; i++)
        {
			UIItemSlot slot = inventorySlots.GetChild(i).GetComponent<UIItemSlot>();
            slot.slot_index = i;

            inventory_slots[i] = slot;
            slots.Add(slot);
        }

        Transform equipSlots = UIUtil.FindChild<Transform>(transform, "EquipSlots");
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Helmet, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Helmet"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Hand_0"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 1), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Hand_1"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Armor, 1), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Armor"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Ring_0"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 1), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Ring_1"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Shoes, 1), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Shoes"));
		foreach (var itr in equip_slots)
		{
			itr.Value.part = itr.Key.first;
			itr.Value.equip_index = itr.Key.second;
			slots.Add(itr.Value);
		}
		
        close = UIUtil.FindChild<Button>(transform, "Close");
        UIUtil.AddPointerUpListener(close.gameObject, () => { SetActive(false); });
        
        item_info = UIUtil.FindChild<UIItemInfo>(transform, "ItemInfo");
        item_info.Init();
        item_info.gameObject.SetActive(false);

        player_info = UIUtil.FindChild<UIPlayerInfo>(transform, "PlayerInfo");
        player_info.Init();

		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Add, OnItemAdd);
        Util.EventSystem.Subscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
        Util.EventSystem.Subscribe<UIItem>(EventID.Inventory_Slot_Select, OnSlotSelectNotify);
        Util.EventSystem.Subscribe<UIItem>(EventID.Inventory_Slot_Release, OnSlotReleaseNotify);

        Debug.Log("init complete UIInventory");
    }

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Unsubscribe<UIItem>(EventID.Inventory_Slot_Select, OnSlotSelectNotify);
		Util.EventSystem.Unsubscribe<UIItem>(EventID.Inventory_Slot_Release, OnSlotReleaseNotify);
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
	
    private void OnItemAdd(Item itemData)
    {
		Debug.Log("OnItemAdd(item_id:" + itemData.meta.id + ", item_seq:" + itemData.item_seq + ", slot_index:" + itemData.slot_index + ")");
//		inventory_slots[item.slot_index].Init(item);

		UIItem item = GameObject.Instantiate<UIItem>(item_prefabs[(int)itemData.meta.type]);
		if (null == item)
		{
			throw new System.Exception("can not instantiate slot object for item(item_id:" + itemData.meta.id + ", name:" + itemData.meta.name + ")");
		}

		item.inventory = this;
		item.gameObject.SetActive(true);
		item.Init(itemData);
		inventory_slots[itemData.slot_index].SetItem(item);
    }

    private void OnItemRemove(Item itemData)
    {
        Debug.Log("OnItemRemove(item_id:" + itemData.meta.id + ", item_seq:" + itemData.item_seq + ", slot_index:" + itemData.slot_index + ")");
		inventory_slots[itemData.slot_index].SetItem(null);

    }

    private void OnItemEquip(ItemEquipEvent evt)
    {
        player_info.Refresh();
        var key = new Tuple<EquipItem.Part, int>(evt.item.part, evt.equip_index);
        if (false == equip_slots.ContainsKey(key))
        {
            throw new System.Exception("invalid item equip slot(equip_part:" + evt.item.part + ", equip_index:" + evt.item.equip_index +")");
        }

		UIItem item = GameObject.Instantiate<UIItem>(item_prefabs[(int)evt.item.meta.type]);
		if (null == item)
		{
			throw new System.Exception("can not instantiate slot object for item(item_id:" + evt.item.meta.id + ", name:" + evt.item.meta.name + ")");
		}
		item.gameObject.SetActive(true);
		item.Init(evt.item);

		equip_slots[key].SetItem(item);
	}

	private void OnItemUnequip(ItemEquipEvent evt)
    {
        player_info.Refresh();
        var key = new Tuple<EquipItem.Part, int>(evt.item.part, evt.equip_index);
        if (false == equip_slots.ContainsKey(key))
        {
            throw new System.Exception("invalid item equip slot(equip_part:" + evt.item.part + ", equip_index:" + evt.item.equip_index + ")");
        }

		equip_slots[key].SetItem(null);
    }

    private void OnSlotSelectNotify(UIItem slot)
    {
        item_info.slot = slot;

        item_info.description.text = slot.item_data.description;

        item_info.buttons[(int)UIItemInfo.Action.Drop].gameObject.SetActive(true);
        item_info.actions[(int)UIItemInfo.Action.Drop] += () => {
            GameManager.Instance.player.inventory.Remove(slot.item_data.slot_index);
            item_info.actions[(int)UIItemInfo.Action.Drop] = null;
            item_info.gameObject.SetActive(false);
        };

		switch (slot.item_data.meta.type)
		{
			case Item.Type.Potion:
				item_info.buttons[(int)UIItemInfo.Action.Drink].gameObject.SetActive(true);
				item_info.actions[(int)UIItemInfo.Action.Drink] += () =>
				{
					PotionItem potionItem = GameManager.Instance.player.inventory.Remove(slot.item_data.slot_index) as PotionItem;
					if (null == potionItem)
					{
						throw new System.InvalidCastException();
					}
					potionItem.Drink(GameManager.Instance.player);
					item_info.actions[(int)UIItemInfo.Action.Drink] = null;
					item_info.gameObject.SetActive(false);
				};
				break;
		}
	}

    private void OnSlotReleaseNotify(UIItem slot)
    {
    }
}
