using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData {
	public enum State {
		Invalid,
		AccecptWait,
		OnGoing,
		Complete,
		Rewared,
		Max
	};

	public class Dialouge {
		public string speacker;
		public string[] dialouge;
	};

	public string id;
	public string name;
	public State state = State.Invalid;
	//public RewardInfo reward = new RewardInfo();
	public List<QuestTrigger> triggers = new List<QuestTrigger> ();
	public List<QuestProgress> progresses = new List<QuestProgress> ();
	public Dialouge startDialouge = new Dialouge();
	//public List<Dialouge> completeDialouges = new List<Dialouge> ();

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