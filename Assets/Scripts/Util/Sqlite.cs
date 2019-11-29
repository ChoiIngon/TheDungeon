using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

namespace Util
{
	public class Sqlite : System.IDisposable
	{
		private IDbConnection conn;
		private bool disposed;

		public class DataReader
		{
			private IDataReader impl;
			private Dictionary<string, int> nameToIndex = new Dictionary<string, int>();
			public DataReader(IDataReader data)
			{
				impl = data;

				for (int i = 0; i < data.FieldCount; i++)
				{
					nameToIndex[data.GetName(i)] = i;
				}
			}

			public bool Read()
			{
				return impl.Read();
			}

			public bool GetBoolean(string name)
			{
				if (false == nameToIndex.ContainsKey(name))
				{
					throw new System.Exception("can not find colum(name:" + name + ")");
				}
				return impl.GetBoolean(nameToIndex[name]);
			}

			public double GetDouble(string name)
			{
				if (false == nameToIndex.ContainsKey(name))
				{
					throw new System.Exception("can not find colum(name:" + name + ")");
				}
				return impl.GetDouble(nameToIndex[name]);
			}

			public float GetFloat(string name)
			{
				if (false == nameToIndex.ContainsKey(name))
				{
					throw new System.Exception("can not find colum(name:" + name + ")");
				}
				return impl.GetFloat(nameToIndex[name]);
			}

			public short GetInt16(string name)
			{
				if (false == nameToIndex.ContainsKey(name))
				{
					throw new System.Exception("can not find colum(name:" + name + ")");
				}
				return impl.GetInt16(nameToIndex[name]);
			}

			public int GetInt32(string name)
			{
				if (false == nameToIndex.ContainsKey(name))
				{
					throw new System.Exception("can not find colum(name:" + name + ")");
				}
				return impl.GetInt32(nameToIndex[name]);
			}

			public long GetInt64(string name)
			{
				if (false == nameToIndex.ContainsKey(name))
				{
					throw new System.Exception("can not find colum(name:" + name + ")");
				}
				return impl.GetInt64(nameToIndex[name]);
			}

			public string GetString(string name)
			{
				if (false == nameToIndex.ContainsKey(name))
				{
					throw new System.Exception("can not find colum(name:" + name + ")");
				}
				return impl.GetString(nameToIndex[name]);
			}

			public T GetEnum<T>(string column)
			{
				return (T)System.Enum.Parse(typeof(T), GetString(column));
			}
		}

		public Sqlite()
		{
			conn = null;
		}

		public Sqlite(string db)
		{
			Open(db);
		}
		~Sqlite()
		{
			this.Dispose(false);
		}

		public void Open(string db)
		{
			Debug.Log("open db(path:" + db + ")");
			if (null != conn)
			{
				Debug.LogWarning("alredy connected(db:" + db + ")");
				return;
			}
			conn = new SqliteConnection("URI=file:" + db);
			conn.Open();
		}

		public DataReader Execute(string query)
		{
			IDbCommand cmd = conn.CreateCommand();
			cmd.CommandText = query;
			return new DataReader(cmd.ExecuteReader());
			//return cmd.ExecuteReader();
		}

		public void Dispose()
		{
			this.Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (true == this.disposed)
			{
				return;
			}

			if (true == disposing)
			{
				if (null != conn)
				{
					Debug.Log("close database");
					conn.Close();
				}
			}

			this.disposed = true;
		}
	}
}