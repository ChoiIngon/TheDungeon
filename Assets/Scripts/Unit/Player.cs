using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

public class Player : Singleton<Player> {
    public Unit.Stat stats;
    public int coins;
    public Inventory inventory;
	public Dictionary<Tuple<EquipmentItem.Part, int>, EquipmentItem> equipments;

    public void Init() {
        stats = new Unit.Stat();

		stats.health = 1000;
		stats.attack = 100;
		stats.defense = 100;
		stats.speed = 100;
		stats.critcal = 0.0f;
		stats.coinBonus = 0.0f;
		stats.expBonus = 0.0f;

        coins = 0;
		Load ();

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
		inventory = new Inventory();
		inventory.Init ();
    }
	public EquipmentItem GetEquipment(EquipmentItem.Part category, int index) {
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
    public Unit.Stat GetStat()
    {
        Unit.Stat stat = new Unit.Stat();
        foreach (var equipment in equipments)
        {
            if (null == equipment.Value)
            {
                continue;
            }
            stat += equipment.Value.GetStat(stats);
        }
        return stats + stat;
    }

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerdata.dat");
		string json = JsonUtility.ToJson (this);
		Debug.Log ("save data:" + json);
		bf.Serialize (file, json);
		file.Close ();
	}

	public void Load()
	{
		Debug.Log (Application.persistentDataPath);
		if (File.Exists (Application.persistentDataPath + "/playerdata.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerdata.dat", FileMode.Open);
			string json = (string)bf.Deserialize (file);
			Player player = JsonUtility.FromJson<Player> (json);
			coins = player.coins;
			file.Close ();
		} 
	}

}
