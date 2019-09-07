using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItemStatMeta
{
	public StatType type;
	public float base_value;
	public float rand_value;
}

public class EquipItem : Item
{
	public enum Part
	{
		Invalid,
		Helmet,
		Hand,
		Armor,
		Ring,
		Shoes
	}

	[System.Serializable]
	public new class Meta : Item.Meta
	{
		public Part part = Part.Invalid;
		public EquipItemStatMeta main_stat = new EquipItemStatMeta();

		public override Item CreateInstance()
		{
			return new EquipItem(this);
		}
	}

	public class Encant
	{
	}

	public int 	level = 0;
	public Part part = Part.Invalid;
	
	public bool equip
    {
        get {
            return equip_index != -1;
        }
    }
    public int  equip_index = -1;
	public Stat main_stat = new Stat();
	public Stat sub_stat = new Stat();
	public Encant enchant = null;

	public EquipItem(EquipItem.Meta meta) : base(meta)
	{
		part = meta.part;
	}

	/*
	public override List<string> Actions() {
		List<string> actions = base.Actions ();
		actions.Add ("EQUIP");
		actions.Add ("UNEQUIP");
		return actions;
	}
	*/
}