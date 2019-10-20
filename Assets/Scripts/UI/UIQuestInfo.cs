using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour
{
	// Start is called before the first frame update
	private Text contents;
	private void Awake()
	{
		contents = GetComponent<Text>();
		contents.text = "";
		Util.EventSystem.Subscribe<Quest>(EventID.Quest_Start, OnQuestStart);
		Util.EventSystem.Subscribe<Quest>(EventID.Quest_Update, OnQuestUpdate);
		Util.EventSystem.Subscribe<Quest>(EventID.Quest_Complete, OnQuestComplete);
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<Quest>(EventID.Quest_Start, OnQuestStart);
		Util.EventSystem.Unsubscribe<Quest>(EventID.Quest_Update, OnQuestUpdate);
		Util.EventSystem.Unsubscribe<Quest>(EventID.Quest_Complete, OnQuestComplete);
	}

	private void OnQuestStart(Quest quest)
	{
		contents.text = "";
		GameManager.Instance.ui_textbox.on_close += () => 
		{
			contents.text += "<B>" + quest.quest_name + "</B>\n";
			List<QuestProgress> progresses = quest.GetQuestProgresses();
			foreach (QuestProgress progress in progresses)
			{
				contents.text += progress.name + " " + progress.count + "/" + progress.goal + "\n";
			}
		};
		StartCoroutine(GameManager.Instance.ui_npc.Write(quest.start_dialogues));
	}
	private void OnQuestUpdate(Quest quest)
	{
		contents.text = "";
		contents.text += "<B>" + quest.quest_name + "</B>\n";
		List<QuestProgress> progresses = quest.GetQuestProgresses();
		foreach (QuestProgress progress in progresses)
		{
			if (progress.count >= progress.goal)
			{
				contents.text += "<color=#DDDDDD>" + progress.name + " 완료\n";
			}
			else
			{
				contents.text += progress.name + " " + progress.count + "/" + progress.goal + "\n";
			}
		}
	}

	private void OnQuestComplete(Quest quest)
	{
		contents.text = "";
		Quest.Reward reward = quest.GetReward();
		quest.complete_dialogues.Add(new Quest.Dialogue()
		{
			speaker_id = "",
			script = "퀘스트 완료 보상\n" +
			(reward.coin == 0 ? "" : ("Coin : " + reward.coin))
		});
		StartCoroutine(GameManager.Instance.ui_npc.Write(quest.complete_dialogues));
	}
}
