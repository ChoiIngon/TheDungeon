using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class QuestTrigger
{
	public abstract bool IsAvailable();
}

public class QuestTrigger_LessCompleteQuestCount : QuestTrigger {
	public const string id = "LessCompleteQuestCount";
	public int count;
	public QuestTrigger_LessCompleteQuestCount(int count)
	{
		this.count = count;
	}
	public override bool IsAvailable() {
		if (count <= ProgressManager.Instance.completes.Count) {
			return true;
		}
		return false;
	}
}
	
public class QuestTrigger_CompleteQuestID : QuestTrigger {
	public const string id = "CompleteQuestID";
	public string questID;
	public QuestTrigger_CompleteQuestID(string questID)
	{
		this.questID = questID;
	}
	public override bool IsAvailable() {
		if (false == ProgressManager.Instance.completes.ContainsKey (questID)) {
			return false;
		}
		return true;
	}
}