using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItemStat
{
	public string description;
	//public ItemStatTrigger trigger;
	public abstract Player.Stat GetStat (Player.Stat stat);
}

public class EquipmentItemStat_MaxHealth : EquipmentItemStat
{
	float percent;
	public EquipmentItemStat_MaxHealth(float percent, string description)
	{
		this.percent = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.maxHealth = (int)(stat.maxHealth * percent);
		return result;
	}
}

public class EquipmentItemStat_Attack : EquipmentItemStat
{
	float value;
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
	float value;
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
	public float value;
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
	public float percent;
	public EquipmentItemStat_Critical(float percent, string description)
	{
		this.percent = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.critcal = percent;
		return result;
	}
}

public class EquipmentItemStat_CoinBonus : EquipmentItemStat {
	public float percent;
	public EquipmentItemStat_CoinBonus( float percent, string description)
	{
		this.percent = percent;
		this.description = description;
	}

	public override Player.Stat GetStat (Player.Stat stat)
	{
		Player.Stat result = new Player.Stat();
		result.coinBonus = percent;
		return result;
	}
}