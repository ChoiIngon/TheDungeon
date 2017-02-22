using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIDialogBox : MonoBehaviour {
	public Button submit;
	public Button cancel;

	public event System.Action onSubmit;
	public Text content;
	private RectTransform rectTransform;
	private bool active;
	private static UIDialogBox _instance;  
	public static UIDialogBox Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (UIDialogBox)GameObject.FindObjectOfType(typeof(UIDialogBox));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "UIDialogBox";  
					_instance = container.AddComponent<UIDialogBox>();  
				}  
				_instance.Init ();
				//DontDestroyOnLoad (_instance);
			}  

			return _instance;  
		}  
	}
	// Use this for initialization
	void Init () {
		rectTransform = GetComponent<RectTransform> ();
		gameObject.SetActive (false);
		submit.onClick.AddListener (Submit);
		cancel.onClick.AddListener (Close);
		active = false;
	}

	private void Submit() {
		content.text = "";
		gameObject.SetActive(false);
		if (null != onSubmit) {
			onSubmit ();
		}
		active = false;
		onSubmit = null;
	}

	private void Close() {
		content.text = "";
		gameObject.SetActive(false);
		active = false;
		onSubmit = null;
	}
		
	public int width {
		set {
			rectTransform.sizeDelta = new Vector2 (value, rectTransform.rect.height);
		}
	}

	public IEnumerator Write(string text, TextAnchor alinement = TextAnchor.MiddleLeft)
	{
		active = true;
		gameObject.SetActive(true);
		content.text = text;
		content.alignment = alinement;
		rectTransform.sizeDelta = new Vector2 (rectTransform.rect.width, content.preferredHeight+100);

		while(active)
		{
			yield return null;
		}
	}
}