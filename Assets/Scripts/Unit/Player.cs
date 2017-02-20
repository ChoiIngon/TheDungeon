using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
public class Player : Unit {
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
					_instance.Init ();
					DontDestroyOnLoad (container);
				}  
			}  
			return _instance;  
		}  
	}

	public Transform 	ui;
    public UIGaugeBar 	health;
	public UIGaugeBar 	exp;

	public Transform 	bloodMarkPanel;

	public BloodMark bloodMarkPrefab;
	public Coin coinPrefab;

    public Inventory inventory;
	public Dictionary<Tuple<EquipmentItem.Part, int>, EquipmentItem> equipments;

    public override void Init() {
		base.Init ();

		level = 1;
		stats.health = 1000;
		stats.attack = 100;
		stats.defense = 100;
		stats.speed = 100;
		stats.critcal = 0.0f;
		stats.coinBonus = 0.0f;
		stats.expBonus = 0.0f;

		exp = ui.FindChild("Exp").GetComponent<UIGaugeBar>();
		exp.max = level;
		exp.current = 0;
	
        health = ui.FindChild("Health").GetComponent<UIGaugeBar>();
		health.current = health.max = stats.health;

		UICoin.Instance.count = 0;
		#if UNITY_EDITOR
		Assert.AreNotEqual(null, exp);
		Assert.AreNotEqual(null, health);
		Assert.AreNotEqual(null, UICoin.Instance);
		Assert.AreNotEqual(null, bloodMarkPanel);
		Assert.AreNotEqual(null, bloodMarkPrefab);
		Assert.AreNotEqual(null, coinPrefab);
		#endif

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
		health.max = GetStat ().health;
		inventory.ui.playerInfo.Init ();
		return prev;
	}
	public EquipmentItem UnequipItem(EquipmentItem.Part category, int index) {
		EquipmentItem item = equipments [new Tuple<EquipmentItem.Part, int> (category, index)];
		equipments [new Tuple<EquipmentItem.Part, int> (category, index)] = null;
		health.max = GetStat ().health;
		inventory.ui.playerInfo.Init ();
		return item;
	}
	public void AddCoin(int amount)
	{
		int total = amount;
		int multiply = 1;
		float scale = 1.0f;
		while (0 < total) {
			int countCount = Random.Range (1, 10);
			for (int i = 0; i < countCount; i++) {
				Coin coin = GameObject.Instantiate<Coin> (coinPrefab);
				coin.amount = Mathf.Min(total, multiply);
				coin.transform.SetParent (DungeonMain.Instance.coins, false);
				coin.transform.localScale = new Vector3 (scale, scale, 1.0f);
				coin.transform.localPosition = Vector3.zero;
				iTween.MoveBy (coin.gameObject, new Vector3 (Random.Range (-1.5f, 1.5f), Random.Range (0.0f, 0.5f), 0.0f), 0.5f);
				total -= coin.amount;
				if (0 >= total) {
					return;
				}
			}
			multiply *= 10;
			scale += 0.1f;
		}
	}
	public IEnumerator AddExp(int amount)
	{
		int incExp = amount + (int)exp.current;
		exp.current = 0;
		while (exp.max <= incExp) {
			yield return StartCoroutine(exp.DeferredValue(exp.max, 0.25f));
			incExp -= (int)exp.max;
			level += 1;
			exp.max = level;
			exp.current = 0;
			health.current = health.max;
			// levelup effect
		}
		yield return StartCoroutine(exp.DeferredValue(incExp, 0.1f));
	}
	public override void Attack(Unit defender)
	{
		Stat stat = GetStat ();
		int attack = (int)Mathf.Max(1, stat.attack + Random.Range(-stat.attack * 0.1f, stat.attack * 0.1f) - defender.stats.defense);
		if (stat.critcal/100.0f >= Random.Range (0.0f, 1.0f)) {
			attack *= 3;
			// critical effect
		} 

		for (int i = 0; i < 2; i++) {
			EquipmentItem weapon = equipments [new Tuple<EquipmentItem.Part, int> (EquipmentItem.Part.Hand, i)];
			if (null != weapon && null != weapon.enchantment) {
				weapon.enchantment.Enchant (defender);
			}
		}
		defender.Damage (attack);
	}
	public override void Damage(int damage)
	{
		// proc damage
		StartCoroutine(health.DeferredValue(health.current-damage, 0.2f));
		// damage prefab
		iTween.CameraFadeFrom (0.1f, 0.1f);
		iTween.ShakePosition (Camera.main.gameObject, new Vector3 (0.3f, 0.3f, 0.0f), 0.2f);

		BloodMark bloodMark = GameObject.Instantiate<BloodMark> (bloodMarkPrefab);
		bloodMark.transform.SetParent (bloodMarkPanel.transform, false);
		bloodMark.transform.position = new Vector3 (
			Random.Range (Screen.width / 2 - Screen.width / 2 * 0.85f, Screen.width / 2 + Screen.width / 2 * 0.9f), 
			Random.Range (Screen.height / 2 - Screen.height / 2 * 0.85f, Screen.height / 2 + Screen.height / 2 * 0.9f),
			0.0f
		);
	}
    public override void Health(int health)
    {
        StartCoroutine(this.health.DeferredValue((float)health, 0.2f));
    }
	public override Stat GetStat ()
	{
		Stat stat = new Stat ();
		foreach (var equipment in equipments) {
			if (null == equipment.Value) {
				continue;
			}
			stat += equipment.Value.GetStat (stats);
		}
		return stats + stat;
	}
}
