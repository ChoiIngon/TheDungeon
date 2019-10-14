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
	private void Awake()
	{
		item_prefabs[(int)Item.Type.Equipment] = UIUtil.FindChild<UIEquipItem>(transform, "ItemPrefabs/EquipItem");
		item_prefabs[(int)Item.Type.Expendable] = UIUtil.FindChild<UIPotionItem>(transform, "ItemPrefabs/PotionItem");
		item_prefabs[(int)Item.Type.Key] = UIUtil.FindChild<UIKeyItem>(transform, "ItemPrefabs/KeyItem");

		close = UIUtil.FindChild<Button>(transform, "Close");
		item_info = UIUtil.FindChild<UIItemInfo>(transform, "ItemInfo");
		player_info = UIUtil.FindChild<UIPlayerInfo>(transform, "PlayerInfo");
	}

	public void Init()
    {
		GridLayoutGroup inventorySlots = UIUtil.FindChild<GridLayoutGroup>(transform, "ItemSlots");
		for (int i = 0; i < Inventory.MAX_SLOT_COUNT; i++)
		{
			UIItemSlot slot = inventorySlots.transform.GetChild(i).GetComponent<UIItemSlot>();
			slot.inventory = this;
			slot.slot_index = i;
			slot.rectTransform.sizeDelta = inventorySlots.cellSize;
			inventory_slots[i] = slot;
			
			slots.Add(slot);
		}
		Transform equipSlots = UIUtil.FindChild<Transform>(transform, "EquipSlots");
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Helmet, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Helmet"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Hand_0"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 1), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Hand_1"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Armor, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Armor"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Ring_0"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 1), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Ring_1"));
		equip_slots.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Shoes, 0), UIUtil.FindChild<UIEquipSlot>(equipSlots, "Shoes"));
		foreach (var itr in equip_slots)
		{
			itr.Value.inventory = this;
			itr.Value.part = itr.Key.first;
			itr.Value.equip_index = itr.Key.second;
			itr.Value.slot_index = -1;
			slots.Add(itr.Value);
		}
		UIUtil.AddPointerUpListener(close.gameObject, () => { SetActive(false); });

		item_info.Init();
        player_info.Init();
		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Add, OnItemAdd);
        Util.EventSystem.Subscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
		Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
		Debug.Log("init complete UIInventory");
    }

	public void Clear()
	{
		foreach (var slot in slots)
		{
			slot.SetItem(null);
		}
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
		Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
	}

	public void SetActive(bool flag)
    {
        gameObject.SetActive(flag);
        if(true == flag)
        {
			player_info.Refresh();
            Util.EventSystem.Subscribe(EventID.Player_Stat_Change, OnPlayerStatChange);
			Util.EventSystem.Publish(EventID.Inventory_Open);
        }
        else
        {
            Util.EventSystem.Unsubscribe(EventID.Player_Stat_Change, OnPlayerStatChange);
			Util.EventSystem.Publish(EventID.Inventory_Close);
		}
    }
	
    private void OnItemAdd(Item itemData)
    {
		UIItem item = GameObject.Instantiate<UIItem>(item_prefabs[(int)itemData.meta.type]);
		if (null == item)
		{
			throw new System.Exception("can not instantiate slot object for item(item_id:" + itemData.meta.id + ", name:" + itemData.meta.name + ")");
		}

		item.gameObject.SetActive(true);
		item.Init(itemData);
		inventory_slots[itemData.slot_index].SetItem(item);
    }

    private void OnItemRemove(Item itemData)
    {
		Object.Destroy(inventory_slots[itemData.slot_index].item.gameObject);
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
		item.outline.active = true;
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
		Object.Destroy(equip_slots[key].item.gameObject);
		equip_slots[key].SetItem(null);
    }

	private void OnPlayerStatChange()
	{
		player_info.Refresh();
	}
}
