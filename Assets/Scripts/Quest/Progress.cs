using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif


[System.Serializable]
public class Progress
{
	public static class Type {
		public const string KillMonster = "KillMonster";
		public const string CollectItem = "CollectItem";
		public const string CrrentLocation = "CurrentLocation";
	}

	public string	name = "";
	public int		step = 0;
	public int		progress = 0;
	public string	complete_type = "";
	public string	complete_key = "";
	public int		complete_goal = 0;

	public virtual void Start()
	{
		if (false == ProgressManager.Instance.updates.ContainsKey(complete_type))
		{
			ProgressManager.Instance.updates[complete_type] = this.Update;
		}
		else
		{
			ProgressManager.Instance.updates[complete_type] += this.Update;
		}
	}

	public virtual void Stop()
	{

	}

	public virtual bool IsComplete()
	{
		if (progress >= complete_goal)
		{
			
			return true;
		}
		return false;
	}

	public virtual void Update(string key)
	{
		if ("" == this.complete_key || this.complete_key == key)
		{
			progress++;
			ProgressManager.Instance?.onUpdateProgress(this);
		}
	}
}



