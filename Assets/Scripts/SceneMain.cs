using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class SceneMain : MonoBehaviour {
	[ReadOnly] public bool isComplete = false;
	
	void Start ()
	{
		StartCoroutine(Run ());
	}

	public abstract IEnumerator Run ();

	protected IEnumerator PunchPosition (GameObject go, Hashtable hashTable, bool wait = false) {
		if (true == wait) {
			hashTable.Add ("oncompletetarget", gameObject);
			hashTable.Add ("oncomplete", "OnComplete");
		}
		iTween.PunchPosition (go, hashTable);
		while (true == wait && false == isComplete) {
			yield return null;
		}
		isComplete = false;
	}

	protected IEnumerator MoveTo (GameObject go, Hashtable hashTable, bool wait = false) {
		if (true == wait) {
			hashTable.Add ("oncompletetarget", gameObject);
			hashTable.Add ("oncomplete", "OnComplete");
		}
		iTween.MoveTo (go, hashTable);
		while (true == wait && false == isComplete) {
			yield return null;
		}
		isComplete = false;
	}
	protected IEnumerator MoveBy(GameObject go, Hashtable hashTable, bool wait = false) {
		if (true == wait) {
			hashTable.Add ("oncompletetarget", gameObject);
			hashTable.Add ("oncomplete", "OnComplete");
		}
		iTween.MoveBy (go, hashTable);
		while (true == wait && false == isComplete) {
			yield return null;
		}
		isComplete = false;
	}
	protected IEnumerator ScaleTo (GameObject go, Hashtable hashTable, bool wait = false) {
		if (true == wait) {
			hashTable.Add ("oncompletetarget", gameObject);
			hashTable.Add ("oncomplete", "OnComplete");
		}
		iTween.ScaleTo (go, hashTable);
		while (true == wait && false == isComplete) {
			yield return null;
		}
		isComplete = false;
	}

	protected IEnumerator ShakePosition(GameObject go, Hashtable hashTable, bool wait = false) {
		if (true == wait) {
			hashTable.Add ("oncompletetarget", gameObject);
			hashTable.Add ("oncomplete", "OnComplete");
		}
		iTween.PunchPosition (go, hashTable);
		while (true == wait && false == isComplete) {
			yield return null;
		}
		isComplete = false;	
	}

	protected IEnumerator ValueTo(Color from, Color to, float time)
	{
		Hashtable hashTable = iTween.Hash("from", from, "to", to, "time", time);
		hashTable.Add("oncompletetarget", gameObject);
		hashTable.Add("oncomplete", "OnComplete");

		iTween.ValueTo(gameObject, hashTable);
		while (false == isComplete)
		{
			yield return null;
		}
		isComplete = false;
	}
	void OnComplete() {
		isComplete = true;
	}
}
