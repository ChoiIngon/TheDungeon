using System;
using System.Collections.Generic;

class UnitStat
{
	public enum StatType
	{
		Invalid,
		Health,
		Attack,
		Attack_Value,
		Attack_Rate,
		Defense,
		Defense_Value,
		Defense_Rate,
		Speed,
		Speed_Value,
		Speed_Rate,
		CriticalChance,
		CriticalRate
	}

	public Dictionary<StatType, float> stats = new Dictionary<StatType, float>();

	public void SetStat(StatType statType, float value)
	{
		stats[statType] = value;
	}

	public void AddStat(StatType statType, float value)
	{
		if (true == stats.ContainsKey(statType))
		{
			stats[statType] += value;
		}
		else
		{
			stats[statType] = value;
		}
	}

	public float GetStat(StatType statType)
	{
		if (true == stats.ContainsKey(statType))
		{
			return stats[statType];
		}
		return 0.0f;
	}

	static public UnitStat operator + (UnitStat rhs, UnitStat lhs)
	{
		UnitStat stat = new UnitStat();
		foreach (var itr in rhs.stats)
		{
			stat.stats[itr.Key] = itr.Value;
		}

		foreach (var itr in lhs.stats)
		{
			if (true == stat.stats.ContainsKey(itr.Key))
			{
				stat.stats[itr.Key] += itr.Value;
			}
			else
			{ 
				stat.stats[itr.Key] = itr.Value;
			}
		}
		return stat;
	}
}