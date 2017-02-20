using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour {
	public IEnumerator Start()
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync("Village", LoadSceneMode.Additive);	
		while (false == operation.isDone) {
			// loading progress
			yield return null;
		}
	}
}
