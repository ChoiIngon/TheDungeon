using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ItemManager : Util.Singleton<ItemManager>
{
	private List<Item.Type> item_type_gacha = new List<Item.Type>()
	{
		Item.Type.Equipment,
		Item.Type.Expendable
	};
	private Dictionary<string, Item.Meta> metas = new Dictionary<string, Item.Meta>();
	private ExpendableItemManager expendable_item_manager = new ExpendableItemManager();
	public void Init()
	{
		EquipItemManager.Instance.Init();
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

		ProgressManager.Instance.Update(ProgressType.CollectItem, "", 1);
		return item;
	}

	public EquipItem CreateRandomEquipItem()
	{
		ProgressManager.Instance.Update(ProgressType.CollectItem, "", 1);
		return EquipItemManager.Instance.CreateRandomItem();
	}

	public ExpendableItem CreateRandomExpendableItem()
	{
		ProgressManager.Instance.Update(ProgressType.CollectItem, "", 1);
		return expendable_item_manager.CreateRandomItem();
	}

	public Item CreateRandomItem()
	{
		ProgressManager.Instance.Update(ProgressType.CollectItem, "", 1);
		Item.Type itemType = item_type_gacha[Random.Range(0, item_type_gacha.Count)];
		switch (itemType)
		{
			case Item.Type.Equipment:
				return CreateRandomEquipItem();
			case Item.Type.Expendable:
				return CreateRandomExpendableItem();
		}
		return null;
	}

	public T FindMeta<T>(string id) where T : Item.Meta
	{
		if (false == metas.ContainsKey(id))
		{
			throw new System.Exception("can't find item meta(id:" + id + ")");
		}
		return metas[id] as T;
	}

	public ExpendableItem.Meta GetRandomExpendableItemMeta()
	{
		if (0 == expendable_item_manager.item_metas.Count)
		{
			throw new System.Exception("no potion item info");
		}
		return expendable_item_manager.item_metas[Random.Range(0, expendable_item_manager.item_metas.Count)];
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