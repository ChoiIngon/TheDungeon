using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class SceneMain : MonoBehaviour {
	[ReadOnly] public bool isComplete = false;
	[ReadOnly] public Texture2D fadeColorTexture;
	void Start ()
	{
		iTween.CameraFadeAdd ();
		fadeColorTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		StartCoroutine(CameraFadeFrom(Color.black, iTween.Hash("amount", 1.0f, "time", 1.0f)));
	    //UITicker.Instance.gameObject.SetActive(false);
        StartCoroutine(Run ());
	}

	public abstract IEnumerator Run ();

	void SetCameraFadeColor(Color color)
	{
		fadeColorTexture.SetPixel(0, 0, color);
        fadeColorTexture.Apply();
		iTween.CameraFadeSwap (fadeColorTexture);
	}

	protected IEnumerator CameraFadeTo (Color color, Hashtable hashTable, bool wait = false) {
		SetCameraFadeColor (color);
		if (true == wait) {
			hashTable.Add ("oncompletetarget", gameObject);
			hashTable.Add ("oncomplete", "OnComplete");
		}

		iTween.CameraFadeTo (hashTable);
		while (true == wait && false == isComplete) {
			yield return null;
		}
		isComplete = false;
	}

	protected IEnumerator CameraFadeFrom (Color color, Hashtable hashTable, bool wait = false) 	{
		SetCameraFadeColor (color);
		if (true == wait) {
			hashTable.Add ("oncompletetarget", gameObject);
			hashTable.Add ("oncomplete", "OnComplete");
		}

		iTween.CameraFadeFrom (hashTable);
		while (true == wait && false == isComplete) {
			yield return null;
		}
		isComplete = false;
	}

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

	protected IEnumerator ValueTo(GameObject go, float amount, float time)
	{
		Hashtable hashTable = iTween.Hash("amount", amount, "time", time);
		hashTable.Add("oncompletetarget", gameObject);
		hashTable.Add("oncomplete", "OnComplete");

		iTween.FadeTo(go, hashTable);
		while (false == isComplete)
		{
			yield return null;
		}
		isComplete = false;
		/*
		float color = 0.0f;
		yield return new WaitForSeconds(time * 0.6f);
		while (1.0f > color)
		{
			renderer.color = new Color(color, color, color, 1.0f);
			color += Time.deltaTime / (time - time * 0.6f);
			yield return null;
		}
		renderer.color = Color.white;
		*/
	}
	void OnComplete() {
		isComplete = true;
	}
}
