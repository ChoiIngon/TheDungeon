﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonBox : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private bool complete = false;
	private float time = 0.2f;
	private TouchInput touch_input;
	public string item_id;
    // Start is called before the first frame update
    void Start()
    {
		close = UIUtil.FindChild<SpriteRenderer>(transform, "Close");
		open = UIUtil.FindChild<SpriteRenderer>(transform, "Open");
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
		touch_input.onTouchDown += (Vector3 position) => 
		{
			touch_input.AddBlockCount();
			StartCoroutine(Open());
		};
		touch_input.AddBlockCount();
		gameObject.SetActive(false);
	}

	public IEnumerator Show()
	{
		close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		touch_input.ReleaseBlockCount();
		gameObject.SetActive(true);
		complete = false;
		while (false == complete)
		{
			yield return null;
		}
		gameObject.SetActive(false);
	}

	public IEnumerator Open()
	{
		yield return StartCoroutine(Util.UITween.Overlap(close, open, time));
		string text = "아이템을 획득 했습니다";
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(text));
		complete = true;
	}
}