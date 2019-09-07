using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIInventory : MonoBehaviour
{
    public UIPlayerInfo player_info;
    public UIItemInfo item_info;
    public UISlotInventory[] inventory_slots = new UISlotInventory[Inventory.MAX_SLOT_COUNT];
    public UISlotEquip[] equip_slots = new UISlotEquip[7];
    public Button close;

    public void Init()
    {
        Transform inventorySlots = UIUtil.FindChild<Transform>(transform, "InventorySlots");
        for (int i = 0; i < Inventory.MAX_SLOT_COUNT; i++)
        {
            UISlotInventory slot = inventorySlots.GetChild(i).GetComponent<UISlotInventory>();
            inventory_slots[i] = slot;
        }

        Transform equipSlots = UIUtil.FindChild<Transform>(transform, "EquipmentSlots");
        for (int i = 0; i < 7; i++)
        {
            UISlotEquip slot = equipSlots.GetChild(i).GetComponent<UISlotEquip>();
            equip_slots[i] = slot;
        }
        
        close = UIUtil.FindChild<Button>(transform, "Close");
        EventTrigger trigger = close.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) =>
        {
            gameObject.SetActive(false);
        });
        trigger.triggers.Add(entry);
        
        item_info = UIUtil.FindChild<UIItemInfo>(transform, "ItemInfo");
        item_info.Init();
        item_info.gameObject.SetActive(false);

        player_info = UIUtil.FindChild<UIPlayerInfo>(transform, "PlayerInfo");
        player_info.Init();

        Util.EventSystem.Subscribe<Item>(EventID.Inventory_Add, OnItemAdd);
        Util.EventSystem.Subscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
        Util.EventSystem.Subscribe<UISlot>(EventID.Inventory_Slot_Select, OnSlotSelectNotify);
        Debug.Log("init complete UIInventory");

        SetActive(true);
        /*
		gameObject.SetActive (false);
        */
    }

    public void SetActive(bool flag)
    {
        gameObject.SetActive(flag);
        if(true == flag)
        {
            item_info.gameObject.SetActive(false);
            player_info.Init();
            Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
            Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
        }
        else
        {
            Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
            Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
        }
    }
	
    public UISlot selected
    {
        set
        {
            for (int i = 0; i < inventory_slots.Length; i++)
            {
                inventory_slots[i].outline.outline = false;
            }
            for (int i = 0; i < 7; i++)
            {
                equip_slots[i].outline.outline = false;
            }
            if (null != value)
            {
                value.outline.outline = true;
            }
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

    private void OnSlotSelectNotify(UISlot slot)
    {
        item_info.slot = slot;
        
        switch (slot.item.meta.type)
        {
            case Item.Type.Equipment:
                {
                    EquipItem item = (EquipItem)slot.item;
                    foreach(Stat.Data stat in item.main_stat.GetStats())
                    {
                        Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT stat_name, description FROM meta_stat where stat_type=" + (int)stat.type);
                        while (true == reader.Read())
                        {
                            item_info.stats.text += string.Format(reader.GetString("description"), stat.value) + "\n";
                        }
                    }

                    foreach(Stat.Data stat in item.sub_stat.GetStats())
                    {
                        Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData, "SELECT stat_name, description FROM meta_stat where stat_type=" + (int)stat.type);
                        while (true == reader.Read())
                        {
                            item_info.stats.text += "<color=green> +" + string.Format(reader.GetString("description"), stat.value) + "</color>\n";
                        }
                    }
                }
                break;
            case Item.Type.Potion:
                {
                    /*
                    PotionItem item = (PotionItem)data.item;
                    inventory.itemInfo.buttons[(int)UIItemInfo.Action.Use].gameObject.SetActive(true);
                    inventory.itemInfo.actions[(int)UIItemInfo.Action.Use] += () => {
                        item.Use(DungeonMain.Instance.player);
                        inventory.Pull(data.index);
                        inventory.itemInfo.gameObject.SetActive(false);
                    };
                    */
                }
                break;
        }

        item_info.buttons[(int)UIItemInfo.Action.Drop].gameObject.SetActive(true);
        item_info.actions[(int)UIItemInfo.Action.Drop] += () => {
            GameManager.Instance.player.inventory.Remove(slot.item.slot_index);
            item_info.actions[(int)UIItemInfo.Action.Drop] = null;
            item_info.gameObject.SetActive(false);
        };
    }

    private void OnItemEquip(ItemEquipEvent evt)
    {
        player_info.Refresh();
    }

    private void OnItemUnequip(ItemEquipEvent evt)
    {
        player_info.Refresh();
    }
}
