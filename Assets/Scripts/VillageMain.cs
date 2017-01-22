using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class VillageMain : MonoBehaviour {
	Button start;
	// Use this for initialization
	void Start () {
		start = GetComponent<Button> ();
		start.onClick.AddListener(() => {
			SceneManager.LoadScene("Dungeon");
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
