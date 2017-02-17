using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SceneMain : MonoBehaviour {
	public bool isComplete = false;
	public static float widthScale = 1.0f;
	public static float heightScale = 1.0f;
	void Start () {
		CanvasScaler canvasScaler = Object.FindObjectOfType<CanvasScaler> ();
		SceneMain.widthScale = Screen.width/canvasScaler.referenceResolution.x;
		SceneMain.heightScale = Screen.height/canvasScaler.referenceResolution.y;
		Debug.Log ("ui scale:{x:" + widthScale + ", y:" + heightScale + ")");
		iTween.CameraFadeAdd ();
		StartCoroutine (Run ());
	}

	public abstract IEnumerator Run ();

	void SetCameraFadeColor(Color color)
	{
		Texture2D colorTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		colorTexture.SetPixel(0, 0, color);
		colorTexture.Apply();
		iTween.CameraFadeSwap (colorTexture);
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

	void OnComplete() {
		isComplete = true;
	}
}
