using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextBox : MonoBehaviour {
	public int lineCount;
	public float charPerSecond;
	Text contents;
	string copied;
	ScrollRect scrollRect;
	AudioSource audioSource;
	Button button;
	Coroutine coroutine;
	public string text {
		set {
			copied = value;
			coroutine = StartCoroutine(Type());
		}
	}
	// Use this for initialization
	void Start () {
		button = transform.FindChild ("Button").GetComponent<Button> ();
		button.onClick.AddListener (() => {
			if(null != coroutine)
			{
				StopCoroutine(coroutine);
				contents.text = copied;
				button.gameObject.SetActive(false);
			}
		});
	
		contents = transform.FindChild ("ScrollView/Viewport/Contents").GetComponent<Text> ();
		audioSource = GetComponent<AudioSource> ();

		scrollRect = transform.FindChild ("ScrollView").GetComponent<ScrollRect> ();
		RectTransform rt = scrollRect.GetComponent<RectTransform>();
		contents.fontSize = (int)(rt.rect.height / lineCount - contents.lineSpacing);
	}

	IEnumerator Type()
	{
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

	}
}
