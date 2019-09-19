using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Achieve : Progress
{
	public const string AchieveType_CollectCoin = "CollectCoin";
	public const string AchieveType_Level = "Level";
	public static Dictionary<string, Progress.Operation> achieve_operations = new Dictionary<string, Progress.Operation>()
	{
		{ Achieve.AchieveType_CollectCoin, Progress.Operation.Add },
		{ Achieve.AchieveType_Level, Progress.Operation.Max },
	};

	public class Meta
	{
		public string name;
		public string sprite_path;
		public string type;
		public int step;
		public int goal;
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
		UITicker.Instance.Write("complete achieve: " + name);
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
	}
}

public class AchieveManager : Util.Singleton<AchieveManager>
{
	private Dictionary<string, Achieve> achieves = new Dictionary<string, Achieve>();

	public void Init()
	{
		CreateAchieveTableIfNotExists();
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
				"PRIMARY KEY('achieve_type')" +
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
			"SELECT achieve_type, achieve_step, achieve_name, achieve_goal FROM meta_achieve order by achieve_type, achieve_step"
		);
		while (true == reader.Read())
		{
			Achieve.Meta meta = new Achieve.Meta()
			{
				type = reader.GetString("achieve_type"),
				step = reader.GetInt32("achieve_step"),
				name = reader.GetString("achieve_name"),
				goal = reader.GetInt32("achieve_goal")
			};

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

				achieves[itr.Key].metas = itr.Value;
				achieves.Add(achieve.type, achieve);
				ProgressManager.Instance.Add(achieve);
			}
			else
			{
				Achieve achieve = achieves[itr.Key];
				if (achieve.step >= itr.Value.Count)
				{
					if (achieve.count >= itr.Value[itr.Value.Count - 1].goal)
					{
						achieves.Remove(itr.Key);
						ProgressManager.Instance.Remove(achieve);
					}
				}
				if (true == achieves.ContainsKey(itr.Key))
				{
					achieves[itr.Key].metas = itr.Value;
				}
			}
		}
	}
}