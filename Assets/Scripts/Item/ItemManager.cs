using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager> {
	private Dictionary<string, Item.Info> infos;
	private int totalWeight;

	[System.Serializable]
	public class GradeWeight
	{
		public int grade;
		public int weight;
	}
	[System.Serializable]
	public class EquipmentItemConfig
	{
		public GradeWeight[] grade_weight;
		public EquipmentItemStat.Info[] item_stats;
		public EquipmentItem.Info[] items;
	}
	EquipmentItemConfig config;
	public IEnumerator Init() {
		infos = new Dictionary<string, Item.Info> ();
		yield return NetworkManager.Instance.HttpRequest ("info_equipment_item.php", (string json) => {
			config = JsonUtility.FromJson<EquipmentItemConfig>(json);
			foreach(GradeWeight itr in config.grade_weight)
			{
				totalWeight += itr.weight;
			}
			foreach(EquipmentItem.Info info in config.items)
			{
				infos.Add(info.id, info);
			}
		});

		InitPotionItemInfo ();
	}

	EquipmentItem.Grade GetRandomGrade() {
		int weight = Random.Range(0, totalWeight);
		foreach(var itr in config.grade_weight)
		{
			weight -= itr.weight;
			if(0 >= weight)
			{
				return (EquipmentItem.Grade)itr.grade;
			}
		}

		return EquipmentItem.Grade.Low;
	}

	public Item CreateRandomItem(int level)
	{
		EquipmentItem.Grade grade = GetRandomGrade ();
		EquipmentItem.Info info = config.items [Random.Range (0, config.items.Length)];
		EquipmentItem item = info.CreateInstance () as EquipmentItem;
		item.grade = (EquipmentItem.Grade)grade;
		item.level = level;
		item.mainStat = CreateStat(level, info.main_stat);
		for (int i = 0; i < (int)item.grade - (int)EquipmentItem.Grade.Normal; i++) {
			item.subStats.Add (CreateStat(level, config.item_stats [Random.Range (0, config.item_stats.Length)]));
		}
		return item;
	}
			
	public T FindInfo<T>(string id) where T:Item
	{
		if (false == infos.ContainsKey (id)) {
			throw new System.Exception ("can't find item info(id:" + id + ")");
		}
		return infos[id] as T;
	}

	private void InitPotionItemInfo()
	{
		{
			HealingPotionItem.Info info = new HealingPotionItem.Info ();
			info.id = "ITEM_POTION_HEALING";
			info.name = "Healing Potion";
			info.price = 100;
			info.icon = "item_potion_002";
			infos.Add (info.id, info);
		}
		{
			PoisonPotionItem.Info info = new PoisonPotionItem.Info ();
			info.id = "ITEM_POTION_POISON";
			info.name = "Poison Potion";
			info.price = 100;
			info.icon = "item_potion_003";
			infos.Add (info.id, info);
		}
	}

	EquipmentItemStat CreateStat(int level, EquipmentItemStat.Info info)
	{
		float value = info.base_value;
		for (int i = 0; i < level; i++) {
			value += Random.Range (0, info.random_value);
		}
		value = Mathf.Round (value * 100) / 100.0f;
		if ("MAX_HEALTH" == info.type) {
			return new EquipmentItemStat_MaxHealth (value, info.description);
		} else if ("ATTACK" == info.type) {
			return new EquipmentItemStat_Attack (value, info.description);
		} else if ("DEFENSE" == info.type) {
			return new EquipmentItemStat_Defense (value, info.description);
		} else if ("SPEED" == info.type) {
			return new EquipmentItemStat_Speed (value, info.description);
		} else if ("CRITICAL" == info.type) {
			return new EquipmentItemStat_Critical (value, info.description);
		} else if ("COIN_BONUS" == info.type) {
			return new EquipmentItemStat_CoinBonus (value, info.description);
		} else if ("EXP_BONUS" == info.type) {
			return new EquipmentItemStat_ExpBonus (value, info.description);
		} else if ("STEALTH" == info.type) {
			return new EquipmentItemStat_Stealth (value, info.description);
		}
		return null;
	}
}