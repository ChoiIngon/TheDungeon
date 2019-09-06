using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

public class Player : Unit
{
	private Dictionary<Tuple<EquipItem.Part, int>, EquipItem> equip_items;

	public Inventory inventory;

	public void Init()
	{
		equip_items = new Dictionary<Tuple<EquipItem.Part, int>, EquipItem>();
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Helmet, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Hand, 1), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Armor, 0), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Ring, 1), null);
		equip_items.Add(new Tuple<EquipItem.Part, int>(EquipItem.Part.Shoes, 0), null);
		inventory = new Inventory();
		inventory.Init();

		Load();

		CalculateStat();
	}
	public EquipItem Equip(EquipItem item, int index = 0)
	{
		if (null == item)
		{
			return null;
		}

		EquipItem prev = equip_items[new Tuple<EquipItem.Part, int>(item.part, index)];
		equip_items[new Tuple<EquipItem.Part, int>(item.part, index)] = item;

		stats += item.main_stat;
		stats += item.sub_stat;

		CalculateStat();
		return prev;
	}

	public EquipItem Unequip(EquipItem.Part part, int index)
	{
		EquipItem item = equip_items[new Tuple<EquipItem.Part, int>(part, index)];
		equip_items[new Tuple<EquipItem.Part, int>(part, index)] = null;

		stats -= item.main_stat;
		stats -= item.sub_stat;

		CalculateStat();
		return item;
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerdata.dat");
		string json = JsonUtility.ToJson(this);
		Debug.Log("save data:" + json);
		bf.Serialize(file, json);
		file.Close();
	}

	public void Load()
	{
		Debug.Log(Application.persistentDataPath);
		if (File.Exists(Application.persistentDataPath + "/playerdata.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerdata.dat", FileMode.Open);
			string json = (string)bf.Deserialize(file);
			Player player = JsonUtility.FromJson<Player>(json);
			file.Close();
		}
	}
	/*


   
	public EquipmentItem GetEquipment(EquipmentItem.Part category, int index) {
        if(equipments.ContainsKey(new Tuple<EquipmentItem.Part, int>(category, index)))
        {
            return null;
        }
		return equipments [new Tuple<EquipmentItem.Part, int> (category, index)];
	}
	
	
	*/
}
