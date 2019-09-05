using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
public class ItemManager : Util.Singleton<ItemManager> {
	/*
	private Dictionary<string, Item.Info> infos;
	private Dungeon.LevelInfo dungeonLevelInfo;
	private int totalWeight;

	[System.Serializable]
	public class GradeWeight
	{
		public int grade;
		public int weight;
	}
	[System.Serializable]
	public class EquipItemConfig
	{
		public GradeWeight[] grade_weight;
		public EquipItemStat.Info[] item_stats;
		public EquipItem.Info[] items;
	}
	EquipItemConfig config;
	public IEnumerator Init() {
		infos = new Dictionary<string, Item.Info> ();
		yield return NetworkManager.Instance.HttpRequest ("info_equipment_item.php", (string json) => {
			config = JsonUtility.FromJson<EquipItemConfig>(json);
			foreach(GradeWeight itr in config.grade_weight)
			{
				totalWeight += itr.weight;
			}
			foreach(EquipItem.Info info in config.items)
			{
				infos.Add(info.id, info);
			}
		});

		InitPotionItemInfo ();
	}

	public void InitDungeonLevel(Dungeon.LevelInfo info)
	{
		dungeonLevelInfo = info;
		totalWeight = 0;
		foreach(GradeWeight itr in info.items.grade_weight)
		{
			totalWeight += itr.weight;
		}
	}

	EquipItem.Grade GetRandomGrade() {
		int weight = Random.Range(0, totalWeight);
		foreach(var itr in dungeonLevelInfo.items.grade_weight)
		{
			weight -= itr.weight;
			if(0 >= weight)
			{
				return (EquipItem.Grade)itr.grade;
			}
		}

		return EquipItem.Grade.Low;
	}

	public Item CreateRandomItem(int level)
	{
		EquipItem.Grade grade = GetRandomGrade ();
		EquipItem.Info info = config.items [Random.Range (0, config.items.Length)];
		EquipItem item = info.CreateInstance () as EquipItem;
		item.grade = (EquipItem.Grade)grade;
		item.level = level;
		item.mainStat = CreateStat(level, info.main_stat);
		for (int i = 0; i < (int)item.grade - (int)EquipItem.Grade.Normal; i++) {
			item.subStats.Add (CreateStat(level, config.item_stats [Random.Range (0, config.item_stats.Length)]));
		}

		Analytics.CustomEvent("CreateItem", new Dictionary<string, object>
		{
			{"id", item.id },
			{"type", item.type.ToString()},
			{"name", item.name },
			{"grade", item.grade.ToString()},
			{"level", item.level}
		});
		return item;
	}

	public Item CreateItem(string id)
	{
		Item item = FindInfo<Item.Info> (id).CreateInstance ();
		Analytics.CustomEvent("CreateItem", new Dictionary<string, object>
		{
			{"id", item.id },
			{"type", item.type.ToString()},
			{"name", item.name },
			{"grade", item.grade.ToString()}
		});
		return item;
	}
			
	public T FindInfo<T>(string id) where T:Item.Info
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
			info.grade = (int)Item.Grade.Normal;
			info.description = "An elixir that will instantly return you to full health and cure poison.";
			infos.Add (info.id, info);
		}
		{
			PoisonPotionItem.Info info = new PoisonPotionItem.Info ();
			info.id = "ITEM_POTION_POISON";
			info.name = "Poison Potion";
			info.price = 100;
			info.icon = "item_potion_003";
			info.grade = (int)Item.Grade.Normal;
			infos.Add (info.id, info);
		}
	}

	EquipItemStat CreateStat(int level, EquipItemStat.Info info)
	{
		float value = info.base_value;
		for (int i = 0; i < level; i++) {
			value += Random.Range (0, info.random_value);
		}
		value = Mathf.Round (value * 100) / 100.0f;
		if ("MAX_HEALTH" == info.type) {
			return new EquipItemStat_MaxHealth (value, info.description);
		} else if ("ATTACK" == info.type) {
			return new EquipItemStat_Attack (value, info.description);
		} else if ("MUL_ATTACK" == info.type) {
			return new EquipItemState_MulAttack (value, info.description);
		} else if ("DEFENSE" == info.type) {
			return new EquipItemStat_Defense (value, info.description);
		} else if ("SPEED" == info.type) {
			return new EquipItemStat_Speed (value, info.description);
		} else if ("CRITICAL" == info.type) {
			return new EquipItemStat_Critical (value, info.description);
		} else if ("COIN_BONUS" == info.type) {
			return new EquipItemStat_CoinBonus (value, info.description);
		} else if ("EXP_BONUS" == info.type) {
			return new EquipItemStat_ExpBonus (value, info.description);
		} else if ("STEALTH" == info.type) {
			return new EquipItemStat_Stealth (value, info.description);
		} else if ("VIABILITY" == info.type) {
			return new EquipItemStat_Viability (value, info.description);
		}
		#if UNITY_EDITOR
		throw new System.Exception("unhandled equipment item stat("+ info.type+")");
		#else
		return null;
		#endif
	}
	*/
}