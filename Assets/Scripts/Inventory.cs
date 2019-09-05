﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
	[System.Serializable]
	public class Slot {
		public int index;
		public int count;
		public Item item;
		public void Drop()
		{
		}
	}
	public const int MAX_SLOT_COUNT = 15;
	public Slot[] slots;
	
	public int count {
		get {
			return _count;
		}
	}
	private int _count;
	public void Init()
	{
		_count = 0;
		slots = new Slot[MAX_SLOT_COUNT];
		for (int i = 0; i < MAX_SLOT_COUNT; i++) {
			slots [i] = new Slot ();
			slots [i].index = i;
		}
	}
	public Slot Put(Item data)
	{
		if (null == data) {
			return null;
		}

		for(int i=0;i<slots.Length; i++)
		{
			Slot slot = slots[i];
			if(null == slot.item)
			{
				slot.item = data;
				slot.count = 1;
				_count += 1;

				Util.EventSystem.Publish(EventID.Inventory_Put, slot);
				return slot;
			}
		}
		return null;
	}
	public Item Pull(int index)
	{
		Slot slot = slots [index];
		if (null == slot.item) {
			return null;
		}

		slot.count -= 1;
		Item item = slot.item;
		if (0 == slot.count) {
			slot.item = null;
			//ui.Pull (slot.index);
			_count -= 1;
		}
		Util.EventSystem.Publish(EventID.Inventory_Pull, slot);
		return item;
	}
	public void Swap(int from, int to)
	{
		Item a = Pull (from);
		Item b = Pull (to);
		if (null == a) {
			return;
		}
		{
			Slot slot = slots [to];
			slot.item = a;
			slot.count = 1;
			_count += 1;
			Util.EventSystem.Publish(EventID.Inventory_Put, slot);
		}
		if(null != b) {
			Slot slot = slots [from];
			slot.item = b;
			slot.count = 1;
			_count += 1;
			Util.EventSystem.Publish(EventID.Inventory_Put, slot);
		}
	}
	public T GetItem<T>(int index) where T : Item {
		Slot slot = slots [index];
		if (null == slot.item) {
			return null;
		}
		return slot.item as T;
	}
	public List<T> GetItems<T>() where T : Item {
		List<T> items = new List<T> ();
		for (int i = 0; i < MAX_SLOT_COUNT; i++) {
			Slot slot = slots [i];
			if (null == slot.item) {
				continue;
			}
			T item = slot.item as T;
			if (null == item) {
				continue;
			}
			items.Add (item);
		}
		return items;
	}
}