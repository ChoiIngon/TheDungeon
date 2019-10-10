using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINpc : MonoBehaviour {
	public Image image;
	public Sprite sprite {
		set {
			image.sprite = value;
		}
	}
	
	float widthScale = 1.0f;
	float heightScale = 1.0f;
	public void Init()
	{
		CanvasScaler canvasScaler = Object.FindObjectOfType<CanvasScaler> ();
		widthScale = Screen.width/canvasScaler.referenceResolution.x;
		heightScale = Screen.height/canvasScaler.referenceResolution.y;
	}
	public IEnumerator Talk(string text)
	{
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * widthScale, "easeType", "easeInOutExpo"));
		GameManager.Instance.ui_textbox.on_close += () =>
		{
			iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
		};
		yield return StartCoroutine(GameManager.Instance.ui_textbox.TypeWrite(text));
		GameManager.Instance.ui_textbox.on_close = null;
	}

	public IEnumerator Talk(string speaker, string[] text)
	{
		gameObject.SetActive (true);
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * widthScale, "easeType", "easeInOutExpo"));
		GameManager.Instance.ui_textbox.on_close += () =>
		{
			iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
		};
		yield return StartCoroutine(GameManager.Instance.ui_textbox.TypeWrite(text));
		GameManager.Instance.ui_textbox.on_close = null;
	}
}
