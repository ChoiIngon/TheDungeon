using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
	public int level;

	public float max_health;
	public float cur_health;

	public float attack;
	public float defense;
	public float speed;
	public float critcal;
	
	public Stat stats = new Stat();
	
	public virtual void Attack(Unit defender)
	{
		defender.Damage (0);
	}
	public virtual void Damage (int damage)
	{
	}
	public virtual void Health(int health)
	{
	}

	public virtual void CalculateStat()
	{
		attack = stats.GetStat(StatType.Attack) * (1.0f + stats.GetStat(StatType.Attack_Rate));
		defense = stats.GetStat(StatType.Defense) * (1.0f + stats.GetStat(StatType.Defense_Rate));
		speed = stats.GetStat(StatType.Speed) * (1.0f + stats.GetStat(StatType.Speed_Rate));
		critcal = stats.GetStat(StatType.CriticalChance);
		max_health = stats.GetStat(StatType.Health) * (1.0f + stats.GetStat(StatType.Health_Rate));
	}
}
