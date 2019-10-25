using UnityEngine;
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
	public const string MeetNpc = "MeetNcp";

	public static Dictionary<string, Progress.Operation> progress_operations = new Dictionary<string, Progress.Operation>()
	{
		{ CollectCoin, Progress.Operation.Add },
		{ CollectItem, Progress.Operation.Add },
		{ PlayerLevel, Progress.Operation.Max },
		{ DungeonLevel, Progress.Operation.Max },
		{ DieCount, Progress.Operation.Add },
		{ SellKey, Progress.Operation.Add },
		{ EnemiesSlain, Progress.Operation.Add },
		{ BossSlain, Progress.Operation.Add },
		{ UseItem, Progress.Operation.Add },
		{ MeetNpc, Progress.Operation.Add },
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
		Tuple<string, string> key = new Tuple<string, string>(progress.type, progress.key);
		if (false == progresses.ContainsKey(key))
		{
			Debug.Log("can not find progress(tyep:" + progress.type + ", key:" + progress.key + ")");
			return;
		}
		progresses[key].Remove(progress);
		if (0 == progresses[key].Count)
		{
			progresses.Remove(key);
		}
	}
	
	public void Update(string progressType, string progressKey, int changedAmount)
	{
		if ("" != progressKey)
		{
			Tuple<string, string> key = new Tuple<string, string>(progressType, progressKey);
			if (true == progresses.ContainsKey(key))
			{
				List<Progress> progressesInKey = new List<Progress>(progresses[key]);
				foreach (Progress progress in progressesInKey)
				{
					progress.Update(changedAmount);
				}
			}
		}

		{
			Tuple<string, string> key = new Tuple<string, string>(progressType, "");
			if (true == progresses.ContainsKey(key))
			{
				List<Progress> progressesInKey = new List<Progress>(progresses[key]);
				foreach (Progress progress in progressesInKey)
				{
					progress.Update(changedAmount);
				}
			}
		}
	}
}