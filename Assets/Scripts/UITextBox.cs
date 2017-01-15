using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextBox : MonoBehaviour {
	public int lineCount;
	public float charPerSecond;
	public float moveSpeed;
	public string text {
		set {
			copied = value;
			coroutine = StartCoroutine(Type());
		}
	}
	private static UITextBox _instance;  
	public static UITextBox Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (UITextBox)GameObject.FindObjectOfType(typeof(UITextBox));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "UITextBox";  
					_instance = container.AddComponent<UITextBox>();  
				}  
			}  
			return _instance;  
		}  
	}
	Text contents;
	string copied;
	ScrollRect scrollRect;
	AudioSource audioSource;
	Button button;
	Coroutine coroutine;
	float height;
	// Use this for initialization
	void Start () {
		button = transform.FindChild ("Button").GetComponent<Button> ();
		button.gameObject.SetActive (false);
		button.onClick.AddListener (() => {
			if(null != coroutine)
			{
				StopCoroutine(coroutine);
				coroutine = null;
				contents.text = copied;
			}
			else
			{
				button.gameObject.SetActive(false);
				StartCoroutine(DownTextBox());
			}
		});
	
		contents = transform.FindChild ("ScrollView/Viewport/Contents").GetComponent<Text> ();
		audioSource = GetComponent<AudioSource> ();

		scrollRect = transform.FindChild ("ScrollView").GetComponent<ScrollRect> ();
		{
			RectTransform rt = scrollRect.GetComponent<RectTransform> ();
			contents.fontSize = (int)(rt.rect.height / lineCount - contents.lineSpacing);
		}
		{
			RectTransform rt = GetComponent<RectTransform> ();
			height = rt.rect.height;
		}
		{
			RectTransform rt = button.GetComponent<RectTransform> ();
			rt.sizeDelta = new Vector2 (rt.sizeDelta.x, Screen.height + height);
		}
	}
		
	IEnumerator Type()
	{
		contents.text = "";
		while (0.0f >= transform.position.y) {
			Vector3 position = transform.position;
			position.y += height * Time.deltaTime * 2.0f;
			transform.position = position;
			yield return null;
		}
		button.gameObject.SetActive (true);
		foreach (char c in copied)
		{
			audioSource.Play();
			contents.text += c;
			if(null != scrollRect)
			{
				scrollRect.verticalNormalizedPosition = 0.0f;
			}

			yield return new WaitForSeconds(1.0f / charPerSecond);
		}
		coroutine = null;
	}

	IEnumerator DownTextBox()
	{
		while (-height < transform.position.y) {
			Vector3 position = transform.position;
			position.y -= height * Time.deltaTime * 2.0f;
			transform.position = position;
			yield return null;
		}
		transform.position = new Vector3 (transform.position.x, -height, 0.0f);
	}
}
