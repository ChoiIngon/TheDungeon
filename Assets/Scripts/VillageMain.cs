using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VillageMain : SceneMain {
    public Text log;
    public Npc npc;
	public TouchInput dungeon;
	public UIDialogBox dialogBox;
    public UITextBox textBox;
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
		yield return StartCoroutine (CheckQuest ());
    }

	IEnumerator Init()
	{
		log.text = "connect to server..";
		NetworkManager.Instance.Init ();
		log.text += "complete\n";

		ResourceManager.Instance.onDowonloadProgress += (string bundleName, float progress, int currentCount, int totalCount) => {
            downloadProgress.max = 1.0f;
            downloadProgress.current = progress;
			downloadProgress.text.text = "download.." + bundleName + " " + (int)(progress * 100) + " %";
		};
		ResourceManager.Instance.onLoadProgress += (string bundleName, string assetName) => {
			log.text += "load " + assetName + "\n";
		};

        yield return StartCoroutine(ResourceManager.Instance.Init());

		log.text += "load item configuration..";
		yield return StartCoroutine(ItemManager.Instance.Init ());
		log.text += "complete\n";
		log.text += "load monster configuration..";
		yield return StartCoroutine(MonsterManager.Instance.Init ());
		log.text += "complete\n";

		while (0.0f < log.color.a) {
			Color color = log.color;
			color.a -= Time.deltaTime;
			log.color = color;
			yield return null;
		}
		log.color = new Color (1.0f, 1.0f, 1.0f, 0.0f);
        downloadProgress.gameObject.SetActive(false);
		dungeon.gameObject.SetActive (true);
		state = State.Idle;
    }

	IEnumerator ChangeScene(string scene, GameObject target)
	{
		state = State.Popup;
		bool isSubmit = false;
		dialogBox.onSubmit += () =>  {
			isSubmit = true;
		};
		yield return StartCoroutine(dialogBox.Write("Do you want to go into the dungeon?"));
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
		//QuestData quest = QuestManager.Instance.GetAvailableQuest ();
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

		state = State.Popup;
		yield return StartCoroutine(npc.Talk(texts));
		state = State.Idle;
	}
}
