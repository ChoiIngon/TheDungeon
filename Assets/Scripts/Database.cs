using System.Collections.Generic;
using Mono.Data.Sqlite;

#if UNITY_EDITOR
using UnityEngine;
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
	[MenuItem("Tools/Database/Disconnect")]
	private static void Disconnect()
	{
		foreach (var itr in databases)
		{
			itr.Value.Dispose();
		}
		databases = null;
	}

	[MenuItem("Tools/Database/Delete User Data")]
	private static void DeleteUserData()
	{
		Disconnect();
		System.IO.File.Delete(Application.persistentDataPath + "/user_data.db");
	}
#endif
}
