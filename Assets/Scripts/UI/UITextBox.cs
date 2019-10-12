using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITextBox : MonoBehaviour
{
	public enum State
	{
		Idle,       // 텍스트 박스가 숨어 있는 상태
		Raise,      // 디스플레이를 위해 텍스트 박스가 떠 오르고 있는 상태
		Typing,     // 글자를 한자씩 치고 있는 상태
		Complete,   // 타이핑 완료 상태, raise에서 한번에 글자를 모두 출력 하고 complete 상태로 가는 것 가틍
		Hide
	}

	public State state;
	public float charPerSecond;
	public float time;

	public ScrollRect scroll_rect;
	public Text contents;
	public Button fast;
	public Button next;
	public Button close;
	public Button submit;
	public Button cancel;

	private float height;
	private RectTransform rectTransform;
	private Vector3 position;
	private int paragraph;
	private Coroutine hide_coroutine;

	public System.Action on_submit;
	public System.Action on_close;

	public void Init()
	{
		rectTransform = GetComponent<RectTransform>();
		height = rectTransform.rect.height + rectTransform.anchoredPosition.y;
		position = rectTransform.anchoredPosition;

		fast.onClick.AddListener(FastForward);
		next.onClick.AddListener(() => { state = State.Hide; });
		close.onClick.AddListener(Close);
		cancel.onClick.AddListener(Close);
		submit.onClick.AddListener(() =>
		{
			submit.gameObject.SetActive(false);
			cancel.gameObject.SetActive(false);
			FastForward();
			on_submit?.Invoke();
			on_submit = null;
		});

		state = State.Idle;
		fast.gameObject.SetActive(false);
		next.gameObject.SetActive(false);
		close.gameObject.SetActive(false);
		submit.gameObject.SetActive(false);
		cancel.gameObject.SetActive(false);
	}

	public IEnumerator TypeWrite(string[] texts)
	{
		paragraph = texts.Length - 1;
		foreach (string text in texts)
		{
			yield return TypeWrite(text);
			paragraph = Mathf.Max(0, paragraph - 1);
		}
	}
	public IEnumerator TypeWrite(string text)
	{
		contents.text = "";
		if (State.Idle == state || State.Hide == state)
		{
			yield return StartCoroutine(Show());
		}

		state = State.Typing;
		fast.gameObject.SetActive(true);
		next.gameObject.SetActive(false);
		close.gameObject.SetActive(false);
		submit.gameObject.SetActive(false);
		cancel.gameObject.SetActive(false);

		AudioManager.Instance.Play("textbox_type", true);
		foreach (char c in text)
		{
			contents.text += c;
			scroll_rect.verticalNormalizedPosition = 0.0f;
			if (State.Typing == state)	// can be changed to State.Complete for fast forward
			{
				yield return new WaitForSeconds(1.0f / charPerSecond);
			}
		}
		AudioManager.Instance.Stop("textbox_type");
		FastForward();

		if (null != on_submit)
		{
			fast.gameObject.SetActive(false);
			next.gameObject.SetActive(false);
			close.gameObject.SetActive(false);
			submit.gameObject.SetActive(true);
			cancel.gameObject.SetActive(true);
		}

		while (State.Complete == state)
		{
			yield return null;
		}
		on_submit = null;
		yield return hide_coroutine;
	}
	public IEnumerator LogWrite(string text)
	{
		if (State.Idle == state || State.Hide == state)
		{
			yield return StartCoroutine(Show());
		}

		state = State.Typing;
		fast.gameObject.SetActive(false);
		next.gameObject.SetActive(false);
		close.gameObject.SetActive(false);
		submit.gameObject.SetActive(false);
		cancel.gameObject.SetActive(false);
		contents.text += text + "\n";
		scroll_rect.verticalNormalizedPosition = 0.0f;
	}

	public Rect Resize(float h)
	{
		Rect prev = rectTransform.rect;
		iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", h, "time", 0.5f, "onupdate", "OnUpdate"));
		return prev;
	}

	private void OnUpdate(float value)
	{
		
		Vector2 delta = new Vector2(0.0f, value);
		rectTransform.sizeDelta = delta;
		Debug.Log(value);
	}

	private IEnumerator Show()
	{
		if (State.Hide == state && null != hide_coroutine)
		{
			Debug.Log("try to change state(Hide->Raise)");
			StopCoroutine(hide_coroutine);
			hide_coroutine = null;
		}

		state = State.Raise;
		Util.EventSystem.Publish(EventID.TextBox_Open);

		while (height > rectTransform.anchoredPosition.y)
		{
			Vector2 position = rectTransform.anchoredPosition;
			position.y += rectTransform.rect.height * (Time.deltaTime / time);
			rectTransform.anchoredPosition = position;
			yield return null;
		}
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, height);
	}
	private IEnumerator Hide()
	{
		close.gameObject.SetActive(false);
		Util.EventSystem.Publish(EventID.TextBox_Close);
		state = State.Hide;
		while (position.y < rectTransform.anchoredPosition.y)
		{
			Vector2 position = rectTransform.anchoredPosition;
			position.y -= rectTransform.rect.height * (Time.deltaTime / time);
			rectTransform.anchoredPosition = position;
			yield return null;
		}
		rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, position.y, 0.0f);
		state = State.Idle;
		contents.text = "";
	}

	public void ActiveCloseButton()
	{
		FastForward();
	}

	public void Close()
	{
		on_close?.Invoke();
		hide_coroutine = StartCoroutine(Hide());
		on_close = null;
	}

	public void FastForward()
	{
		scroll_rect.verticalNormalizedPosition = 0.0f;
		state = State.Complete;
		fast.gameObject.SetActive(false);
		if (0 == paragraph)
		{
			close.gameObject.SetActive(true);
		}
		else
		{
			next.gameObject.SetActive(true);
		}
		//iTween.MoveBy(close.gameObject, iTween.Hash("y", 20.0f, "easeType", "easeInQuad", "loopType", "pingPong"));
	}
}
