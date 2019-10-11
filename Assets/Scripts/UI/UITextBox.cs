using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextBox : MonoBehaviour {
	public enum State
	{
		Hide,
		Raise,
		Typing,
		Next,
		Complete
	}
	public State state;
	public int lineCount;
	public float charPerSecond;
	public float time;
	
	public ScrollRect scrollRect;
	public Text contents;
	public Button fast;
	public Button next;
	public Button close;
	public Button submit;
	public Button cancel;


	private string copied;
	private float height;
	private RectTransform rectTransform;
	private Vector3 position;
	private int paragraph;
	private Coroutine hideCoroutine;

	public System.Action on_submit;
	public System.Action on_close;
	public void Init ()
	{
		rectTransform = GetComponent<RectTransform>();
		height = rectTransform.rect.height + rectTransform.anchoredPosition.y;
		position = rectTransform.anchoredPosition;

		Rect parentRect = transform.parent.GetComponent<RectTransform>().rect;

		fast.gameObject.SetActive (false);
		next.gameObject.SetActive (false);
		close.gameObject.SetActive (false);
		submit.gameObject.SetActive(false);
		cancel.gameObject.SetActive(false);

		fast.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, parentRect.height);
		next.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, parentRect.height);
		close.GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, parentRect.height);

		fast.onClick.AddListener (ScrollDown);
		next.onClick.AddListener (() => { state = State.Next; Debug.Log(state.ToString()); });
		close.onClick.AddListener(Close);
		submit.onClick.AddListener(() =>
		{
			submit.gameObject.SetActive(false);
			cancel.gameObject.SetActive(false);
			ScrollDown();
			on_submit?.Invoke();
			on_submit = null;
		});
		cancel.onClick.AddListener(Close);

		state = State.Hide;
		Debug.Log(state.ToString());
	}

	public IEnumerator Show()
	{
		Util.EventSystem.Publish(EventID.TextBox_Open);
		state = State.Raise;
		Debug.Log(state.ToString());
		contents.text = "";
		while (height > rectTransform.anchoredPosition.y)
		{
			Vector2 position = rectTransform.anchoredPosition;
			position.y += height * (Time.deltaTime/time);
			rectTransform.anchoredPosition = position;
			yield return null;
		}
		rectTransform.anchoredPosition = new Vector2 (rectTransform.anchoredPosition.x, height);
	}

	public IEnumerator Hide()
	{
		close.gameObject.SetActive(false);
		//iTween.Stop (close.gameObject);
		Util.EventSystem.Publish(EventID.TextBox_Close);
		state = State.Hide;
		Debug.Log(state.ToString());
		while (position.y < rectTransform.anchoredPosition.y)
		{
			Vector2 position = rectTransform.anchoredPosition;
			position.y -= height * (Time.deltaTime/time);
			rectTransform.anchoredPosition = position;
			yield return null;
		}
		rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, position.y, 0.0f);
	}

	public void LogWrite(string text)
	{
		if (null != hideCoroutine)
		{
			StopCoroutine(hideCoroutine);
		}
		if (State.Hide == state)
		{
			StartCoroutine(Show());
		}
		state = State.Typing;
		Debug.Log(state.ToString());

		fast.gameObject.SetActive(false);
		next.gameObject.SetActive(false);
		close.gameObject.SetActive(false);

		contents.text += text + "\n";
		scrollRect.verticalNormalizedPosition = 0.0f;
	}

	public void AddClose()
	{
		ScrollDown();
	}

	public void Close()
	{
		on_close?.Invoke();
		hideCoroutine = StartCoroutine(Hide());
		on_close = null;
	}
	public IEnumerator TypeWrite(string text)
	{
		contents.text = "";
		if (null != hideCoroutine) {
			StopCoroutine (hideCoroutine);
		}

		fast.gameObject.SetActive(true);
		next.gameObject.SetActive(false);
		close.gameObject.SetActive(false);
		submit.gameObject.SetActive(false);
		cancel.gameObject.SetActive(false);

		Debug.Log("text box state:" + state);
		if (State.Hide == state) {
			yield return StartCoroutine (Show());
		}

		state = State.Typing;
		Debug.Log(state.ToString());
		AudioManager.Instance.Play("textbox_type", true);
		foreach (char c in text)
		{
			contents.text += c;
			if(null != scrollRect)
			{
				scrollRect.verticalNormalizedPosition = 0.0f;
			}
			if (State.Complete != state) {
				yield return new WaitForSeconds (1.0f / charPerSecond);
			}
		}
		AudioManager.Instance.Stop("textbox_type");
		ScrollDown();

		if (null != on_submit)
		{
			close.gameObject.SetActive(false);
			submit.gameObject.SetActive(true);
			cancel.gameObject.SetActive(true);
		}

		while (State.Complete == state) {
			yield return null;
		}

		on_submit = null;
		yield return hideCoroutine;
	}

	public IEnumerator TypeWrite(string[] texts)
	{
		paragraph = texts.Length - 1;
		foreach (string text in texts) {
			yield return StartCoroutine (TypeWrite (text));
			paragraph = Mathf.Max(0, paragraph-1);
		}
	}
	
	void ScrollDown()
	{
		scrollRect.verticalNormalizedPosition = 0.0f;
		state = State.Complete;
		Debug.Log(state.ToString());
		fast.gameObject.SetActive (false);
		if (0 == paragraph) {
			close.gameObject.SetActive (true);
		} else {
			next.gameObject.SetActive (true);
		}
		//iTween.MoveBy(close.gameObject, iTween.Hash("y", 20.0f, "easeType", "easeInQuad", "loopType", "pingPong"));
	}
}
