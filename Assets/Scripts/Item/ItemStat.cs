using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemStat
{
	public string description;
	//public ItemStatTrigger trigger;
	public abstract Player.Stat GetStat (Player.Stat stat);
}

public class ItemStat_MaxHealth : ItemStat
{
	float percent;
	public ItemStat_MaxHealth(float percent, string description)
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

public class ItemStat_Attack : ItemStat
{
	float value;
	public ItemStat_Attack(float value, string description)
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

public class ItemStat_Defense : ItemStat
{
	float value;
	public ItemStat_Defense(float value, string description)
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

public class ItemStat_Speed : ItemStat
{
	public float value;
	public ItemStat_Speed(float value, string description)
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

public class ItemStat_Critical : ItemStat
{
	public float percent;
	public ItemStat_Critical(float percent, string description)
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

public class ItemStat_CoinBonus : ItemStat {
	public float percent;
	public ItemStat_CoinBonus( float percent, string description)
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