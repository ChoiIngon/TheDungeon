using UnityEngine;
using System.Collections;

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
	public Data data = new Data();
	public float attack {
		get {
			return 1.0f + data.strength;
		}
	}
	public GameObject ui;
	public GameObject damage;
	GaugeBar health = null;
	// Use this for initialization
	void Start () {
		health = ui.transform.FindChild ("Health").GetComponent<GaugeBar> ();
		health.max = 0;
		health.current = 0;
		RectTransform rt = ui.transform.GetComponent<RectTransform> ();
		rt.position = Camera.main.WorldToScreenPoint (transform.position);
	}

	public void Init()
	{
		inventory.Init ();
	}
	public Inventory inventory;
	public EquipmentItemData helmet;
	public EquipmentItemData[] hands = new EquipmentItemData[2];
	public EquipmentItemData[] rings = new EquipmentItemData[2];
	public EquipmentItemData armor;
	public EquipmentItemData shoes;

	public void EquipItem(ItemData item, int index)
	{
		EquipmentItemData equipment = (EquipmentItemData)item;
		if (null == equipment) {
			return;
		}
		UnequipItem (equipment.info.category, index);
		switch (equipment.info.category) {
		case ItemInfo.Category.Armor:
			armor = equipment;
			break;
		case ItemInfo.Category.Hand:
			{
				WeaponItemInfo info = (WeaponItemInfo)item.info;
				if (true == info.twoHanded) {
					hands [0] = equipment;
					hands [1] = equipment;
				} else {
					hands [index] = equipment;
				}
			}
			break;
		case ItemInfo.Category.Helmet:
			helmet = equipment;
			break;
		case ItemInfo.Category.Ring:
			rings [index] = equipment;
			break;
		case ItemInfo.Category.Shoes:
			shoes = equipment;
			break;
		default :
			throw new System.Exception ("Unequiptable item");
		}
	}
	public void UnequipItem(ItemInfo.Category category, int index)
	{
		switch (category) {
		case ItemInfo.Category.Armor:
			if (null != armor) {
				inventory.Put (armor);
				armor = null;
			}
			break;
		case ItemInfo.Category.Hand:
			if(null != hands[index])
			{
				inventory.Put(hands [index]);
				if (hands [0] == hands [1]) {
					hands [0] = null;
					hands [1] = null;
				} else {
					hands [index] = null;
				}
			}
			break;
		case ItemInfo.Category.Helmet:
			if (null != helmet) {
				inventory.Put (helmet);
				helmet = null;
			}
			break;
		case ItemInfo.Category.Ring:
			if(null != rings[index])
			{
				inventory.Put(rings [index]);
				rings [index] = null;
			}
			break;
		case ItemInfo.Category.Shoes:
			if (null != shoes) {
				inventory.Put (shoes);
				shoes = null;
			}
			break;
		default :
			throw new System.Exception ("Unequiptable item");
			break;
		}
	}
}
