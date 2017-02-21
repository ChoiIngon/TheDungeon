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
    public Dictionary<string, UpdateDelegate> updates;
	public Dictionary<string, CompleteQuest> completes;
	public Dictionary<string, QuestData> quests;

    public CompleteDelegate onComplete;
    
	public void Init() {
		onComplete = null;
		if (null != quests) {
			return;
		}
		updates = new Dictionary<string, UpdateDelegate>();
		quests = new Dictionary<string, QuestData> {
            { "QUEST_001",  new QuestData() {
                id = "QUEST_001", name = "First Blood", state = QuestData.State.AccecptWait,
                progresses = new List<QuestProgress> {
					new QuestProgress(QuestEvent.KillMonster, "", 1)
                }
            } },
            { "QUEST_002",  new QuestData() {
                id = "QUEST_002", name = "Daemon Hunter", state = QuestData.State.AccecptWait,
                progresses = new List<QuestProgress> {
					new QuestProgress(QuestEvent.KillMonster, "DAEMON_001", 3)
                }
            } },
			{ "QUEST_004", new QuestData() {
				id = "QUEST_004", name = "Get Item", state = QuestData.State.Invalid,
				triggers = new List<QuestTrigger> {
					new QuestTrigger_CompleteQuestID("QUEST_EXAMPLE")
				},
				progresses = new List<QuestProgress> {
					new QuestProgress(QuestEvent.CollectItem, "QUEST_ITEM", 1)
				},
				startDialouge = new QuestData.Dialouge() {
					speacker = "npc",
					dialouge = new string[] {
						"you got quest collect item"
					}
				}
			}}
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
		if (false == updates.ContainsKey (type)) {
			return;
		}
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