using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ItemStat
{
	public enum Type {
		Attack,
		Defense,
		Speed,
		Health
	}
	public Type type;
	public string description;
	public abstract float GetValue ();
}

public class ConstItemStat : ItemStat
{
	public ConstItemStat(ItemStat.Type type, float value, string description)
	{
		this.type = type;
		this.value = value;
		this.description = description;
	}
	public float value;
	public override float GetValue() {
		return value;
	}
}

public class RandomItemStat : ItemStat
{
	public float min;
	public float max;
	public RandomItemStat(ItemStat.Type type, float min, float max, string description)
	{
		this.type = type;
		this.min = min;
		this.max = max;
		this.description = description;
	}
	public override float GetValue() {
		return Random.Range (min, max);
	}
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
	public ItemStat 		mainStat;
	public List<ItemStat> 	subStat = new List<ItemStat>();
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
		InitArmorItemInfo ();
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
	*/
	private void InitArmorItemInfo()
	{
		for(int i=0; i<10; i++)
		{
			ArmorItemInfo info = new ArmorItemInfo ();
			info.name = "Armor_" + i.ToString ();
			info.price = 100;
			info.grade = (ItemInfo.Grade)Random.Range ((int)ItemInfo.Grade.Low, (int)ItemInfo.Grade.All);
			info.icon = ResourceManager.Instance.Load<Sprite> ("item_shirt_003");
			info.id = "ITEM_ARMOR_" + i.ToString ();
			infos.Add (info.id, info);
		}
	}

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
			info.mainStat = new RandomItemStat (ItemStat.Type.Attack, i + 1, i + 14, "ATK : " + (i + 1) + " ~ " + (i + 14));
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