using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemStatTrigger
{
	public abstract bool IsAvailable (Player.Stat stat);
}

public class ItemStatTrigger_LowHealth : ItemStatTrigger
{
	float percent;
	public ItemStatTrigger_LowHealth(float percent)
	{
		this.percent = percent;
	}
	public override bool IsAvailable(Player.Stat stat)
	{
		if (stat.curHealth <= stat.maxHealth * percent) {
			return true;
		}
		return false;
	}
}




public abstract class Item {
	public enum Type {
		Equipment,
		Potion,
		Key
	}
	public enum Grade {
		Low,
		Normal,
		High,
		Magic,
		Rare,
		Legendary,
		Max
	}

	public string 	id;
	public string 	name;
	public Sprite 	icon;
	public Type 	type;
	public Grade 	grade;	
	public int 		price;
	public int		level;
	public abstract Item CreateInstance();
	public List<string> Actions() {
		List<string> actions = new List<string> ();
		actions.Add ("DROP");
		return actions;
	}

	public void Pickup()
	{
		Player.Instance.inventory.Put (this);
	}
}
	

	
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
		/*
		dictItemInfo = new Dictionary<string, ItemInfo>();
		TextAsset resource = Resources.Load ("Config/ItemInfo") as TextAsset;
		JSONNode root = JSON.Parse (resource.text);

		InitRingItemInfo(root);
		InitShieldItemInfo (root);
		InitShirtItemInfo (root);

		InitPotionItemInfo(root);

		Debug.Log ("init complete ItemManager");
		*/
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
	/*
	private void InitShieldItemInfo(JSONNode root)
	{
		JSONNode itemInfos = root ["shield"];
		for (int i=0; i<itemInfos.Count; i++) {
			JSONNode jsonInfo = itemInfos[i];
			ShieldItemInfo itemInfo = new ShieldItemInfo();
			itemInfo.id = jsonInfo["id"];
			itemInfo.name = jsonInfo["name"];
			itemInfo.cost = jsonInfo["cost"].AsInt;
			itemInfo.weight = jsonInfo["weight"].AsInt;
			itemInfo.description = jsonInfo["description"];
			itemInfo.defense = jsonInfo["defense"].AsInt;
			itemInfo.speed = jsonInfo["speed"].AsInt;
			dictItemInfo.Add (itemInfo.id, itemInfo);
		}
	}
	*/
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
	}
	/*
	private delegate BuffInfo InitBuffInfo(JSONNode attr);
	private void InitPotionItemInfo(JSONNode root)
	{
		Dictionary<string, InitBuffInfo> buffInfo = new Dictionary<string, InitBuffInfo> ();
		buffInfo ["heal"] = (JSONNode attr) => { 
			HealBuffInfo info = new HealBuffInfo ();
			info.name = attr ["name"];
			info.amount = attr ["amount"].AsInt;
			return info;
		};
		buffInfo ["stamina"] = (JSONNode attr) => { 
			StaminaBuffInfo info = new StaminaBuffInfo ();
			info.name = attr ["name"];
			info.amount = attr ["amount"].AsInt;
			return info;
		};
		buffInfo ["attack"] = (JSONNode attr) => { 
			AttackBuffInfo info = new AttackBuffInfo();
			info.name = attr ["name"];
			info.attack = attr["attack"].AsInt;
			info.turn = attr["turn"].AsInt;
			return info;
		};
		buffInfo ["poison"] = (JSONNode attr) => { 
			PoisonBuffInfo info = new PoisonBuffInfo();
			info.name = attr ["name"];
			info.damage.SetValue(attr["damage"]);
			info.turn = attr["turn"].AsInt;
			return info;
		};
		JSONNode itemInfos = root ["potion"];
		for (int i=0; i<itemInfos.Count; i++) {
			JSONNode jsonInfo = itemInfos[i];
			PotionItemInfo itemInfo = new PotionItemInfo();
			itemInfo.id = jsonInfo["id"];	
			itemInfo.name = jsonInfo["name"];
			itemInfo.cost = jsonInfo["cost"].AsInt;
			itemInfo.weight = jsonInfo["weight"].AsInt;
			itemInfo.description = jsonInfo["description"];
			JSONNode jsonBuff = jsonInfo["buff"];
			itemInfo.buff.Add (buffInfo[jsonBuff["type"]](jsonBuff));
			dictItemInfo.Add (itemInfo.id, itemInfo);
		}
	}
	*/
}