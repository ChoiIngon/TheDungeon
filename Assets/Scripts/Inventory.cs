using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
	[System.Serializable]
	public class Slot {
		public int index;
		public int count;
		public ItemData item;
	}
	public const int MAX_SLOT_COUNT = 15;
	public int gold;
	public Slot[] slots;
	public UIInventory ui;

	public void Init()
	{
		slots = new Slot[MAX_SLOT_COUNT];
		for (int i = 0; i < MAX_SLOT_COUNT; i++) {
			slots [i] = new Slot ();
			slots [i].index = i;
		}
		ui.Init ();
	}
	public bool Put(ItemData data)
	{
		if (null == data) {
			return true;
		}
		/*
		switch (data.info.category) {
		case ItemInfo.Category.Key :
		case ItemInfo.Category.Potion:
			foreach(ItemData item in items)
			{
				if(null == item) {
					continue;
				}
				if(item.info.id == data.info.id)
				{
					item.count += data.count;
					return;
				}
			}
			break;
		default :
			break;
		}
		*/

		for(int i=0;i<slots.Length; i++)
		{
			Slot slot = slots[i];
			if(null == slot.item)
			{
				slot.item = data;
				slot.count = 1;
				ui.ActivateInventorySlot (slot.index, true);
				return true;
			}
		}
		return false;
	}
	public ItemData Pull(int index)
	{
		Slot slot = slots [index];
		if (null == slot.item) {
			throw new System.Exception("no item");
		}

		slot.count -= 1;
		ItemData item = slot.item;
		if (0 == slot.count) {
			slot.item = null;
			ui.ActivateInventorySlot (slot.index, false);
		}
		return item;
	}

    public List<ItemData> Get(ItemInfo.Category category) 
    {
        List<ItemData> datas = new List<ItemData>();
        foreach (Slot slot in slots)
        {
            if(null == slot.item)
            {
                continue;
            }
            if(category != slot.item.info.category)
            {
                continue;
            }
            datas.Add(slot.item);
        }

        return datas;
    }

}