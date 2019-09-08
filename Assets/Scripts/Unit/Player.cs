using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif

public class ItemEquipEvent
{
    public EquipItem item;
    public int equip_index;
}

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

		Load();

		CalculateStat();
	}
    
	public EquipItem Equip(EquipItem item, int equip_index = 0)
	{
		if (null == item)
		{
			return null;
		}

        EquipItem prev = Unequip(item.part, equip_index);
		equip_items[new Tuple<EquipItem.Part, int>(item.part, equip_index)] = item;
        item.equip_index = equip_index;
		stats += item.main_stat;
		stats += item.sub_stat;

		CalculateStat();

        Debug.Log("equip item(item_id:" + item.meta.id + ", part:" + item.part + ", equip_index:" + equip_index + ")");
        Util.EventSystem.Publish<ItemEquipEvent>(EventID.Item_Equip, new ItemEquipEvent() { item = item, equip_index = equip_index } );
        return prev;
	}

	public EquipItem Unequip(EquipItem.Part part, int equip_index)
	{
		EquipItem item = equip_items[new Tuple<EquipItem.Part, int>(part, equip_index)];
        if (null == item)
        {
            return null;
        }

        item.equip_index = -1;
		equip_items[new Tuple<EquipItem.Part, int>(part, equip_index)] = null;

		stats -= item.main_stat;
		stats -= item.sub_stat;

		CalculateStat();

        Debug.Log("unequip item(item_id:" + item.meta.id + ", part:" + item.part + ", equip_index:" + equip_index + ")");
        Util.EventSystem.Publish<ItemEquipEvent>(EventID.Item_Unequip, new ItemEquipEvent() { item = item, equip_index = equip_index });
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
