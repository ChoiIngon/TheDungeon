using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
	public float max_health;
	public float cur_health;

	public float attack;
	public float defense;
	public float speed;
	public float critical;
	
	public Stat stats = new Stat();
	
	public virtual void CalculateStat()
	{
		attack = stats.GetStat(StatType.Attack) * (1.0f + stats.GetStat(StatType.Attack_Rate));
		defense = stats.GetStat(StatType.Defense) * (1.0f + stats.GetStat(StatType.Defense_Rate));
		speed = stats.GetStat(StatType.Speed) * (1.0f + stats.GetStat(StatType.Speed_Rate));
		critical = stats.GetStat(StatType.CriticalChance);
		max_health = stats.GetStat(StatType.Health) * (1.0f + stats.GetStat(StatType.Health_Rate));
	}
}
