using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Achievenment
{
	public void Init()
	{
		Database.Execute(Database.Type.UserData,
			"CREATE TABLE IF NOT EXISTS user_achievement (" +
				"achieve_id TEXT NOT NULL," +
				"achieve_step INT NOT NULL DEFAULT 0," +
				"achieve_state INT NOT NULL DEFAULT 0," +
				"progress_count INT NOT NULL DEFAULT 0," +
				"PRIMARY KEY('achieve_id')" +
			")"
		);

		Util.Database.DataReader reader = Database.Execute(Database.Type.UserData,
			"SELECT achieve_id, achieve_step, achieve_state, progress_count FROM user_achievement"
		);

		while (true == reader.Read())
		{
			
		}
	}
}

