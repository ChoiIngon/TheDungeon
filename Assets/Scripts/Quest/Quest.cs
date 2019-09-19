using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : Progress
{
	public enum State
	{
		Invalid,
		AccecptWait,
		OnGoing,
		Complete,
		Rewared,
		Max
	};

	public class Dialogue
	{
		public string speaker;
		public string[] scripts; // 대사
	}

	public class Reward
	{
		public int coin;
	}

	public string name;
	public State state = State.Invalid;
	public Reward reward = new Reward();
	public List<QuestTrigger> triggers = new List<QuestTrigger> ();
	
	public Dialogue start_dialogue = new Dialogue();
	public Dialogue complete_dialogue = new Dialogue();

	public bool IsAvailable()
	{
		if (State.Invalid != state) {
			return false;
		}
		foreach (QuestTrigger trigger in triggers) {
			if(false == trigger.IsAvailable())
			{
				return false;
			}
		}
		state = State.AccecptWait;
		return true;
	}

	public void OnStart()
	{
		ProgressManager.Instance.Add(this);
	}
	public override void OnUpdate()
	{
		throw new System.NotImplementedException();
	}

	public override void OnComplete()
	{
		throw new System.NotImplementedException();
	}
}