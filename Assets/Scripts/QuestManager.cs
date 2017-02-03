using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		public string dialouge;
	};

	public string id;
	public string name;
	public State state = State.Invalid;
	//public RewardInfo reward = new RewardInfo();
	public List<QuestTrigger> triggers = new List<QuestTrigger> ();
	public List<QuestProgress> progresses = new List<QuestProgress> ();
	//public List<Dialouge> startDialouges = new List<Dialouge> ();
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

public class CompleteQuest {
	public string id;
	public int date;
	public int count;
};

public class QuestManager : Singleton<QuestManager> {
	public delegate void UpdateDelegate(string key);
	public delegate void CompleteDelegate(QuestData quests);
	public UpdateDelegate[]  updates = new UpdateDelegate[(int)QuestProgress.Type.Max];
	public CompleteDelegate onComplete;
	public Dictionary<string, CompleteQuest> completes = new Dictionary<string, CompleteQuest>();
	public Dictionary<string, QuestData> quests = new Dictionary<string, QuestData>();

	//private delegate QuestStartCondition CreateStartConditionInstance(JSONNode attr);
	//private delegate QuestCompleteCondition CreateCompleteConditionInstance (JSONNode attr);
	public void Init() {
		completes = new Dictionary<string, CompleteQuest>();
        quests = new Dictionary<string, QuestData> {
            { "QUEST_001",  new QuestData() {
                id = "QUEST_001", name = "First Blood", state = QuestData.State.AccecptWait,
                progresses = new List<QuestProgress> {
                    new QuestProgress_KillMonster() { monsterID = "", goal = 1, progress = 0 }
                }
            } },
            { "QUEST_002",  new QuestData() {
                id = "QUEST_002", name = "Daemon Hunter", state = QuestData.State.AccecptWait,
                progresses = new List<QuestProgress> {
                    new QuestProgress_KillMonster() { monsterID = "DAEMON_001", goal = 3, progress = 0 }
                }
            } }
        };
        foreach(var itr in quests)
        {
            QuestData quest = itr.Value;
            quest.Start();
        }
	}

	public void Update(QuestProgress.Type type, string id)
	{
		if (null == updates [(int)type]) {
			return;
		}
		updates [(int)type] (id);
		foreach (var itr in quests) {
			QuestData quest = itr.Value;
			if (true == quest.IsComplete ()) {
				if (null != onComplete) {
					onComplete (quest);
				}
			}
		}
	}

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
				return quest;
			}
		}
		return null;
	}
}