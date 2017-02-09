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
	IEnumerator Start () {
        start.onClick.AddListener(() => {
            SceneManager.LoadScene("Dungeon");
        });

        string url = "http://ec2-52-221-224-126.ap-southeast-1.compute.amazonaws.com/~wered/check_account_exist.php?env=dev&account_id=test&account_type=1";
        log.text = "start www connect\n";
        www = new WWW(url);
        yield return www;
        log.text += "success www connect\n";
        log.text = "result:" + www.text;
    }
}
