using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest_Example : QuestData {
	public Quest_Example()
	{
		this.id = "QUEST_EXAMPLE";
		this.name = "Example Quest";
		this.state = QuestData.State.Invalid;
		this.triggers.Add (new QuestTrigger_LessCompleteQuestCount (0));
		this.progresses.Add (new QuestProgress ("Go into the dungeon", QuestProgress.Type.CrrentLocation, "Dungeon", 1));
		startDialouge = new Dialouge ();
		startDialouge.speaker = "Far Seer";
		startDialouge.dialouge = new string[] {
			"Welcome to \'Last Village\'\nYour first quest is go to the dungeon and reach the last level\n",
			"Touch enter button"
		};
	}
}
