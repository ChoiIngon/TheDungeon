﻿using UnityEngine;
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
    
    public GaugeBar health;
	public GaugeBar exp;
	public int gold;
    public float attack;
    public float defense;
    public float speed;
    public float critcal;

	[System.Serializable]
	public struct Stat
	{
		public int curHealth;
		public int maxHealth;
		public float attack;
		public float defense;
		public float speed;
		public float critcal;
		public float goldBonus;
		static public Stat operator + (Stat rhs, Stat lhs)
		{
			rhs.curHealth += lhs.curHealth;
			rhs.maxHealth += lhs.maxHealth;
			rhs.attack += lhs.attack;
			rhs.defense += lhs.defense;
			rhs.speed += lhs.speed;
			rhs.critcal += lhs.critcal;
			rhs.goldBonus += lhs.goldBonus;
			return rhs;
		}
	}

    public Inventory inventory;
	public Dictionary<Tuple<EquipmentItem.Part, int>, EquipmentItem> equipments;

    public GameObject ui;
	public GameObject damage;

    public void Init() {
        level = 0;

		exp = ui.transform.FindChild("Exp").GetComponent<GaugeBar>();
		exp.max = 1;
		exp.current = 0;

        health = ui.transform.FindChild("Health").GetComponent<GaugeBar>();
        health.max = 1;
        health.current = health.max;

        attack = 0.0f;
        defense = 0.0f;
        speed = 0.0f;
        critcal = 0.0f;
    
        // init equipments
		equipments = new Dictionary<Tuple<EquipmentItem.Part, int>, EquipmentItem>();
		equipments.Add (new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Helmet, 0), null);
		equipments.Add (new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Hand, 0), null);
		equipments.Add (new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Hand, 1), null);
		equipments.Add (new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Armor, 0), null);
		equipments.Add (new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Ring, 0), null);
		equipments.Add (new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Ring, 1), null);
		equipments.Add (new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Shoes, 0), null);

        // init inventory
		inventory.Init ();
    }

	public EquipmentItem GetEquipement(EquipmentItem.Part category, int index) {
        if(equipments.ContainsKey(new Tuple<EquipmentItem.Part, int>(category, index)))
        {
            return null;
        }
		return equipments [new Tuple<EquipmentItem.Part, int> (category, index)];
	}

	public EquipmentItem EquipItem(EquipmentItem item, int index) {
		if (null == item) {
			return null;
		}

		EquipmentItem prev = equipments [new Tuple<EquipmentItem.Part, int> (item.part, index)];
		equipments [new Tuple<EquipmentItem.Part, int> (item.part, index)] = item;
		return prev;
	}

	public EquipmentItem UnequipItem(EquipmentItem.Part category, int index) {
		EquipmentItem item = equipments [new Tuple<EquipmentItem.Part, int> (category, index)];
		equipments [new Tuple<EquipmentItem.Part, int> (category, index)] = null;
		return item;
	}

	public void AddCoin(int amount)
	{
	}

	public void AddExp(int amount)
	{
		exp.current += amount;	
		while (exp.current >= exp.max) {
			exp.current -= exp.max;
			level += 1;
			exp.max += 1;
		}
	}
}
