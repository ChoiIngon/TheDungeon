using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
	Invalid,
	Health = 1,
	Health_Rate,
	Attack,
	Attack_Rate,
	Defense = 5,
	Defense_Rate,
	Speed,
	Speed_Rate,
	CriticalChance,
	CriticalRate,
	ExpBonus,
	CoinBonus,
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
		}
		else
		{
			datas[data.type] = data.value;
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

		if (EPSILON >= datas[data.type])
		{
			datas.Remove(data.type);
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
}

