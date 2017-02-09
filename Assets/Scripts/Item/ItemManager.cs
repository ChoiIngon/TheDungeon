using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager {
	private Dictionary<string, Item> items;
	private static ItemManager _instance;  
	public static ItemManager Instance  
	{  
		get  
		{  
			if (null == _instance) 
			{  
				_instance = new ItemManager ();    
			}  
			return _instance;  
		}  
	}
	public EquipmentItem CreateRandomItem(EquipmentItem.Grade grade)
	{
		return null;
	}
	public Item CreateItem(string id) {
		return FindInfo<Item> (id).CreateInstance ();
	}

	public T FindInfo<T>(string id) where T:Item
	{
		if (false == items.ContainsKey (id)) {
			throw new System.Exception ("can't find item info(id:" + id + ")");
		}
		return items[id] as T;
	}

	public void Init() {
		items = new Dictionary<string, Item> ();
		InitWeaponItemInfo ();
		InitArmorItemInfo ();
		InitRingItemInfo ();
		InitPotionItemInfo ();
	}

	private void InitRingItemInfo()
	{
		for(int i=0; i<10; i++)
		{
			EquipmentItem item = new EquipmentItem ();
			item.id = "ITEM_RING_" + i.ToString ();
			item.name = "Ring" + i.ToString ();
			item.price = 100;
			item.grade = (EquipmentItem.Grade)Random.Range ((int)EquipmentItem.Grade.Low, (int)EquipmentItem.Grade.Max);
			item.icon = ResourceManager.Instance.Load<Sprite> ("item_ring_00" + (i % 3 +1).ToString());
			item.part = EquipmentItem.Part.Ring;
			item.mainStat = new ItemStat_Critical( (i+1)/100, "CRI : +" + (i + 1) + "%");
			item.subStats.Add(new ItemStat_CoinBonus((i+1)/100, "gold bonus : +" + (i+1) +"%"));
			items.Add (item.id, item);
		}
	}

	private void InitArmorItemInfo()
	{
		for(int i=0; i<10; i++)
		{
			EquipmentItem item = new EquipmentItem ();
			item.id = "ITEM_ARMOR_" + i.ToString ();
			item.name = "Armor_" + i.ToString ();
			item.price = 100;
			item.grade = (EquipmentItem.Grade)Random.Range ((int)EquipmentItem.Grade.Low, (int)EquipmentItem.Grade.Max);
			item.icon = ResourceManager.Instance.Load<Sprite> ("item_shirt_00" + (i % 3 +1).ToString());
			item.part = EquipmentItem.Part.Armor;
			item.mainStat = new ItemStat_Defense ( i + 1, "DEF : +" + (i + 1));
			items.Add (item.id, item);
		}
	}

	private void InitWeaponItemInfo()
	{
		for(int i=0; i<10; i++)
		{
			EquipmentItem item = new EquipmentItem ();
			item.id = "ITEM_WEAPON_" + i.ToString ();
			item.name = "Weapon_" + i.ToString ();
			item.price = 100;
			item.grade = (EquipmentItem.Grade)Random.Range ((int)EquipmentItem.Grade.Low, (int)EquipmentItem.Grade.Max);
			item.icon = ResourceManager.Instance.Load<Sprite> ("item_sword_00" + (i % 3 +1).ToString());
			item.part = EquipmentItem.Part.Hand;
			item.mainStat = new ItemStat_Attack(i+1, "ATK : +" + (i + 1));
			items.Add (item.id, item);
		}
		for(int i=0; i<10; i++)
		{
			EquipmentItem item = new EquipmentItem ();
			item.id = "ITEM_SHIELD_" + i.ToString ();
			item.name = "Shield " + i.ToString ();
			item.price = 100;
			item.grade = (EquipmentItem.Grade)Random.Range ((int)EquipmentItem.Grade.Low, (int)EquipmentItem.Grade.Max);
			item.icon = ResourceManager.Instance.Load<Sprite> ("item_shield_00" + (i % 3 +1).ToString());
			item.part = EquipmentItem.Part.Hand;
			item.mainStat = new ItemStat_Defense(i+1, "DEF : +" + (i + 1));
			items.Add (item.id, item);
		}
	}
	private void InitPotionItemInfo()
	{
		{
			HealingPotionItem item = new HealingPotionItem ();
			item.id = "ITEM_POTION_HEALING";
			item.name = "Healing Potion";
			item.price = 100;
			item.grade = Item.Grade.Normal;
			item.icon = ResourceManager.Instance.Load<Sprite> ("item_potion_002");
			items.Add (item.id, item);
		}
		{
			PoisonPotionItem item = new PoisonPotionItem ();
			item.id = "ITEM_POTION_POISON";
			item.name = "Poison Potion";
			item.price = 100;
			item.grade = Item.Grade.Normal;
			item.icon = ResourceManager.Instance.Load<Sprite> ("item_potion_003");
			items.Add (item.id, item);
		}
	}
}