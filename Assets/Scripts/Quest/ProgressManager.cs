using UnityEngine;
using System.Collections.Generic;

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
	public Operation operation = Operation.Invalid;

	public void Update(int changedAmount)
	{
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
		if (Progress.Operation.Invalid == progress.operation)
		{
			throw new System.Exception("invalid progress update operation(progress_type:" + progress.type + ", progress_key:" + progress.key + ")");
		}
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
	}

	public void Update(string progressType, string progressKey, int changedAmount)
	{
		if("" != progressKey)
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