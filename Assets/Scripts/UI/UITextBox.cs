using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextBox : MonoBehaviour {
	public enum State
	{
		Hide,
		Raise,
		Typing,
		Complete,
		Fall
	}
	public State state;
	public int lineCount;
	public float charPerSecond;
	public float moveSpeed;
	public AudioSource audioSource;

	private Text contents;
	private string copied;
	private ScrollRect scrollRect;
	private Button button;
	private float height;
	// Use this for initialization
	void Start () {
		button = transform.FindChild ("Button").GetComponent<Button> ();
		button.gameObject.SetActive (false);
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
		state = State.Hide;
	}
		
	public IEnumerator Write(string text)
	{
		state = State.Raise;
		RectTransform rt = GetComponent<RectTransform> ();
		height = rt.rect.height;

		contents.text = "";
		while (height >= rt.anchoredPosition.y) {
			Vector2 position = rt.anchoredPosition;
			position.y += height * Time.deltaTime * moveSpeed;
			rt.anchoredPosition = position;
			yield return null;
		}

		rt.anchoredPosition = new Vector2 (rt.anchoredPosition.x, height);

		state = State.Typing;
		button.onClick.AddListener (WriteAll);
		button.gameObject.SetActive (true);

		foreach (char c in text)
		{
			audioSource.Play();
			contents.text += c;
			if(null != scrollRect)
			{
				scrollRect.verticalNormalizedPosition = 0.0f;
			}
			yield return new WaitForSeconds(1.0f / charPerSecond);
			if (State.Complete == state) {
				contents.text = text;
				break;
			}
		}

		state = State.Complete;
		while (State.Fall != state) {
			scrollRect.verticalNormalizedPosition = 0.0f;
			yield return null;
		}

		while (0 < transform.position.y) {
			Vector3 position = transform.position;
			position.y -= height * Time.deltaTime * moveSpeed;
			transform.position = position;
			yield return null;
		}
		transform.position = new Vector3 (transform.position.x, 0.0f, 0.0f);
	}

	public void WriteAll()
	{
		state = State.Complete;
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener(HideTextBox);
	}

	public void HideTextBox()
	{
		state = State.Fall;
		button.onClick.RemoveAllListeners ();
		button.gameObject.SetActive (false);
	}
}
