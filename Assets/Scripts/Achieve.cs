using UnityEngine;
using System.Collections.Generic;

public class Achieve : Progress
{
	public const string AchieveType_CollectCoin = "CollectCoin";
	public const string AchieveType_CollectItem = "CollectItem";
	public const string AchieveType_PlayerLevel = "PlayerLevel";
	public const string AchieveType_DungeonLevel = "DungeonLevel";
	public const string AchieveType_DieCount = "DieCount";
	public const string AchieveType_SellKey = "SellKey";
	public const string AchieveType_EnemiesSlain = "EnemiesSlain";
	public const string AchieveType_BossSlain = "BossSlain";
	public const string AchieveType_UseItem = "UseItem";


	public static Dictionary<string, Progress.Operation> achieve_operations = new Dictionary<string, Progress.Operation>()
	{
		{ AchieveType_CollectCoin, Progress.Operation.Add },
		{ AchieveType_PlayerLevel, Progress.Operation.Max },
		{ AchieveType_DieCount, Progress.Operation.Add },
		{ AchieveType_SellKey, Progress.Operation.Add },
		{ AchieveType_CollectItem, Progress.Operation.Add },
		{ AchieveType_EnemiesSlain, Progress.Operation.Add },
	};

	public class Meta
	{
		public string name;
		public string sprite_path;
		public string type;
		public int step;
		public int goal;
		public Stat.Data reward_stat;
	}

	public string name = "";
	public int step = 0;
	public List<Meta> metas = new List<Meta>();

	public Achieve(string name, string type, int step, int count, int goal)
	{
		this.name = name;
		this.type = type;
		this.step = step;
		this.count = count;
		this.goal = goal;
		if (false == achieve_operations.ContainsKey(type))
		{
			throw new System.Exception("invalid progress update operation(progress_type:" + type + ", progress_key:" + key + ")");
		}
		this.operation = achieve_operations[type];
	}

	public override void OnUpdate()
	{
		Database.Execute(Database.Type.UserData,
			"UPDATE user_achieve SET achieve_count=" + count + " WHERE achieve_type='" + type + "'"
		);
	}

	public override void OnComplete()
	{
		GameManager.Instance.ui_ticker.Write(GameText.GetText("ACHIEVE/COMPLETE", name));
		if (metas.Count >= step + 1)
		{
			Meta meta = metas[step];
			step = meta.step;
			count = count - goal;
			goal = meta.goal;
			Database.Execute(Database.Type.UserData,
				"UPDATE user_achieve SET achieve_step=" + step + ", achieve_count=" + count + ", achieve_goal=" + goal + " WHERE achieve_type='" + type + "'"
			);

			if (count >= goal)
			{
				OnComplete();
			}
		}
		else
		{
			ProgressManager.Instance.Remove(this);
		}
	}
}

public class AchieveManager : Util.Singleton<AchieveManager>
{
	private Dictionary<string, Achieve> achieves;

	public void Init()
	{
		CreateAchieveTableIfNotExists();
		achieves = new Dictionary<string, Achieve>();
		LoadAchieveDatas();
		LoadAchieveMetas();
	}

	private void CreateAchieveTableIfNotExists()
	{
		Database.Execute(Database.Type.UserData,
			"CREATE TABLE IF NOT EXISTS user_achieve (" +
				"achieve_name TEXT NOT NULL," +
				"achieve_type TEXT NOT NULL," +
				"achieve_step INT NOT NULL DEFAULT 0," +
				"achieve_count INT NOT NULL DEFAULT 0," +
				"achieve_goal INT NOT NULL DEFAULT 0," +
				"PRIMARY KEY('achieve_type', 'achieve_step')" +
			")"
		);
	}

	private void LoadAchieveDatas()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.UserData,
			"SELECT achieve_name, achieve_type, achieve_step, achieve_count, achieve_goal FROM user_achieve"
		);
		while (true == reader.Read())
		{
			Achieve achieve = new Achieve(
				reader.GetString("achieve_name"),
				reader.GetString("achieve_type"),
				reader.GetInt32("achieve_step"),
				reader.GetInt32("achieve_count"),
				reader.GetInt32("achieve_goal")
			);

			achieves.Add(achieve.type, achieve);
			ProgressManager.Instance.Add(achieve);
		}
	}

	private void LoadAchieveMetas()
	{
		Dictionary<string, List<Achieve.Meta>> achieve_metas = new Dictionary<string, List<Achieve.Meta>>();

		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT achieve_type, achieve_step, achieve_name, achieve_goal, reward_stat_type, reward_stat_value FROM meta_achieve order by achieve_type, achieve_step"
		);
		while (true == reader.Read())
		{
			Achieve.Meta meta = new Achieve.Meta()
			{
				type = reader.GetString("achieve_type"),
				step = reader.GetInt32("achieve_step"),
				name = reader.GetString("achieve_name"),
				goal = reader.GetInt32("achieve_goal"),
			};
			meta.reward_stat = new Stat.Data() { type = (StatType)reader.GetInt32("reward_stat_type"), value = reader.GetFloat("reward_stat_value") };

			if (false == achieve_metas.ContainsKey(meta.type))
			{
				achieve_metas[meta.type] = new List<Achieve.Meta>();
			}

			achieve_metas[meta.type].Add(meta);
		}

		foreach (var itr in achieve_metas)
		{
			if (false == achieves.ContainsKey(itr.Key))
			{
				Achieve achieve = new Achieve(
					itr.Value[0].name,
					itr.Key,
					1,
					0,
					itr.Value[0].goal
				);

				Database.Execute(Database.Type.UserData,
					"INSERT INTO user_achieve(achieve_name, achieve_type, achieve_step, achieve_count, achieve_goal) VALUES('" + achieve.name + "','" + achieve.type + "',1,0," + achieve.goal + ")"
				);

                achieve.metas = itr.Value;
				achieves.Add(achieve.type, achieve);
				ProgressManager.Instance.Add(achieve);
			}
			else
			{
				Achieve achieveData = achieves[itr.Key];

				if (false == achieves.ContainsKey(itr.Key))
				{
					Debug.LogError("invalid achievemt type(type:" + itr.Key + ")");
					continue;
				}

				achieveData.metas = itr.Value;
				if (achieveData.step >= achieveData.metas.Count)    // max achievement
				{
					Achieve.Meta lastAchieveMeta = achieveData.metas[achieveData.metas.Count - 1];
					if (achieveData.count >= lastAchieveMeta.goal)
					{
						GameManager.Instance.player.stats.AddStat(lastAchieveMeta.reward_stat);
						achieves.Remove(itr.Key);
						ProgressManager.Instance.Remove(achieveData);
					}
				}
				else if (1 < achieveData.step)
				{
					Achieve.Meta completedAchieveMeta = itr.Value[achieveData.step - 2];
					GameManager.Instance.player.stats.AddStat(completedAchieveMeta.reward_stat);
				}
			}
		}
	}
}