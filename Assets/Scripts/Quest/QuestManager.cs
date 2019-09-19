using UnityEngine;
using UnityEngine.Analytics;
#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
using System.Collections;
using System.Collections.Generic;

public class CompleteQuest
{
	public string id;
	public int date;
	public int count;
};

public class QuestManager : Util.Singleton<QuestManager>
{
	public void Init()
	{
		CreateQuestTableIfNotExists();
	}
	/*
	public delegate void UpdateDelegate(string key);
	public delegate void CompleteDelegate(QuestData quests);
	public delegate QuestTrigger CreateTriggerDelegate(string value);

	private Dictionary<string, CreateTriggerDelegate> triggerFactory = new Dictionary<string, CreateTriggerDelegate>() {
		{ "LessCompleteQuestCount",	(string value) => { return new QuestTrigger_LessCompleteQuestCount(int.Parse(value)); }},
		{ "CompleteQuestID", 		(string value) => { return new QuestTrigger_CompleteQuestID(value); }}	
	};

    public Dictionary<string, UpdateDelegate> updates;
	public Dictionary<string, CompleteQuest> completes;
	public Dictionary<string, QuestData> quests;

    public CompleteDelegate onComplete;
    
	[System.Serializable]
	public class QuestConfig
	{
		[System.Serializable]
		public class Trigger
		{
			public string type;
			public string value;
		}
		[System.Serializable]
		public class Progress
		{	
			public string name;
			public string type;
			public string key;
			public int goal;
		}
		public string id;
		public string name;
		public Trigger[] triggers;
		public Progress[] progresses;
		public QuestData.Dialogue start_dialogue;
		public QuestData.Dialogue complete_dialogue;
		public QuestData.Reward reward;
	}

	
*/
	public Quest Find(string questID)
	{
		return null;
	}
	public Quest GetAvailableQuest()
	{
		Database.Execute(Database.Type.MetaData,
			"SELECT quest_id FROM meta_quest"
		);
		return null;
	}

	private void CreateQuestTableIfNotExists()
	{
		Database.Execute(Database.Type.UserData,
			"CREATE TABLE IF NOT EXISTS user_quest (" +
				"quest_name TEXT NOT NULL," +
				"quest_type TEXT NOT NULL," +
				"quest_count INT NOT NULL DEFAULT 0," +
				"quest_goal INT NOT NULL DEFAULT 0," +
				"PRIMARY KEY('quest_type')" +
			")"
		);
	}
	private void LoadQuestDatas()
	{
	}
}