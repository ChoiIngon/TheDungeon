﻿using UnityEngine;
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
	public AudioSource audioSource;

	public ScrollRect scrollRect;
	public Text contents;
	public Button fast;
	public Button next;
	public Button close;

	private string copied;
	private float height;
	private RectTransform rectTransform;
	private int paragraph;
	private Coroutine hideCoroutine;
	
	public void Init () {
		Rect parentRect = transform.parent.GetComponent<RectTransform>().rect;

		fast.gameObject.SetActive (false);
		fast.GetComponent<RectTransform>().sizeDelta = new Vector2(parentRect.width, parentRect.height);
		fast.onClick.AddListener (FastForward);

		next.gameObject.SetActive (false);
		next.GetComponent<RectTransform>().sizeDelta = new Vector2(parentRect.width, parentRect.height);
		next.onClick.AddListener (() => {
			state = State.Next;
		});

		close.gameObject.SetActive (false);
		close.GetComponent<RectTransform>().sizeDelta = new Vector2(parentRect.width, parentRect.height);
		close.onClick.AddListener (() => {
			hideCoroutine = StartCoroutine(Hide (time));
		});

		audioSource = GameObject.Instantiate<AudioSource> (audioSource);
		rectTransform = GetComponent<RectTransform> ();
        height = rectTransform.rect.height;
		state = State.Hide;
	}

	public IEnumerator Show(float t)
	{
		Util.EventSystem.Publish(EventID.TextBox_Open);
		state = State.Raise;
		contents.text = "";
		while (height > rectTransform.anchoredPosition.y) {
			Vector2 position = rectTransform.anchoredPosition;
			position.y += height * Time.deltaTime / t;
			rectTransform.anchoredPosition = position;
			yield return null;
		}
		rectTransform.anchoredPosition = new Vector2 (rectTransform.anchoredPosition.x, height);
	}

	public IEnumerator Write(string text)
	{
		contents.text = "";
		if (null != hideCoroutine) {
			StopCoroutine (hideCoroutine);
		}
		if (State.Hide == state) {
			yield return StartCoroutine (Show (time));
		}

		state = State.Typing;
		fast.gameObject.SetActive (true);
		next.gameObject.SetActive (false);
		close.gameObject.SetActive (false);

		audioSource.Play();
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
		//yield return null;
		audioSource.Stop();
		FastForward ();
		while (State.Complete == state) {
			yield return null;
		}

		yield return hideCoroutine;
	}

	public IEnumerator Write(string[] texts)
	{
		paragraph = texts.Length - 1;
		foreach (string text in texts) {
			yield return StartCoroutine (Write (text));
			paragraph = Mathf.Max(0, paragraph-1);
		}
	}
	public IEnumerator Hide(float t = 0.0f)
	{
		close.gameObject.SetActive (false);
		//iTween.Stop (close.gameObject);
		state = State.Hide;
		if (0.0f == t) {
			t = time;
		}
		while (0 < rectTransform.anchoredPosition.y) {
			Vector2 position = rectTransform.anchoredPosition;
			position.y -= height * Time.deltaTime / t;
			rectTransform.anchoredPosition = position;
			yield return null;
		}
		rectTransform.anchoredPosition = new Vector3 (rectTransform.anchoredPosition.x, 0.0f, 0.0f);
		Util.EventSystem.Publish(EventID.TextBox_Close);
	}
	void FastForward()
	{
		scrollRect.verticalNormalizedPosition = 0.0f;
		state = State.Complete;
		fast.gameObject.SetActive (false);
		if (0 == paragraph) {
			close.gameObject.SetActive (true);
		} else {
			next.gameObject.SetActive (true);
		}
		//iTween.MoveBy(close.gameObject, iTween.Hash("y", 20.0f, "easeType", "easeInQuad", "loopType", "pingPong"));
	}
}
