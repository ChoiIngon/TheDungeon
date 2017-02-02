using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestProgress {
	public enum Type {
		KillMonster,
		DiedBy,
		MeetNpc,
		GatherItem,
		Max
	}

	public Type type {
		protected set;
		get;
	}

	public int goal;
	public int progress;
	public virtual void Start() {
		QuestManager.Instance.updates[(int)type] += this.Update;
	}

	public virtual bool IsComplete() {
		if (progress >= goal) {
			QuestManager.Instance.updates[(int)type] -= this.Update;
			return true;
		}
		return false;
	}

	public virtual void Update(string key) {
	}

}

public class QuestProgress_KillMonster : QuestProgress {
	public string monsterID;

	public QuestProgress_KillMonster() {
		type = QuestProgress.Type.KillMonster;
	}

	public override void Update(string monsterID) {
		if ("" == this.monsterID || this.monsterID == monsterID) {
			progress++;
		}
	}
}

public class QuestProgress_DiedBy : QuestProgress {
	public string reason;

	public QuestProgress_DiedBy() {
		type = QuestProgress.Type.DiedBy;
	}

	public override void Update(string reason) {
		if (this.reason == reason) {
			progress++;
		}
	}
}
/*
public class QuestCompleteCondition_MeetNpc : QuestCompleteCondition {
	public string npcID;

	public QuestCompleteCondition_MeetNpc() {
		type = QuestCompleteCondition.Type.MeetNpc;
	}

	public override void Start() {
	}

	public override bool IsComplete() {
		Object obj = Player.Instance.target;
		if(null != obj && Object.Category.NPC == obj.category) {
			Npc npc = (Npc)obj;
			if(npcID == npc.id)
			{
				return true;
			}
		}
		return false;
	}
}
*/