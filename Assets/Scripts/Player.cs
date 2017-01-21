using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	private static Player _instance;  
	public static Player Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (Player)GameObject.FindObjectOfType(typeof(Player));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "Player";  
					_instance = container.AddComponent<Player>();  
				}  
			}  
			return _instance;  
		}  
	}
	[System.Serializable]
	public class Info {
		public int health;
	}
	[System.Serializable]
	public class Data {
		public int level;
		public int exp;
		public int health;
		public float strength;
		public float defense;
		public float speed;
		public Data()
		{
			level = 1;
			exp = 0;
			health = 100;
			strength = 1.0f;
			defense = 1.0f;
			speed = 1.0f;
		}
	}

	GaugeBar health = null;
	public GameObject ui;
	public GameObject damage;
	public Data data;
	public Inventory inventory;
	public Dictionary<Tuple<ItemInfo.Category, int>, EquipmentItemData> equipments;

	public float attack {
		get {
			return 1.0f + data.strength;
		}
	}

	public void Init() {
		data = new Data ();

		health = ui.transform.FindChild ("Health").GetComponent<GaugeBar> ();
		health.max = 0;
		health.current = 0;
		ui.transform.GetComponent<RectTransform> ().position = Camera.main.WorldToScreenPoint (transform.position);

		equipments = new Dictionary<Tuple<ItemInfo.Category, int>, EquipmentItemData>();
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Helmet, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Hand, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Hand, 1), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Armor, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Ring, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Ring, 1), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Shoes, 0), null);

		inventory.Init ();
	}

	public T GetEquipement<T>(ItemInfo.Category category, int index) where T:ItemData {
		return equipments [new Tuple<ItemInfo.Category, int> (category, index)] as T;
	}

	public EquipmentItemData EquipItem(ItemData item, int index) {
		if (null == item) {
			return null;
		}
		EquipmentItemData curr = (EquipmentItemData)item;
		EquipmentItemData prev = equipments [new Tuple<ItemInfo.Category, int> (curr.info.category, index)];
		equipments [new Tuple<ItemInfo.Category, int> (curr.info.category, index)] = curr;
		return prev;
	}

	public EquipmentItemData UnequipItem(ItemInfo.Category category, int index) {
		EquipmentItemData item = equipments [new Tuple<ItemInfo.Category, int> (category, index)];
		equipments [new Tuple<ItemInfo.Category, int> (category, index)] = null;
		return item;
	}
}
