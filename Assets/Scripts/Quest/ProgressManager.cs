using UnityEngine;
using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;

public class CompleteQuest
{
	public string id;
	public int date;
	public int count;
};

public class ProgressManager : Util.Singleton<ProgressManager>
{
	public delegate void UpdateDelegate(string key);
	
	public delegate QuestTrigger CreateTriggerDelegate(string value);

	public System.Action<Progress> onUpdateProgress;
    public System.Action<QuestData>	onCompleteQuest;

	private Dictionary<string, CreateTriggerDelegate> triggerFactory = new Dictionary<string, CreateTriggerDelegate>() {
		{ "LessCompleteQuestCount",	(string value) => { return new QuestTrigger_LessCompleteQuestCount(int.Parse(value)); }},
		{ "CompleteQuestID", 		(string value) => { return new QuestTrigger_CompleteQuestID(value); }}	
	};

    public Dictionary<string, UpdateDelegate> updates;
	public Dictionary<string, CompleteQuest> completes;
	

	public void Update(string completeType, string completeKey)
	{
		if (false == updates.ContainsKey (completeType))
		{
			return;
		}

		updates[completeType]?.Invoke(completeKey);
		/*
		foreach (var itr in quests)
		{
			QuestData quest = itr.Value;
			if (true == quest.IsComplete ())
			{
				if (null != onCompleteQuest)
				{
					onCompleteQuest (quest);
				}
			}
		}
		*/
	}

	/*
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
	*/
}