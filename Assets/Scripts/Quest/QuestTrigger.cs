﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class QuestTrigger
{
	public abstract bool IsAvailable();
}

public class QuestTrigger_LessLevel : QuestTrigger
{
	public const string id = "LessLevel";
	public int level;
	public override bool IsAvailable() {
		if (level <= Player.Instance.level) {
			return true;
		}
		return false;
	}
}

public class QuestTrigger_LessCompleteQuestCount : QuestTrigger {
	public const string id = "LessCompleteQuestCount";
	public int count;
	public QuestTrigger_LessCompleteQuestCount(int count)
	{
		this.count = count;
	}
	public override bool IsAvailable() {
		if (count <= QuestManager.Instance.completes.Count) {
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
		if (false == QuestManager.Instance.completes.ContainsKey (questID)) {
			return false;
		}
		return true;
	}
}