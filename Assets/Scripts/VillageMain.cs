using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VillageMain : SceneMain {
    public Text log;
	public GameObject dungeon;
	public UIDialogBox dialogBox;
	// Use this for initialization
	public override IEnumerator Run () {
		dungeon.SetActive (false);
		yield return StartCoroutine (Init ());
		dungeon.transform.FindChild("TouchInput").GetComponent<TouchInput> ().onTouchDown += (Vector3 position) => {
			iTween.ScaleTo(dungeon, iTween.Hash("scale", new Vector3(1.2f, 1.2f, 1.0f), "time", 0.1f, "easetype", iTween.EaseType.easeInOutBack));
		};
		dungeon.transform.FindChild("TouchInput").GetComponent<TouchInput> ().onTouchUp += (Vector3 position) => {
			iTween.ScaleTo(dungeon, iTween.Hash("scale", new Vector3(1.0f, 1.0f, 1.0f), "time", 0.1f, "easetype", iTween.EaseType.easeInOutBack));
			StartCoroutine(ChangeScene("Dungeon", dungeon));
		};	
    }

	IEnumerator Init()
	{
		log.text = "connect to server..";
		NetworkManager.Instance.Init ();
		log.text += "complete\n";
		log.text += "load images..";
		ResourceManager.Instance.onDowonloadProgress += (string bundleName, float progress, int curIndex, int totalCount) => {
			Debug.Log(bundleName + " " + (progress * 100) + "%, " + curIndex + "/" + totalCount);
		};
        yield return StartCoroutine(ResourceManager.Instance.Init());
		log.text += "complete\n";
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
		dungeon.SetActive (true);
	}

	IEnumerator ChangeScene(string scene, GameObject target)
	{
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
	}
}
