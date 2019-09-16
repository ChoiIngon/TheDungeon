using UnityEngine;
using System.Collections.Generic;

public abstract class Progress
{
	public static class Type
	{
		public const string KillMonster = "KillMonster";
		public const string CollectItem = "CollectItem";
		public const string CrrentLocation = "CurrentLocation";
	}

	public string id = "";
	public string type = "";
	public string key = "";
	public int count = 0;
	public int goal = 0;

	public void Update(string key)
	{
		count++;
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
	/*
	public void Init()
	{
		Database.Execute(Database.Type.UserData,
			"CREATE TABLE IF NOT EXISTS user_progress (" +
				"progress_id TEXT NOT NULL," +
				"progress_step INT NOT NULL DEFAULT 0," +
				"progress_state INT NOT NULL DEFAULT 0," +
				"progress_count INT NOT NULL DEFAULT 0," +
				"PRIMARY KEY('achieve_id')" +
			")"
		);
	}
	*/
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
	}

	public void Update(string progressType, string progressKey)
	{
		{
			Tuple<string, string> key = new Tuple<string, string>(progressType, progressKey);
			if (true == progresses.ContainsKey(key))
			{
				foreach (Progress progress in progresses[key])
				{
					progress.Update(progressKey);
				}
			}
		}

		{
			Tuple<string, string> key = new Tuple<string, string>(progressType, "");
			if (true == progresses.ContainsKey(key))
			{
				foreach (Progress progress in progresses[key])
				{
					progress.Update(progressKey);
				}
			}
		}
	}

	/*
	public QuestData Find(string questID)
	{ 
		return quests.ContainsKey (questID) ? quests [questID] : null;
	}

	public QuestData GetAvailableQuest()
	{
		foreach(var v in quests)
		{
			QuestData quest = v.Value;
			if(true == quest.IsAvailable())
			{
				quest.Start ();
				return quest;
			}
		}
		return null;
	}
	*/
}