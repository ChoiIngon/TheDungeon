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
        start.onClick.AddListener(() => {
            SceneManager.LoadScene("Dungeon");
        });

        NetworkManager.Instance.Init();
    }
}
