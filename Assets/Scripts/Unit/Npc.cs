using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Npc : MonoBehaviour {
	public UITextBox textBox;
	public Image panel;
	public Image image;
	public Sprite sprite {
		set {
			image.sprite = value;
		}
	}

	// Use this for initialization
	void Start () {
		gameObject.SetActive (false);
	}

	public IEnumerator Talk(string text)
	{
		gameObject.SetActive (true);
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * SceneMain.widthScale, "easeType", "easeInOutExpo"));
		yield return StartCoroutine(textBox.Write(text));
		iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
		yield return StartCoroutine (textBox.Hide (0.5f));
	}

	public IEnumerator Talk(string[] text)
	{
		gameObject.SetActive (true);
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * SceneMain.widthScale, "easeType", "easeInOutExpo"));
		yield return StartCoroutine(textBox.Write(text));
		iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
		yield return StartCoroutine (textBox.Hide (0.5f));
	}
}
