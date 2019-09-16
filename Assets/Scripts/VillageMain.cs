using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VillageMain : SceneMain {
	public TouchInput dungeon;
    public UIGaugeBar downloadProgress;

	public enum State
	{
		Invalid, Idle, Popup, Max
	}
	public State state {
		set {
			switch (value) {
			case State.Invalid:
				dungeon.enabled = false;
				break;
			case State.Idle:
				dungeon.enabled = true;
				break;
			case State.Popup:
				dungeon.enabled = false;
				break;
			default :
				throw new System.Exception ("undefined state : " + value.ToString ());
			}
		}
	}

	public override IEnumerator Run () {
		state = State.Invalid;
		dungeon.gameObject.SetActive (false);
		dungeon.onTouchDown += (Vector3 position) => {
			iTween.ScaleTo(dungeon.gameObject, iTween.Hash("scale", new Vector3(1.2f, 1.2f, 1.0f), "time", 0.1f, "easetype", iTween.EaseType.easeInOutBack));
		};
		dungeon.onTouchUp += (Vector3 position) => {
			iTween.ScaleTo(dungeon.gameObject, iTween.Hash("scale", new Vector3(1.0f, 1.0f, 1.0f), "time", 0.1f, "easetype", iTween.EaseType.easeInOutBack));
			StartCoroutine(ChangeScene("Dungeon", dungeon.gameObject));
		};
		yield return StartCoroutine (Init ());

		ProgressManager.Instance.Update (Progress.Type.CrrentLocation, "Village");
		yield return StartCoroutine (CheckQuest ());
    }

	IEnumerator Init()
	{
		/*
		NetworkManager.Instance.Init ();
		UILog.Instance.Write ("connect to server complete");;
		ResourceManager.Instance.onDowonloadProgress += (string bundleName, float progress, int currentCount, int totalCount) => {
            downloadProgress.max = 1.0f;
            downloadProgress.current = progress;
			downloadProgress.text.text = "download.." + bundleName + " " + (int)(progress * 100) + " %";
		};
		ResourceManager.Instance.onLoadProgress += (string bundleName, string assetName) => {
			UILog.Instance.Write (assetName);
		};
        yield return StartCoroutine(ResourceManager.Instance.Init());

		yield return StartCoroutine(ItemManager.Instance.Init ());
		UILog.Instance.Write("load item complete");

		yield return StartCoroutine(MonsterManager.Instance.Init ());
		UILog.Instance.Write ("load monster complete");
		yield return StartCoroutine(QuestManager.Instance.Init ());
		QuestManager.Instance.onComplete += (QuestData data) => {
			StartCoroutine(OnCompleteQuest(data));
		};
		UILog.Instance.Write ("load quest complete");
		Player.Instance.Init ();
		UICoin.Instance.Init ();
		yield return StartCoroutine (UILog.Instance.Hide (1.0f));
        downloadProgress.gameObject.SetActive(false);
		dungeon.gameObject.SetActive (true);
		state = State.Idle;
		*/
		yield break;
    }

	IEnumerator ChangeScene(string scene, GameObject target)
	{
		state = State.Popup;
		bool isSubmit = false;
		GameManager.Instance.ui_dialogbox.onSubmit += () =>  {
			isSubmit = true;
		};
		yield return StartCoroutine(GameManager.Instance.ui_dialogbox.Write("Do you want to go into the dungeon?"));
		if(true == isSubmit)
		{
			StartCoroutine(CameraFadeTo(Color.black, iTween.Hash("amount", 1.0f, "time", 2.0f)));
			Vector3 position = Camera.main.transform.position;
			yield return StartCoroutine(MoveTo (Camera.main.gameObject, iTween.Hash(
				"x", target.transform.position.x,
				"y", target.transform.position.y,
				"z", target.transform.position.z-0.5f,
				//"easetype", iTween.EaseType.easeInExpo,
				"time", 2.0f
			), true));
			Camera.main.transform.position = position;
			SceneManager.LoadScene(scene);
		}
		state = State.Idle;
	}

	IEnumerator CheckQuest()
	{
		{
			/*
			QuestData quest = ProgressManager.Instance.GetAvailableQuest ();
			if (null != quest && null != quest.start_dialogue) {
				state = State.Popup;
				yield return StartCoroutine (GameManager.Instance.ui_npc.Talk (quest.start_dialogue.speaker, quest.start_dialogue.scripts));
				state = State.Idle;
			}
			*/
		}
		/*
		string[] texts = new string[] {
			"Many heroes of all kinds ventured into the Dungeon before you.\n",
			"Some of them have returned with treasures and magical artifacts, most have never been heard of ever since.\n",
			"But none has succeeded in retrieving the Amulet of Yendor, which is told to be hidden in the depths of Pixel Dungeon.\n",
			"You consider yourself ready for the challenge, but most importantly you feel that fortune smiles upon you.\n",
			"It's time to start your own adventure!\r\nThe Dungeon lies right beneath the City, its upper levels are actually constitute the City's sewer system.\n",
			"Being nominally a part of the City, these levels are not that dangerous. No one will call them a safe place, but at least you won't need to deal with evil magic here.\n",
			"Many years ago an underground prison for the most dangerous criminals was built here. At that moment it seemed a very clever idea, this place, indeed, was very hard to escape.\n",
			"But soon dark miasma started to permeate from below, driving prisoners and guards insane.\n",
			"In the end the prison was abandoned, though some convicts were left locked here.\n",
			"The caves, which stretch down under the abandoned prison, are sparcely populated. They lie too deep to be exploited by the City and they are too poor in minerals to interest the dwarves.\n",
			"In the past there was a trade outpost somewhere here on the route between these two states, but it perished after the decline of Dwarven Metropolis.\n",
			"Only omnipresent gnolls and subterranean animals dwell here.\n",
			"Dwarven Metropolis was once the greatest of dwarven city-states. In its heyday the mechanized army of dwarves has successfully repelled the invasion of the old god and his demon army.\n",
			"But it is said,that the returning warriors have brought seeds of corruption with them and that victory was the beginning of the end for the underground kingdom.\r\n",
			"In the past these levels were the outskirts of Metropolis. After the costly victory in the war with the old god dwarves were too weakened to clear them of remaining demons. Gradually demons have consolidated their grip on this place and now it's called Demon Halls.\r\n",
			"Very few adventurers ever descended this far..."
		};

		//state = State.Popup;
		//yield return StartCoroutine(npc.Talk(texts));
		//state = State.Idle;
		*/
		yield break;
	}
	IEnumerator OnCompleteQuest(QuestData quest)
	{
		//state = State.Popup;
		//foreach(QuestData quest in completeQuests)
		{
			/*
			if (null == quest.complete_dialogue) {
				yield break;
			}
			if (null == quest.complete_dialogue.scripts) {
				yield break;
			}

			state = State.Popup;
			yield return StartCoroutine (GameManager.Instance.ui_npc.Talk (quest.complete_dialogue.speaker, quest.complete_dialogue.scripts));
			state = State.Idle;
			*/
		}
		//	completeQuests.Clear ();
		//state = State.Idle;
		yield break;
	}
}
