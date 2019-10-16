using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest
{
	public class Reward
	{
		public int coin;
		public string item_id;
	}

	public class Dialogue
	{
		public string speaker_id;
		public string script; // 대사
	}

	public enum State
	{
		Invalid,
		AccecptWait,
		OnGoing,
		Complete,
		Rewared,
		Max
	};

	public string quest_id;
	public string quest_name;
	public int step;
	public State state = State.Invalid;

	public List<List<QuestProgress>> progresses = new List<List<QuestProgress>>();
	public List<Dialogue> start_dialogues = new List<Dialogue>();
	public List<Dialogue> complete_dialogues = new List<Dialogue>();
	public Reward reward = new Reward();

	public void AddProgress(int step, QuestProgress progress)
	{
		if (progresses.Count < step)
		{
			progresses.Resize<List<QuestProgress>>(step);
			progresses[step - 1] = new List<QuestProgress>();
		}

		List<QuestProgress> progressesInStep = progresses[step - 1];
		progressesInStep.Add(progress);
	}

	public List<QuestProgress> GetQuestProgresses()
	{
		if (step > progresses.Count)
		{
			return null;
		}

		return progresses[step - 1];
	}

	public void Start()
	{
		step = 1;
		List<QuestProgress> progressesInStep = progresses[step-1];
		foreach (QuestProgress progress in progressesInStep)
		{
			ProgressManager.Instance.Add(progress);
		}

		Util.EventSystem.Publish<Quest>(EventID.Quest_Start, this);
	}

	public void OnUpdate()
	{
		Util.EventSystem.Publish<Quest>(EventID.Quest_Update, this);
	}

	public void OnComplete()
	{
		bool complete = true;
		{
			List<QuestProgress> progressesInStep = progresses[step - 1];
			foreach (QuestProgress progress in progressesInStep)
			{
				if (progress.count < progress.goal)
				{
					complete = false;
					break;
				}
			}
		}
		if (false == complete)
		{
			return;
		}

		{
			List<QuestProgress> progressesInStep = progresses[step - 1];
			foreach (QuestProgress progress in progressesInStep)
			{
				ProgressManager.Instance.Remove(progress);
			}
		}

		if (step + 1 <= progresses.Count)
		{
			step += 1;
			List<QuestProgress> progressesInStep = progresses[step - 1];
			foreach (QuestProgress progress in progressesInStep)
			{
				ProgressManager.Instance.Add(progress);
			}
		}

		Reward reward = GetReward();
		if(null == reward)
		{
			return;
		}

		Util.EventSystem.Publish<Quest>(EventID.Quest_Complete, this);
	}

	public Reward GetReward()
	{
		if (step != progresses.Count)
		{
			Debug.LogError("not enough quest step");
			return null;
		}

		List<QuestProgress> progressesInStep = progresses[step - 1];
		foreach (QuestProgress progress in progressesInStep)
		{
			if (progress.count < progress.goal)
			{
				return null;
			}
		}

		return reward;
	}
}

public class QuestProgress : Progress
{
	public Quest quest;
	public string name;
	public override void OnUpdate()
	{
		quest.OnUpdate();
		GameManager.Instance.ui_ticker.Write(name + " " + count + "/" + goal);
	}

	public override void OnComplete()
	{
		quest.OnComplete();
	}
}

public class CompleteQuest
{
	public string id;
	public int date;
	public int count;
};

public class QuestManager : Util.Singleton<QuestManager>
{
	public Quest Find(string questID)
	{
		Quest quest = null;
		{
			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT quest_id, quest_name, reward_coin, reward_item_id FROM meta_quest WHERE quest_id='" + questID + "'"
			);
			while (true == reader.Read())
			{
				quest = new Quest();
				quest.quest_id = reader.GetString("quest_id");
				quest.quest_name = reader.GetString("quest_name");
				quest.reward.coin = reader.GetInt32("reward_coin");
				quest.reward.item_id = reader.GetString("reward_item_id");
			}

			if (null == quest)
			{
				throw new System.Exception("invalid quest(quest_id:" + questID + ")");
			}
		}
		{
			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT quest_step, progress_name, progress_type, progress_key, progress_goal FROM meta_quest_progress WHERE quest_id='" + questID + "' ORDER BY quest_step"
			);
			while (true == reader.Read())
			{
				QuestProgress progress = new QuestProgress();
				progress.name = reader.GetString("progress_name");
				progress.type = reader.GetString("progress_type");
				progress.key = reader.GetString("progress_key");
				progress.goal = reader.GetInt32("progress_goal");
				progress.quest = quest;
				quest.AddProgress(reader.GetInt32("quest_step"), progress);
			}

			if (0 == quest.progresses.Count)
			{
				throw new System.Exception("quest progress count is 0(quest_id:" + questID + ")");
			}
		}
		{
			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT dialogue_type, order_num, speaker_id, script FROM meta_quest_dialogue WHERE quest_id='" + questID + "' ORDER BY dialogue_type, order_num"
			);
			while (true == reader.Read())
			{
				Quest.Dialogue dialogue = new Quest.Dialogue();
				dialogue.speaker_id = reader.GetString("speaker_id");
				dialogue.script = reader.GetString("script");
				int dialogueType = reader.GetInt32("dialogue_type");

				if (1 == dialogueType) // start
				{
					quest.start_dialogues.Add(dialogue);
				}
				else //complete
				{
					quest.complete_dialogues.Add(dialogue);
				}
			}
		}
		
		return quest;
	}

	public Quest GetAvailableQuest()
	{
		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT quest_id FROM meta_quest ORDER BY RANDOM() LIMIT 1"
		);

		while (true == reader.Read())
		{
			return Find(reader.GetString("quest_id"));
		}
		return null;
	}
}