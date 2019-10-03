﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStair : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private bool complete = false;
	private float time = 1.0f;
	// Start is called before the first frame update
	void Start()
	{
		close = UIUtil.FindChild<SpriteRenderer>(transform, "Close");
		open = UIUtil.FindChild<SpriteRenderer>(transform, "Open");
	}

	public IEnumerator Show()
	{
		close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		gameObject.SetActive(true);
		complete = false;
		while (false == complete)
		{
			yield return null;
		}
	}

	public IEnumerator Open()
	{
		yield return StartCoroutine(Util.UITween.Overlap(close, open, time));
		complete = true;
	}
}