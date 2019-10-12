using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ItemManager : Util.Singleton<ItemManager>
{
	private Dictionary<string, Item.Meta> metas = new Dictionary<string, Item.Meta>();
	private EquipItemManager equip_item_manager = new EquipItemManager();
	private ExpendableItemManager expendable_item_manager = new ExpendableItemManager();
	public void Init()
	{
		equip_item_manager.Init();
		expendable_item_manager.Init();
		InitKeyItemInfo();
	}

	public void AddItemMeta(Item.Meta meta)
	{
		metas.Add(meta.id, meta);
	}

	public Item CreateItem(string id)
	{
		Item item = FindMeta<Item.Meta>(id).CreateInstance();
		Analytics.CustomEvent("CreateItem", new Dictionary<string, object>
		{
			{"id", item.meta.id },
			{"type", item.meta.type.ToString()},
			{"name", item.meta.name },
			{"grade", item.grade.ToString()}
		});
		return item;
	}

	public EquipItem CreateRandomEquipItem(int level)
	{
		return equip_item_manager.CreateRandomItem(level);
	}

	public Item CreateRandomExpendableItem()
	{
		return expendable_item_manager.CreateRandomItem();
	}

	public T FindMeta<T>(string id) where T : Item.Meta
	{
		if (false == metas.ContainsKey(id))
		{
			throw new System.Exception("can't find item meta(id:" + id + ")");
		}
		return metas[id] as T;
	}

	private void InitKeyItemInfo()
	{
		{
			KeyItem.Meta meta = new KeyItem.Meta();
			meta.type = Item.Type.Key;
			meta.id = "ITEM_KEY";
			meta.name = "Key";
			meta.price = 0;
			meta.sprite_path = "Item/item_key";
			meta.description = "Very important for going to next dungeon levell";
			metas.Add(meta.id, meta);
		}
	}
}