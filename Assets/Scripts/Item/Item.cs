using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemAttribute
{
	public enum Type {
		Attack
	}
	public Type type;
	public float min;
	public float max;
	public string description;
}
	
public abstract class ItemInfo {
	public const int MAX_EQUIPMEMT_CATEGORY = 5;
	public enum Category {
		Helmet,
		Hand,
		Armor,
		Ring,
		Shoes,
		Potion,
		Key,
		All
	}
	public enum Grade
	{
		Low,
		Normal,
		High,
		Magic,
		Rare,
		Legendary,
		All
	}
	public string id;
	public string name;

	public int price;
	public Sprite icon;
	public Category category;
	public Grade grade;
	public ItemAttribute 		mainAttribute;
	public List<ItemAttribute> 	secondaryAttributes = new List<ItemAttribute>();
	public abstract ItemData CreateInstance();
}

public abstract class ItemData {
	public ItemInfo info;
	public int count;
	public int seq;

	/*
	public virtual Character.Status Use (Character character) {
		throw new System.Exception ("the item can not be used");
	}
	*/
}

[System.Serializable]
public abstract class EquipmentItemData : ItemData {
	/*
	public Character.EquipPart part;
	public abstract Character.Status GetStatus();
	*/
}

public class ItemManager {
	private Dictionary<string, ItemInfo> infos;
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
	public ItemData CreateRandomItem(ItemInfo.Category category, ItemInfo.Grade grade)
	{
		return null;
	}
	public ItemData CreateItem(string id) {
		return FindInfo<ItemInfo> (id).CreateInstance ();
	}

	public T FindInfo<T>(string id) where T:ItemInfo
	{
		if (false == infos.ContainsKey (id)) {
			throw new System.Exception ("can't find item info(id:" + id + ")");
		}
		return infos[id] as T;
	}

	public void Init() {
		infos = new Dictionary<string, ItemInfo> ();
		InitWeaponItemInfo ();
		/*
		dictItemInfo = new Dictionary<string, ItemInfo>();
		TextAsset resource = Resources.Load ("Config/ItemInfo") as TextAsset;
		JSONNode root = JSON.Parse (resource.text);

		InitRingItemInfo(root);
		InitShieldItemInfo (root);
		InitShirtItemInfo (root);
		InitWeaponItemInfo(root);

		InitPotionItemInfo(root);

		Debug.Log ("init complete ItemManager");
		*/
	}
	/*
	private void InitRingItemInfo(JSONNode root)
	{
		JSONNode itemInfos = root ["ring"];
		for (int i=0; i<itemInfos.Count; i++) {
			JSONNode jsonInfo = itemInfos[i];
			RingItemInfo itemInfo = new RingItemInfo();
			itemInfo.id = jsonInfo["id"];
			itemInfo.name = jsonInfo["name"];
			itemInfo.cost = jsonInfo["cost"].AsInt;
			itemInfo.weight = jsonInfo["weight"].AsInt;
			itemInfo.description = jsonInfo["description"];
			dictItemInfo.Add (itemInfo.id, itemInfo);
		}
	}
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
	private void InitShirtItemInfo(JSONNode root)
	{
		JSONNode itemInfos = root ["shirt"];
		for (int i=0; i<itemInfos.Count; i++) {
			JSONNode jsonInfo = itemInfos[i];
			ShirtItemInfo itemInfo = new ShirtItemInfo();
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
	private void InitWeaponItemInfo()
	{
		for(int i=0; i<10; i++)
		{
			WeaponItemInfo info = new WeaponItemInfo ();
			info.name = "Weapon_" + i.ToString ();
			info.price = 100;
			info.grade = (ItemInfo.Grade)Random.Range ((int)ItemInfo.Grade.Low, (int)ItemInfo.Grade.All);
			info.attack = Random.Range (1, 10);
			info.icon = ResourceManager.Instance.Load<Sprite> ("item_sword_003");
			info.id = "ITEM_WEAPON_" + i.ToString ();
			info.twoHanded = 0 == Random.Range (0, 2) ? true : false;
			infos.Add (info.id, info);
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