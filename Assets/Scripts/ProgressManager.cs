﻿using UnityEngine;
using System.Collections.Generic;

public class ProgressType
{
	public const string CollectCoin = "CollectCoin";
	public const string CollectItem = "CollectItem";
	public const string PlayerLevel = "PlayerLevel";
	public const string DungeonLevel = "DungeonLevel";
	public const string DieCount = "DieCount";
	public const string SellKey = "SellKey";
	public const string EnemiesSlain = "EnemiesSlain";
	public const string BossSlain = "BossSlain";
	public const string UseItem = "UseItem";

	public static Dictionary<string, Progress.Operation> progress_operations = new Dictionary<string, Progress.Operation>()
	{
		{ CollectCoin, Progress.Operation.Add },
		{ PlayerLevel, Progress.Operation.Max },
		{ DieCount, Progress.Operation.Add },
		{ SellKey, Progress.Operation.Add },
		{ CollectItem, Progress.Operation.Add },
		{ EnemiesSlain, Progress.Operation.Add },
	};
}

public abstract class Progress
{
	public enum Operation
	{
		Invalid,
		Add,
		Max
	}

	public string id = "";
	public string type = "";
	public string key = "";
	public int count = 0;
	public int goal = 0;

	public void Update(int changedAmount)
	{
		if (false == ProgressType.progress_operations.ContainsKey(type))
		{
			throw new System.Exception("invalid progress update operation(progress_type:" + type + ", progress_key:" + key + ")");
		}

		Operation operation = ProgressType.progress_operations[type];
		switch (operation)
		{
			case Operation.Add:
				count += changedAmount;
				break;
			case Operation.Max:
				count = Mathf.Max(count, changedAmount);
				break;
			default:
				throw new System.Exception("invalid progress update operation");
		}
		OnUpdate();
		if (count >= goal)
		{
			OnComplete();
		}
	}

	public abstract void OnUpdate();
	public abstract void OnComplete();
}

public class ProgressManager : Util.Singleton<ProgressManager>
{
    private Dictionary<Tuple<string, string>, List<Progress>> progresses = new Dictionary<Tuple<string, string>, List<Progress>>();
	private List<Progress> complete_progresses = new List<Progress>();

	public void Add(Progress progress)
	{
		Tuple<string, string> key = new Tuple<string, string>(progress.type, progress.key);
		if (false == progresses.ContainsKey(key))
		{
			progresses[key] = new List<Progress>();
		}
		progresses[key].Add(progress);
	}

	public void Remove(Progress progress)
	{
		complete_progresses.Add(progress);
	}
	
	public void Update(string progressType, string progressKey, int changedAmount)
	{
		foreach (Progress complete_progress in complete_progresses)
		{
			Tuple<string, string> key = new Tuple<string, string>(complete_progress.type, complete_progress.key);
			if (false == progresses.ContainsKey(key))
			{
				Debug.Log("can not find progress(tyep:" + complete_progress.type + ", key:" + complete_progress.key + ")");
				return;
			}
			progresses[key].Remove(complete_progress);
			if (0 == progresses[key].Count)
			{
				progresses.Remove(key);
			}
		}

		complete_progresses.Clear();

		if ("" != progressKey)
		{
			Tuple<string, string> key = new Tuple<string, string>(progressType, progressKey);
			if (true == progresses.ContainsKey(key))
			{
				foreach (Progress progress in progresses[key])
				{
					progress.Update(changedAmount);
				}
			}
		}

		{
			Tuple<string, string> key = new Tuple<string, string>(progressType, "");
			if (true == progresses.ContainsKey(key))
			{
				foreach (Progress progress in progresses[key])
				{
					progress.Update(changedAmount);
				}
			}
		}
	}
}