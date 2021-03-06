﻿using UnityEngine;
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
		public string sprite_path;
		public string text; // 대사
	}

	public string quest_id;
	public string quest_name;
	public string sprite_path;
	public int step;
	
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
			Util.EventSystem.Publish<Quest>(EventID.Quest_Update, this);
		}

		Reward reward = GetReward();
		if(null == reward)
		{
			return;
		}

		if (0 < reward.coin)
		{
			GameManager.Instance.player.ChangeCoin(reward.coin);
		}
		if ("" != reward.item_id)
		{
			GameManager.Instance.player.inventory.Add(ItemManager.Instance.CreateItem(reward.item_id));
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

public class QuestManager : Util.Singleton<QuestManager>
{
	private Dictionary<string, Quest> quests = new Dictionary<string, Quest>();
	public void Init()
	{
		try
		{
			GoogleSheetReader sheetReader = new GoogleSheetReader(GameManager.GOOGLESHEET_ID, GameManager.GOOGLESHEET_API_KEY);
			sheetReader.Load("meta_quest");

			foreach (GoogleSheetReader.Row row in sheetReader)
			{
				Quest quest = new Quest();
				quest.quest_id = row.GetString("quest_id");
				quest.quest_name = row.GetString("quest_name");
				quest.sprite_path = row.GetString("sprite_path");
				quest.reward.coin = row.GetInt32("reward_coin");
				quest.reward.item_id = row.GetString("reward_item_id");
				quests.Add(quest.quest_id, quest);
			}
		}
		catch (System.Exception e)
		{
			GameManager.Instance.ui_textbox.on_close = () =>
			{
				Application.Quit();
			};
			GameManager.Instance.ui_textbox.AsyncWrite("error: " + e.Message + "\n" + e.ToString(), false);
		}
	}
	public Quest Find(string questID)
	{
		Quest quest = null;
		{
			Util.Sqlite.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT quest_id, quest_name, reward_coin, reward_item_id, sprite_path FROM meta_quest WHERE quest_id='" + questID + "'"
			);
			while (true == reader.Read())
			{
				quest = new Quest();
				quest.quest_id = reader.GetString("quest_id");
				quest.quest_name = reader.GetString("quest_name");
				quest.sprite_path = reader.GetString("sprite_path");
				quest.reward.coin = reader.GetInt32("reward_coin");
				quest.reward.item_id = reader.GetString("reward_item_id");
			}

			if (null == quest)
			{
				throw new System.Exception("invalid quest(quest_id:" + questID + ")");
			}
		}
		{
			Util.Sqlite.DataReader reader = Database.Execute(Database.Type.MetaData,
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
			Util.Sqlite.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT dialogue_type, dialogue_num, dialogue_text, sprite_path FROM meta_quest_dialogue WHERE quest_id='" + questID + "' ORDER BY dialogue_type, dialogue_num"
			);
			while (true == reader.Read())
			{
				Quest.Dialogue dialogue = new Quest.Dialogue();
				dialogue.sprite_path = reader.GetString("sprite_path");
				dialogue.text = reader.GetString("dialogue_text");
				int dialogueType = reader.GetInt32("dialogue_type");

				switch (dialogueType)
				{
					case 1:
						quest.start_dialogues.Add(dialogue);
						break;
					case 2:
						break;
					case 3:
						quest.complete_dialogues.Add(dialogue);
						break;
				}				
			}
		}
		
		return quest;
	}

	public Quest GetAvailableQuest()
	{
		Util.Sqlite.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT quest_id FROM meta_quest ORDER BY RANDOM() LIMIT 1"
		);

		while (true == reader.Read())
		{
			return Find(reader.GetString("quest_id"));
		}
		return null;
	}
}