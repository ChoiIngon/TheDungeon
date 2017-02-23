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
			public string type;
			public string key;
			public int goal;
		}
		public string id;
		public string name;
		public Trigger[] triggers;
		public Progress[] progresses;
		public QuestData.Dialouge start_dialouge;
		public QuestData.Dialouge complete_dialouge;
		public QuestData.Reward reward;
	}

	public class Config
	{
		public QuestConfig[] quests;
	}
	public IEnumerator Init() {
		onComplete = null;
		if (null != quests) {
			yield break;
		}
		updates = new Dictionary<string, UpdateDelegate>();
		quests = new Dictionary<string, QuestData> ();
			
		yield return NetworkManager.Instance.HttpRequest ("info_quest.php", (string json) => {
			Config config = JsonUtility.FromJson<Config>(json);
			foreach(QuestConfig quest in config.quests)
			{
				QuestData data = new QuestData() {
					id = quest.id, name = quest.name, state = QuestData.State.Invalid
				};

				foreach(QuestConfig.Trigger trigger in quest.triggers)
				{
					data.triggers.Add(triggerFactory[trigger.type](trigger.value));
				}

				foreach(QuestConfig.Progress progress in quest.progresses)
				{
					data.progresses.Add(new QuestProgress(progress.type, progress.key, progress.goal));
				}

				data.startDialouge = quest.start_dialouge;
				data.completeDialouge = quest.complete_dialouge;
				data.reward = quest.reward;
				quests.Add(data.id, data);
			}
		});

		//quests["QUEST_EXAMPLE"] = new Quest_Example();
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
		
	public void Save()
	{
	}

	public void Load()
	{
	}
		

}