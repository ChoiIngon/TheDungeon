﻿using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
	Invalid,
	Health = 1,
	Health_Rate = 2,
	Attack = 3,
	Attack_Rate = 4,
	Defense = 5,
	Defense_Rate = 6,
	Speed = 7,
	Speed_Rate = 8,
	Critical = 9,
	Critical_Damage = 10,
	ExpBonus = 11,
	CoinBonus = 12,
	StartItemCount = 13,
	StartItemLevel = 14,
	Lucky = 15,
}

public class RandomStatMeta
{
	public StatType type;
	public float min_value;
	public float max_value;
	public float interval;
	public float value
	{
		get
		{
			float rand = Random.Range(min_value, max_value);
			return interval * (int)(rand / interval);
		}
	}

	public Stat.Data CreateInstance()
	{
		return new Stat.Data() { type = this.type, value = this.value };
	}
}

public class Stat
{
    const float EPSILON = 0.0001f;
	public class Data
	{
		public StatType type;
		public float value;
	}

	public Dictionary<StatType, float> datas = new Dictionary<StatType, float>();
	public void SetStat(Data data)
	{
		datas[data.type] = data.value;
	}

	public float GetStat(StatType type)
	{
		if (true == datas.ContainsKey(type))
		{
			return datas[type];
		}
		return 0.0f;
	}

    public List<Data> GetStats()
    {
        List<Data> stats = new List<Data>();
        foreach(var itr in datas)
        {
            stats.Add(new Data() { type = itr.Key, value = itr.Value });
        }
        return stats; 
    }

	public void AddStat(Data data)
	{
		if (true == datas.ContainsKey(data.type))
		{
			datas[data.type] += data.value;
			//Database.Execute(Database.Type.UserData, "UPDATE user_stats SET stat_value=" + data.value + " WHERE stat_type=" + (int)data.type);
		}
		else
		{
			datas[data.type] = data.value;
			//Database.Execute(Database.Type.UserData, "INSERT INTO user_stats (stat_type, stat_value) VALUES (" + (int)data.type + ", " + data.value + ")");
		}
	}

	public void SubtractStat(Data data)
	{
		if (false == datas.ContainsKey(data.type))
		{
			Debug.LogWarning("can not find stat(stat_type:" + data.type.ToString() + ")");
			return;
		}

#if UNITY_EDITOR
		if (datas[data.type] < data.value)
		{
			Debug.LogWarning("stat value is few than asked(need:" + data.value + ", has:" + datas[data.type] + ")");
		}
#endif
		datas[data.type] -= data.value;
		//Database.Execute(Database.Type.UserData, "UPDATE user_stats SET stat_value=" + data.value + " WHERE stat_type=" + (int)data.type);
		if (EPSILON >= datas[data.type])
		{
			datas.Remove(data.type);
			//Database.Execute(Database.Type.UserData, "DELETE from user_stats WHERE stat_type=" + (int)data.type);
		}
	}

	static public Stat operator + (Stat rhs, Stat lhs)
	{
		Stat stat = new Stat();
		foreach (var itr in rhs.datas)
		{
			stat.AddStat(new Data { type = itr.Key, value = itr.Value });
		}

		foreach (var itr in lhs.datas)
		{
			stat.AddStat(new Data { type = itr.Key, value = itr.Value });
		}
		return stat;
	}

	static public Stat operator - (Stat rhs, Stat lhs)
	{
		Stat stat = new Stat();
		foreach (var itr in rhs.datas)
		{
			stat.AddStat(new Data { type = itr.Key, value = itr.Value });
		}

		foreach (var itr in lhs.datas)
		{
			stat.SubtractStat(new Data { type = itr.Key, value = itr.Value });
		}
		return stat;
	}

	static public float Truncate(float value, float trunc)
	{
		return trunc * (int)(value / trunc);
	}
}

