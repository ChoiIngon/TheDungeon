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

    public int level;
    public int exp;
    public GuageBar health;
    public float attack;
    public float defense;
    public float speed;
    public float critcal;

    public Inventory inventory;
    public Dictionary<Tuple<ItemInfo.Category, int>, EquipmentItemData> equipments;

    public GameObject ui;
	public GameObject damage;

    public void Init() {
        level = 0;
        exp = 0;
        health = ui.transform.FindChild("Health").GetComponent<GaugeBar>();
        health.max = 1;
        health.current = health.max;
        attack = 0.0f;
        defense = 0.0f;
        speed = 0.0f;
        critcal = 0.0f;
    
        // init equipments
        equipments = new Dictionary<Tuple<ItemInfo.Category, int>, EquipmentItemData>();
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Helmet, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Hand, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Hand, 1), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Armor, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Ring, 0), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Ring, 1), null);
		equipments.Add (new Tuple<ItemInfo.Category, int> (ItemInfo.Category.Shoes, 0), null);

        // init inventory
		inventory.Init ();

        // init ui
        ui.transform.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(transform.position);
    }

	public T GetEquipement<T>(ItemInfo.Category category, int index) where T:ItemData {
        if(equipments.ContainsKey(new Tuple<ItemInfo.Category, int>(category, index)))
        {
            return null;
        }
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
