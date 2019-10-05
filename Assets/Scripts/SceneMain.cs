using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class SceneMain : MonoBehaviour {
	private bool complete = false;
	
	void Start ()
	{
		StartCoroutine(Run ());
	}

	public abstract IEnumerator Run ();

	protected IEnumerator MoveTo(GameObject go, Hashtable hashTable, bool wait = false)
	{
		if (true == wait)
		{
			hashTable.Add("oncompletetarget", gameObject);
			hashTable.Add("oncomplete", "OnComplete");
		}
		iTween.MoveTo(go, hashTable);
		while (true == wait && false == complete)
		{
			yield return null;
		}
		complete = false;
	}
	
	void OnComplete() {
		complete = true;
	}
}
