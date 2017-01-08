using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CSVReader : IEnumerable
{
    public class Row
    {
        CSVReader reader;
        string[] values;

        public Row(CSVReader reader, string line)
        {
            this.reader = reader;
            values = line.Split(',');
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

    public CSVReader(string path)
    {
        columnToIndex = new Dictionary<string, int>();
        rows = new List<Row>();
        TextAsset asset = Resources.Load<TextAsset>(path);
        string[] lines = asset.text.Split('\n');

        if (0 == lines.Length)
        {
            return;
        }

        string[] columnNames = lines[0].Split(',');
        for (int i = 0; i < columnNames.Length; i++)
        {
			columnToIndex.Add(columnNames[i].TrimEnd(new char[] {'\n','\r'}), i);
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if ("" == lines[i])
            {
                continue;
            }
            rows.Add(new Row(this, lines[i]));
        }
    }

    public IEnumerator GetEnumerator()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            yield return rows[i];
        }
    }
}


