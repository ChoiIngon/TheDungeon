using UnityEngine;
using System.Collections.Generic;

public class Achieve : Progress
{
	public enum State
	{
		Ongoing,
		Complete
	}
	public class Meta
	{
		public string name;
		public string sprite_path;
		public string type;
		public int step;
		public int goal;
		public string description;
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
			name = meta.name;
			step = meta.step;
			count = count - goal;
			goal = meta.goal;
			Database.Execute(Database.Type.UserData,
				"UPDATE user_achieve SET achieve_name='" + name + "', achieve_step=" + step + ", achieve_count=" + count + ", achieve_goal=" + goal + " WHERE achieve_type='" + type + "'"
			);

			GameManager.Instance.ui_textbox.AsyncWrite(GameText.GetText("ACHIEVE/COMPLETE", name) + "\n" + reward_stat.ToString());
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

	public Stat.Data reward_stat
	{
		get
		{
			if (0 == complete_step)
			{
				return new Stat.Data();
			}
			return metas[complete_step - 1].reward_stat;
		}
	}

	public Meta meta
	{
		get
		{
			if (step >= metas.Count)
			{
				return metas[metas.Count - 1];
			}
			return metas[step - 1];
		}
	}
	
	public int complete_step
	{
		get
		{
			if (step >= metas.Count)    // max achievement
			{
				Meta lastAchieveMeta = metas[metas.Count - 1];
				if (count >= lastAchieveMeta.goal)
				{
					return step;
				}
			}
			else if (1 < step)
			{
				return step - 1;
			}
			return 0;
		}
	}

	public State state {
		get {
			if (step >= metas.Count)    // max achievement
			{
				Achieve.Meta lastAchieveMeta = metas[metas.Count - 1];
				if (count >= lastAchieveMeta.goal)
				{
					return State.Complete;
				}
			}
			return State.Ongoing;
		}
	}
}

public class AchieveManager : Util.Singleton<AchieveManager>
{
	public Dictionary<string, Achieve> achieves;

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
		Util.Sqlite.DataReader reader = Database.Execute(Database.Type.UserData,
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

		Util.Sqlite.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT achieve_type, achieve_step, achieve_name, achieve_goal, sprite_path, reward_stat_type, reward_stat_value, description FROM meta_achieve order by achieve_type, achieve_step"
		);
		while (true == reader.Read())
		{
			Achieve.Meta meta = new Achieve.Meta()
			{
				type = reader.GetString("achieve_type"),
				step = reader.GetInt32("achieve_step"),
				name = reader.GetString("achieve_name"),
				goal = reader.GetInt32("achieve_goal"),
				sprite_path = reader.GetString("sprite_path"),
				description = reader.GetString("description")

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
				if (false == achieves.ContainsKey(itr.Key))
				{
					Debug.LogError("invalid achievemt type(type:" + itr.Key + ")");
					continue;
				}

				Achieve achieveData = achieves[itr.Key];
				achieveData.metas = itr.Value;

				if(Achieve.State.Complete == achieveData.state)
				{
					ProgressManager.Instance.Remove(achieveData);
				}
			}
		}
	}
}