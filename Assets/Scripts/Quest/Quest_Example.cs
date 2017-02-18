using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Example : QuestData {
	public class QuestTrigger_Example : QuestTrigger
	{
		public override bool IsAvailable()
		{
			if (0 == QuestManager.Instance.completes.Count) {
				return true;
			}
			return false;
		}
	}
	public Quest_Example()
	{
		this.id = "QUEST_EXAMPLE";
		this.name = "Example Quest";
		this.state = QuestData.State.Invalid;
		this.triggers.Add (new QuestTrigger_Example ());
		this.progresses = new List<QuestProgress> {
			new QuestProgress () { type = "KillMonster", key = "", goal = 1, progress = 0 }
		};
		startDialouge = new Dialouge ();
		startDialouge.speacker = "Far Seer";
		startDialouge.dialouge = new string[] {
			"Quest dialouge 1, Quest dialouge 1, Quest dialouge 1, Quest dialouge 1",
			"Quest dialouge 2, Quest dialouge 1, Quest dialouge 1, Quest dialouge 2",
			"Quest dialouge 3, Quest dialouge 1, Quest dialouge 1, Quest dialouge 3",
			"Quest dialouge 4, Quest dialouge 1, Quest dialouge 1, Quest dialouge 4",
		};
	}
}
