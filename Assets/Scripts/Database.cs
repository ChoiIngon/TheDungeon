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
	private static Dictionary<Type, Util.Database> db_conn_infos = new Dictionary<Type, Util.Database>();
	public static void Connect(Type type, string db)
	{
		db_conn_infos[type] = new Util.Database(db);
	}

	public static DataReader Execute(Type type, string query)
	{
		if (false == db_conn_infos.ContainsKey(type))
		{
			throw new System.Exception("invalid db connection id(type:" + type.ToString() + ")");
		}

		try
		{
			return db_conn_infos[type].Execute(query);
		}
		catch (SqliteException e)
		{
			throw new System.Exception("db_id:" + type.ToString() + ", query:" + query + ", exception:" + e.Message);
		}
	}
}
