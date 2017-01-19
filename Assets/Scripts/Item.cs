﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemAttributeInfo
{
	public enum Type {
	}
	public float min;
	public float max;
	public ItemAttributeData CreateInstance()
	{
		ItemAttributeData data = new ItemAttributeData ();
		data.info = this;
		data.value = Random.Range (min, max);
		return data;
	}
}

public class ItemAttributeData
{
	public ItemAttributeInfo info;
	public float value;
}

public abstract class ItemInfo {
	public enum Category {
		Hand,
		Helmet,
		Armor,
		Ring,
		Shoes,
		Potion,
		Key,
		Max
	}
	public enum Grade
	{
		Low,
		Normal,
		High,
		Magic,
		Rare,
		Legendary
	}
	public string id;
	public string name;
	public int level;

	public int price;
	public Sprite icon;
	public Category category;
	public Grade grade;
	public List<ItemAttributeInfo> attributes = new List<ItemAttributeInfo> ();
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

public abstract class EquipmentItemData : ItemData {
	public int seq;
	/*
	public Character.EquipPart part;
	public abstract Character.Status GetStatus();
	*/
}

public class ItemManager : MonoBehaviour {
	private Dictionary<string, ItemInfo> dictItemInfo;
	public ItemManager() {
	}
	/*
	public void Init() {
		dictItemInfo = new Dictionary<string, ItemInfo>();
		TextAsset resource = Resources.Load ("Config/ItemInfo") as TextAsset;
		JSONNode root = JSON.Parse (resource.text);

		InitRingItemInfo(root);
		InitShieldItemInfo (root);
		InitShirtItemInfo (root);
		InitWeaponItemInfo(root);

		InitPotionItemInfo(root);

		Debug.Log ("init complete ItemManager");
	}
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
	private void InitWeaponItemInfo(JSONNode root)
	{
		JSONNode itemInfos = root ["weapon"];
		for (int i=0; i<itemInfos.Count; i++) {
			JSONNode jsonInfo = itemInfos[i];
			WeaponItemInfo itemInfo = new WeaponItemInfo();
			itemInfo.id = jsonInfo["id"];
			itemInfo.name = jsonInfo["name"];
			itemInfo.cost = jsonInfo["cost"].AsInt;
			itemInfo.weight = jsonInfo["weight"].AsInt;
			itemInfo.description = jsonInfo["description"];
			itemInfo.attack = jsonInfo["attack"].AsInt;
			itemInfo.speed = jsonInfo["speed"].AsInt;
			dictItemInfo.Add (itemInfo.id, itemInfo);
		}
	}

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
	public ItemInfo Find(string id) {
		return dictItemInfo [id];
	}
	public ItemData CreateInstance(string id) {
		return dictItemInfo [id].CreateInstance ();
	}*/
}