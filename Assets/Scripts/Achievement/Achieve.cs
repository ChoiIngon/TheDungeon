using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Achieve : Progress
{
	public class Meta
	{
		public string id;
		public int step;
		public int goal;
	}

	public string name = "";
	public int state = 0;
	public int step = 0;

	public override void OnUpdate()
	{
		Database.Execute(Database.Type.UserData,
			"INSERT INTO user_achieve VALUES() ON CONFILT(achieve_id) DO UPDATE SET achieve_step=" + step + ", achieve_count=" + count
		);
	}

	public override void OnComplete()
	{
	}

	public void OnReward()
	{
	}
}

public class AchieveManager : Util.Singleton<AchieveManager>
{
	private Dictionary<string, Achieve> achieves = new Dictionary<string, Achieve>();
	public void Init()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT achieve_id, achieve_name, achieve_step, goal, description FROM meta_achieve"
		);
		while (true == reader.Read())
		{
			Achieve achieve = new Achieve();
			achieve.id = reader.GetString("achieve_id");
			achieve.name = reader.GetString("achieve_name");
			achieve.step = reader.GetInt32("achieve_step");
		}

		Database.Execute(Database.Type.UserData,
			"CREATE TABLE IF NOT EXISTS user_achieve (" +
				"achieve_id TEXT NOT NULL," +
				"achieve_step INT NOT NULL DEFAULT 0," +
				"achieve_state INT NOT NULL DEFAULT 0," +
				"achieve_count INT NOT NULL DEFAULT 0," +
				"PRIMARY KEY('achieve_id')" +
			")"
		);
	}
}