using System.Collections.Generic;
using Mono.Data.Sqlite;
#if UNITY_EDITOR
using UnityEditor;
#endif

using DataReader = Util.Database.DataReader;
public class Database
{
	public enum Type
	{
		Invalid,
		MetaData,
		UserData
	}
	private static Dictionary<Type, Util.Database> databases = new Dictionary<Type, Util.Database>();

	public static void Connect(Type type, string db)
	{
		databases[type] = new Util.Database(db);
#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
		{
			Type _type = type;
			if (EditorApplication.isPaused)
			{
				databases[_type].Dispose();
			}
		};
#endif
	}

	public static DataReader Execute(Type type, string query)
	{
		if (false == databases.ContainsKey(type))
		{
			throw new System.Exception("invalid db connection id(type:" + type.ToString() + ")");
		}

		try
		{
			return databases[type].Execute(query);
		}
		catch (SqliteException e)
		{
			throw new System.Exception("db_id:" + type.ToString() + ", query:" + query + ", exception:" + e.Message);
		}
	}
#if UNITY_EDITOR
	private static void HandleOnPlayModeChanged()
	{
		// This method is run whenever the playmode state is changed.
		
	}
#endif
}
