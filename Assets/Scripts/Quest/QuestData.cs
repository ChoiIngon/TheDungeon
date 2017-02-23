using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData {
	public enum State {
		Invalid,
		AccecptWait,
		OnGoing,
		Complete,
		Rewared,
		Max
	};

	[System.Serializable]
	public class Dialouge {
		public string speaker;
		public string[] dialouge;
	}

	[System.Serializable]
	public class Reward
	{
		public int coin;
	}
	public string id;
	public string name;
	public State state = State.Invalid;
	public Reward reward = new Reward();
	public List<QuestTrigger> triggers = new List<QuestTrigger> ();
	public List<QuestProgress> progresses = new List<QuestProgress> ();
	public Dialouge startDialouge = new Dialouge();
	public Dialouge completeDialouge = new Dialouge();

	public bool IsAvailable() {
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

	public void Start()
	{
		if (State.AccecptWait != state) {
			return;
		}
		foreach (QuestProgress progress in progresses) {
			progress.Start ();
		}
		state = State.OnGoing;
	}

	public bool IsComplete() {
		if (State.OnGoing != state) {
			return false;
		}
		foreach (QuestProgress progress in progresses) {
			if(false == progress.IsComplete())
			{
				return false;
			}
		}
		state = State.Complete;
		return true;
	}
}