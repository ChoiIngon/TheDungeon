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

public abstract class ItemStat
{
	public string description;
	public ItemStatTrigger trigger;
	public abstract Player.Stat GetStat (Player.Stat stat);
}

public class ItemStat_IncreaseMaxHealth : ItemStat
{
	float percent;
	public ItemStat_IncreaseMaxHealth(float percent, string description)
	{
		this.percent = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.maxHealth = (int)(stat.maxHealth * percent);
		return result;
	}
}
public class ItemStat_IncreaseAttack : ItemStat
{
	float value;
	public ItemStat_IncreaseAttack(ItemStatTrigger trigger, float value, string description)
	{
		this.trigger = trigger;
		this.value = value;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result  = new Player.Stat();
		result.attack = value;
		return result;
	}
}
public class ItemStat_IncreaseDefense : ItemStat
{
	float value;
	public ItemStat_IncreaseDefense(ItemStatTrigger trigger, float value, string description)
	{
		this.trigger = trigger;
		this.value = value;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.defense = value;
		return result;
	}
}
public class ItemStat_IncreaseSpeed : ItemStat
{
	public float value;
	public ItemStat_IncreaseSpeed(ItemStatTrigger trigger, float value, string description)
	{
		this.trigger = trigger;
		this.value = value;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.speed = value;
		return result;
	}
}
public class ItemStat_IncreaseCritical : ItemStat
{
	public float percent;
	public ItemStat_IncreaseCritical(ItemStatTrigger trigger, float percent, string description)
	{
		this.trigger = trigger;
		this.percent = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.critcal = percent;
		return result;
	}
}
public class ItemStat_IncreaseGoldBonus : ItemStat {
	public float percent;
	public ItemStat_IncreaseGoldBonus(ItemStatTrigger trigger, float percent, string description)
	{
		this.trigger = trigger;
		this.percent = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.goldBonus = percent;
		return result;
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
	public abstract Item CreateInstance();
}
	
[System.Serializable]
public class EquipmentItem : Item {
	public enum Part {
		Helmet,
		Hand,
		Armor,
		Ring,
		Shoes
	}

	public Part		part;
	public ItemStat mainStat;
	public List<ItemStat> subStats = new List<ItemStat> ();

	public Player.Stat GetStat(Player.Stat stat)
	{
		Player.Stat result = new Player.Stat ();
		result += mainStat.GetStat (stat);
		for (int i = 0; i < subStats.Count; i++) {
			if (null != subStats [i].trigger && false == subStats[i].trigger.IsAvailable(stat)) {
				continue;
			}
			result += subStats [i].GetStat (stat);
		}
		return result;
	}
	public override Item CreateInstance()
	{
		EquipmentItem item = new EquipmentItem ();
		item.id = id;
		item.name = name;
		item.icon = icon;
		item.type = type;
		item.price = price;
		item.grade = grade;
		item.part = part;
		item.mainStat = mainStat;
		List<ItemStat> tmp = new List<ItemStat> (subStats); 
		for (int i = 0; i < ((int)grade - (int)Grade.Normal); i++) {
			if (0 == tmp.Count) {
				break;
			}
			int index = Random.Range (0, tmp.Count);
			item.subStats.Add (tmp [index]);
			tmp.RemoveAt (index);
		}
		return item;
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
			item.mainStat = new ItemStat_IncreaseCritical(null, (i+1)/100, "CRI : +" + (i + 1) + "%");
			item.subStats.Add(new ItemStat_IncreaseGoldBonus(null, (i+1)/100, "gold bonus : +" + (i+1) +"%"));
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
			item.mainStat = new ItemStat_IncreaseDefense (null, i + 1, "DEF : +" + (i + 1));
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
			item.mainStat = new ItemStat_IncreaseAttack(null, i+1, "ATK : +" + (i + 1));
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