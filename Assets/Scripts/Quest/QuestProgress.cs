using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif


[System.Serializable]
public class QuestProgress
{
	public static class Type {
		public const string KillMonster = "KillMonster";
		public const string CollectItem = "CollectItem";
		public const string CrrentLocation = "CurrentLocation";
	}

	public string type;
	public string key;
	public int goal;
	public int progress;
	public string name;
	public QuestProgress(string name, string type, string key, int goal)
	{
		this.name = name;
		this.type = type;
		this.key = key;
		this.goal = goal;
		this.progress = 0;
	}
	public virtual void Start()
	{
		#if UNITY_EDITOR
		Assert.AreNotEqual(null, type);
		Assert.AreNotEqual("", type);
		#endif
		/*
		if (false == QuestManager.Instance.updates.ContainsKey(type))
		{
			QuestManager.Instance.updates[type] = this.Update;
		}
		else
		{
			QuestManager.Instance.updates[type] += this.Update;
		}
		*/
	}

	public virtual bool IsComplete()
	{
		/*
		if (progress >= goal)
		{
			QuestManager.Instance.updates[type] -= this.Update;
			return true;
		}
		*/
		return false;
	}

	public virtual void Update(string key)
	{
		if ("" == this.key || this.key == key)
		{
			progress++;
			UITicker.Instance.Write(name + " " + progress + "/" + goal);
		}
	}
}



