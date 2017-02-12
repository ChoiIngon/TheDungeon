using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VillageMain : MonoBehaviour {
	public Button start;
    public Text log;
    public WWW www;
	// Use this for initialization
	void Start () {
		start.gameObject.SetActive (false);
		StartCoroutine (Init ());
        
    }

	IEnumerator Init()
	{
		log.text = "connect to server..";
		NetworkManager.Instance.Init ();
		yield return new WaitForSeconds (0.5f);
		log.text += "complete\n";
		log.text += "load images..";
		ResourceManager.Instance.Init ();
		yield return new WaitForSeconds (0.5f);
		log.text += "complete\n";
		log.text += "load item configuration..";
		yield return StartCoroutine(ItemManager.Instance.Init ());
		yield return new WaitForSeconds (0.5f);
		log.text += "complete\n";
		log.text += "load monster configuration..";
		MonsterManager.Instance.Init ();
		yield return new WaitForSeconds (0.5f);
		log.text += "complete\n";

		start.gameObject.SetActive (true);
		start.onClick.AddListener(() => {
			SceneManager.LoadScene("Dungeon");
		});
	}
}
