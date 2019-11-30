using System.Collections.Generic;
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
	Critical_Chance = 9,
	Critical_Damage = 10,
	ExpBonus = 11,
	CoinBonus = 12,
	StartItemCount = 13,
	StartItemLevel = 14,
	Lucky = 15,
	Stamina = 16,
	Stamina_Rate = 17,
	Damage_Rate = 18,
	Detect_Monster = 19,
	Detect_Treasure = 20,
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
	public class Data
	{
		public StatType type;
		public float value;
		public override string ToString()
		{
			Meta meta = Manager.Instance.FindMeta(type);
			return meta.ToString(value);
		}
	}

	public class Meta
	{
		public StatType type;
		public string description;
		public string ToString(float value)
		{
			string valueText = (Mathf.Round(value * 100.0f)/100).ToString();
			if (0.0f <= value)
			{
				valueText = "+" + valueText;
			}
			return string.Format(description, valueText);
		}
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
			return Mathf.Max(0.0f, Truncate(datas[type], 0.01f));
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
			datas.Add(data.type, 0.0f);
		}

#if UNITY_EDITOR
		if (datas[data.type] < data.value)
		{
			Debug.LogWarning("stat value is few than asked(need:" + data.value + ", has:" + datas[data.type] + ")");
		}
#endif
		datas[data.type] -= data.value;
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

	static public float Truncate(float value, float trunc = 0.01f)
	{
		return trunc * (int)(value / trunc);
	}

	public class Manager : Util.Singleton<Manager>
	{
		private Dictionary<StatType, Meta> metas = new Dictionary<StatType, Meta>();
		public void Init()
		{
			try
			{
				GoogleSheetReader sheetReader = new GoogleSheetReader(GameManager.GOOGLESHEET_ID, GameManager.GOOGLESHEET_API_KEY);
				sheetReader.Load("meta_stat");
				foreach (GoogleSheetReader.Row row in sheetReader)
				{
					Meta meta = new Meta();
					meta.type = row.GetEnum<StatType>("stat_type");
					meta.description = row.GetString("description");
					metas.Add(meta.type, meta);
				}
			}
			catch (System.Exception e)
			{
				GameManager.Instance.ui_textbox.on_close = () =>
				{
					Application.Quit();
				};
				GameManager.Instance.ui_textbox.AsyncWrite("error: " + e.Message + "\n" + e.ToString(), false);
			}
		}

		public Meta FindMeta(StatType type)
		{
			if (false == metas.ContainsKey(type))
			{
				return null;
			}
			return metas[type];
		}
	}
}

