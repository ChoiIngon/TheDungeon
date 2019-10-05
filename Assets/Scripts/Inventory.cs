using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory
{
	public const int MAX_SLOT_COUNT = 15;
	public Item[] items = new Item[MAX_SLOT_COUNT];
	public int count
	{
		get
		{
			return _count;
		}
	}
	private int _count = 0;
	
	public void Add(Item item)
	{
		if (MAX_SLOT_COUNT <= _count)
		{
			throw new System.Exception("inventory count over");
		}

		if (null == item)
		{
			return;
		}

		for(int i=0;i<items.Length; i++)
		{
			if(null == items[i])
			{
				item.slot_index = i;
				items[i] = item;
				_count += 1;
				Util.EventSystem.Publish<Item>(EventID.Inventory_Add, item);
				return;
			}
		}
	}

	public void Add(Item item, int slotIndex)
	{
		if (MAX_SLOT_COUNT <= _count)
		{
			throw new System.Exception("inventory count over");
		}

		if (0 > slotIndex || MAX_SLOT_COUNT <= slotIndex)
		{
			throw new System.Exception("invalid inventory index(index:" + slotIndex + ")");
		}

		if (null == item)
		{
			return;
		}

		if (null != items[slotIndex])
		{
			throw new System.Exception("duplicated inventory slot index(index:" + slotIndex + ")");
		}

		item.slot_index = slotIndex;
		items[slotIndex] = item;
		_count += 1;
		Util.EventSystem.Publish<Item>(EventID.Inventory_Add, item);
	}

	public Item Remove(int index)
	{
        if(0 > index || MAX_SLOT_COUNT <= index)
		{
			throw new System.Exception("invalid inventory index(index:" + index + ")");
		}

		if (null == items[index])
		{
			return null;
		}

		Item item = items[index];
        Util.EventSystem.Publish<Item>(EventID.Inventory_Remove, item);

        item.slot_index = -1;
		items[index] = null;

        _count -= 1;
		return item;
	}
	public void Swap(int from, int to)
	{
		if (0 > from || MAX_SLOT_COUNT <= from)
		{
			throw new System.Exception("out of range inventory index(index:" + from + ")");
		}

        if (0 > to || MAX_SLOT_COUNT <= to)
		{
			throw new System.Exception("out of range inventory index(index:" + to + ")");
		}

		Item a = Remove (from);
		Item b = Remove(to);

        if (null != a)
        {
            a.slot_index = to;
            items[to] = a;
            Util.EventSystem.Publish<Item>(EventID.Inventory_Add, a);
        }
        if (null != b)
		{
			b.slot_index = from;
			items[from] = b;
			Util.EventSystem.Publish<Item>(EventID.Inventory_Add, b);
		}
	}
	public T GetItem<T>(int index) where T : Item
	{
		if (0 >= index || MAX_SLOT_COUNT <= index)
		{
			throw new System.Exception("out of range inventory index(index:" + index + ")");
		}
		return items[index] as T;
	}
    /*
	public List<T> GetItems<T>() where T : Item
	{
		List<T> items = new List<T> ();
		for (int i = 0; i < MAX_SLOT_COUNT; i++)
		{
			if (null == items[i])
			{
				continue;
			}
			T item = items[i] as T;
			if (null == item)
			{
				continue;
			}
			items.Add (item);
		}
		return items;
	}
    */
}