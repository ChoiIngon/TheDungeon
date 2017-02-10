using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemStat
{
	[System.Serializable]
	public class Info
	{
		public string type;
		public string description;
		public float base_value;
		public float random_value;
	}
	public float value;
	public string description;
	public abstract Player.Stat GetStat (Player.Stat stat);
}

public class EquipmentItemStat_MaxHealth : EquipmentItemStat
{
	public EquipmentItemStat_MaxHealth(float value, string description)
	{
		this.value = value;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.health = (int)value;
		return result;
	}
}

public class EquipmentItemStat_Attack : EquipmentItemStat
{
	public EquipmentItemStat_Attack(float value, string description)
	{
		this.value = value;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result  = new Player.Stat();
		result.attack = value;
		return result;
	}
}

public class EquipmentItemStat_Defense : EquipmentItemStat
{
	public EquipmentItemStat_Defense(float value, string description)
	{
		this.value = value;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.defense = value;
		return result;
	}
}

public class EquipmentItemStat_Speed : EquipmentItemStat
{
	public EquipmentItemStat_Speed(float value, string description)
	{
		this.value = value;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.speed = value;
		return result;
	}
}

public class EquipmentItemStat_Critical : EquipmentItemStat
{
	public EquipmentItemStat_Critical(float percent, string description)
	{
		this.value = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.critcal = value;
		return result;
	}
}

public class EquipmentItemStat_CoinBonus : EquipmentItemStat {
	
	public EquipmentItemStat_CoinBonus( float percent, string description)
	{
		this.value = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.coinBonus = value;
		return result;
	}
}

public class EquipmentItemStat_ExpBonus : EquipmentItemStat {
	public EquipmentItemStat_ExpBonus( float percent, string description)
	{
		this.value = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.coinBonus = value;
		return result;
	}
}

public class EquipmentItemStat_Stealth : EquipmentItemStat {
	public EquipmentItemStat_Stealth( float percent, string description)
	{
		this.value = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.coinBonus = value;
		return result;
	}
}