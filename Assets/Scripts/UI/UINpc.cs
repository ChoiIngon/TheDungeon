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
	private static UINpc _instance;  
	public static UINpc Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (UINpc)GameObject.FindObjectOfType(typeof(UINpc));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "UINpc";  
					_instance = container.AddComponent<UINpc>();  
				}  
				_instance.Init ();
				//DontDestroyOnLoad (_instance);
			}  

			return _instance;  
		}  
	}	 

	float widthScale = 1.0f;
	float heightScale = 1.0f;
	void Init()
	{
		CanvasScaler canvasScaler = Object.FindObjectOfType<CanvasScaler> ();
		widthScale = Screen.width/canvasScaler.referenceResolution.x;
		heightScale = Screen.height/canvasScaler.referenceResolution.y;
	}
	public IEnumerator Talk(string text)
	{
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * widthScale, "easeType", "easeInOutExpo"));
		yield return StartCoroutine(UITextBox.Instance.Write(text));
		iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
	}

	public IEnumerator Talk(string[] text)
	{
		gameObject.SetActive (true);
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * widthScale, "easeType", "easeInOutExpo"));
		yield return StartCoroutine(UITextBox.Instance.Write(text));
		iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
	}
}
