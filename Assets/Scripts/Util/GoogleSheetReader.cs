using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
/**
	1. 구글 api 프로젝트 생성
	https://console.developers.google.com/apis/credentials

	Create credentials > API key

*/
public class GoogleSheetReader : IEnumerable
{
	public class Row
	{
		GoogleSheetReader reader;
		string[] values;

		public Row(GoogleSheetReader reader, JSONObject row)
		{
			this.reader = reader;
			values = new string[row.list.Count];
			for (int i = 0; i < row.list.Count; i++)
			{
				values[i] = row.list[i].str;
			}
		}

		public string GetData(string column)
		{
			if (false == reader.columnToIndex.ContainsKey(column))
			{
				throw new System.IndexOutOfRangeException("can't find " + column + " column");
			}
			int index = reader.columnToIndex[column];
			return values[index];
		}

		public string this[string column]
		{
			get { return GetData(column); }
		}
	}

	public Dictionary<string, int> columnToIndex;
	private List<Row> rows;
	private string sheet_id;
	private string api_key;

	public GoogleSheetReader(string sheetID, string apiKey)
	{
		columnToIndex = new Dictionary<string, int>();
		rows = new List<Row>();
		sheet_id = sheetID;
		api_key = apiKey;
	}

	public bool Load(string sheetName, int timeout = 10000 /* 10 secs */)
	{
		HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://sheets.googleapis.com/v4/spreadsheets/" + sheet_id + "/values/" + sheetName + "?key=" + api_key);
		req.Method = "GET";
		req.Timeout = timeout;

		try
		{
			using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
			{
				HttpStatusCode status = response.StatusCode;

				Stream stream = response.GetResponseStream();
				using (StreamReader reader = new StreamReader(stream))
				{
					BuildRows(reader);
				}
			}
		}
		catch (WebException e)
		{
			return false;
		}
		return true;
	}

	public IEnumerator AsyncLoad(string sheetName, int timeout = 10000 /* 10 secs */)
	{
		HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://sheets.googleapis.com/v4/spreadsheets/" + sheet_id + "/values/" + sheetName + "?key=" + api_key);
		req.Method = "GET";
		req.Timeout = timeout;

		bool isDone = false;
		req.BeginGetResponse(new System.AsyncCallback((System.IAsyncResult result) => 
		{
			try
			{
				using (HttpWebResponse response = (HttpWebResponse)req.EndGetResponse(result))
				{
					Stream stream = response.GetResponseStream();
					using (StreamReader reader = new StreamReader(stream))
					{
						BuildRows(reader);
						isDone = true;
					}
					response.Close();
				}
			}
			catch (WebException e)
			{
				isDone = true;
				return;
			}
		}), null);

		while (false == isDone)
		{
			yield return null;
		}
	}
	public IEnumerator GetEnumerator()
	{
		for (int i = 0; i < rows.Count; i++)
		{
			yield return rows[i];
		}
	}

	private void BuildRows(StreamReader reader)
	{
		JSONObject obj = new JSONObject(reader.ReadToEnd());
		JSONObject values = obj["values"];

		if (1 > values.list.Count)
		{
			return;
		}
		JSONObject columnNames = (JSONObject)values.list[0];
		for (int i = 0; i < columnNames.list.Count; i++)
		{
			columnToIndex.Add(columnNames.list[i].str, i);
		}

		for (int i = 1; i < values.list.Count; i++)
		{
			rows.Add(new Row(this, values.list[i]));
		}
	}
}


