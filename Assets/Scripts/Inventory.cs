using UnityEngine;
using System.Collections;

public class Inventory
{
	public const int MAX_SLOT_COUNT = 15;
	public int gold;
	public ItemData[] items = new ItemData[MAX_SLOT_COUNT];
	public UIInventory ui;

	public void Put(ItemData data)
	{
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

		for(int i=0;i<items.Length; i++)
		{
			ItemData item = items[i];
			if(null == item)
			{
				items [i] = data;
				return;
			}
		}
		throw new System.Exception ("no more room in inventory");
	}

	public ItemData Pull(int index)
	{
		ItemData item = items [index];
		if (null == item) {
			throw new System.Exception("no item");
		}

		item.count -= 1;
		if (0 == item.count) {
			items [index] = null;
		}
		return item;
	}
}