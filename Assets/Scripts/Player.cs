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

	public Stat stat;
    public void Init() {
		stat = new Stat ();
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

	public IEnumerator AddExp(int amount)
	{
		int incExp = amount + (int)exp.current;
		exp.current = 0;
		while (exp.max < incExp) {
			yield return StartCoroutine(exp.DeferredChange(exp.max, 0.25f));
			incExp -= (int)exp.max;
			level += 1;
			exp.max = Player.Instance.level;
			exp.current = 0;
			// levelup effect
		}
		yield return StartCoroutine(exp.DeferredChange (incExp, 0.1f));
	}

	public void Attack(Monster monster)
	{
		Stat bonus = new Stat ();
		foreach (var itr in equipments) {
			EquipmentItem item = itr.Value;
			bonus += item.GetStat (stat);
		}
		bonus += stat;

		if (bonus.critcal >= Random.Range (0, 1)) {
			monster.Damage ((int)(bonus.attack * 3.0f));
			// critical effect
		} else {
			monster.Damage ((int)bonus.attack);
		}

		for (int i = 0; i < 2; i++) {
			EquipmentItem hand = equipments [new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Hand, i)];
			if (null != hand && null != hand.enchantment) {
				hand.enchantment.Enchant (monster);
			}
		}
	}
}
