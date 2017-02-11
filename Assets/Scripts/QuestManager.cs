using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
using System.Collections;
using System.Collections.Generic;

public class QuestProgress
{
    public string type;
    public string key;
    public int goal;
    public int progress;
    public virtual void Start()
    {
#if UNITY_EDITOR
        Assert.AreNotEqual(null, type);
        Assert.AreNotEqual("", type);
#endif
        if (false == QuestManager.Instance.updates.ContainsKey(type))
        {
            QuestManager.Instance.updates[type] = this.Update;
        }
        else
        {
            QuestManager.Instance.updates[type] += this.Update;
        }
    }

    public virtual bool IsComplete()
    {
        if (progress >= goal)
        {
            QuestManager.Instance.updates[type] -= this.Update;
            return true;
        }
        return false;
    }

    public virtual void Update(string key)
    {
        if ("" == this.key || this.key == key)
        {
            progress++;
        }
    }
}


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
    public Dictionary<string, UpdateDelegate> updates = new Dictionary<string, UpdateDelegate>();
	public Dictionary<string, CompleteQuest> completes = new Dictionary<string, CompleteQuest>();
	public Dictionary<string, QuestData> quests = new Dictionary<string, QuestData>();

    public CompleteDelegate onComplete;
    //private delegate QuestStartCondition CreateStartConditionInstance(JSONNode attr);
    //private delegate QuestCompleteCondition CreateCompleteConditionInstance (JSONNode attr);
    public void Init() {
		completes = new Dictionary<string, CompleteQuest>();
		onComplete = null;
        quests = new Dictionary<string, QuestData> {
            { "QUEST_001",  new QuestData() {
                id = "QUEST_001", name = "First Blood", state = QuestData.State.AccecptWait,
                progresses = new List<QuestProgress> {
                    new QuestProgress() { type = "KillMonster", key = "", goal = 1, progress = 0 }
                }
            } },
            { "QUEST_002",  new QuestData() {
                id = "QUEST_002", name = "Daemon Hunter", state = QuestData.State.AccecptWait,
                progresses = new List<QuestProgress> {
                    new QuestProgress() { type = "KillMonster", key = "DAEMON_001", goal = 3, progress = 0 }
                }
            } },
            { "QUEST_003", new QuestData() {
                id = "QUEST_003", name = "Cutting it close", state = QuestData.State.AccecptWait,
                progresses = new List<QuestProgress>
                {
                    new QuestProgress() { type = "Heal", key = "", goal = 1, progress = 0 }
                }
            } }
        };
        foreach(var itr in quests)
        {
            QuestData quest = itr.Value;
            quest.Start();
        }
	}

	public void Update(string type, string key)
	{
		if (null == updates [type]) {
			return;
		}
		updates [type] (key);
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