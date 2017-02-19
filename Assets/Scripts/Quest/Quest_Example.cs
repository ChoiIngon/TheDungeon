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
			new QuestProgress () { type = "EnterDungeon", key = "", goal = 1, progress = 0 }
		};
		startDialouge = new Dialouge ();
		startDialouge.speacker = "Far Seer";
		startDialouge.dialouge = new string[] {
			"Welcome to \'Last Village\'\nYour first quest is go to the dungeon and reach the last level\n",
			"Touch enter button"
		};
	}
}
