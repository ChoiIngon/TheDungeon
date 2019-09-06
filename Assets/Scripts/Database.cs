using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

using DataReader = Util.Database.DataReader;
public class Database 
{
	public enum Type
	{
		Invalid,
		MetaData,
		UserData
	}
	private static Dictionary<Type, string> db_conn_infos = new Dictionary<Type, string>();
	public static void Connect(Type type, string db)
	{
		db_conn_infos[type] = db;
	}

	public static DataReader Execute(Type type, string query)
	{
		if (false == db_conn_infos.ContainsKey(type))
		{
			throw new System.Exception("invalid db connection id(type:" + type.ToString() + ")");
		}

		try
		{
			using (Util.Database db = new Util.Database(db_conn_infos[type]))
			{
				return db.Execute(query);
			}
		}
		catch (SqliteException e)
		{
			throw new System.Exception("db_id:" + type.ToString() + ", query:" + query + ", exception:" + e.Message);
		}
	}
}
