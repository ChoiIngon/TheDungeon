using UnityEngine;
using UnityEngine.Analytics;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
using System.Collections;
using System.Collections.Generic;

public class CompleteQuest {
	public string id;
	public int date;
	public int count;
};

public class QuestManager : Singleton<QuestManager> {
	public delegate void UpdateDelegate(string key);
	public delegate void CompleteDelegate(QuestData quests);
    public Dictionary<string, UpdateDelegate> updates = new Dictionary<string, UpdateDelegate>();
	public Dictionary<string, CompleteQuest> completes;
	public Dictionary<string, QuestData> quests;

    public CompleteDelegate onComplete;
    
	public void Init() {
		onComplete = null;
		if (null != quests) {
			return;
		}
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
		quests["QUEST_EXAMPLE"] = new Quest_Example();

		completes = new Dictionary<string, CompleteQuest>();
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
				Analytics.CustomEvent("CompleteQuest", new Dictionary<string, object>
				{
					{"id", quest.id },
					{"name", quest.name }
				});
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
				quest.Start ();
				return quest;
			}
		}
		return null;
	}
}