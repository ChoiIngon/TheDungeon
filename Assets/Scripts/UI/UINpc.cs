﻿using System.Collections;
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
	
	float widthScale = 1.0f;
	float heightScale = 1.0f;
	public void Init()
	{
		CanvasScaler canvasScaler = Object.FindObjectOfType<CanvasScaler> ();
		widthScale = Screen.width/canvasScaler.referenceResolution.x;
		heightScale = Screen.height/canvasScaler.referenceResolution.y;
	}

	public void AsyncWrite(string speaker_id, string[] text)
	{
		StartCoroutine(Write(speaker_id, text));
	}
	public IEnumerator Write(string speaker_id, string[] text)
	{
		while (UITextBox.State.Idle != GameManager.Instance.ui_textbox.state)
		{
			yield return null;
		}

		SetSprite(speaker_id);
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * widthScale, "easeType", "easeInOutExpo"));
		GameManager.Instance.ui_textbox.on_close += () =>
		{
			iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
		};
		yield return GameManager.Instance.ui_textbox.Write(text);
	}

	public void AsyncWrite(List<Quest.Dialogue> dialogues)
	{
		StartCoroutine(Write(dialogues));
	}

	public IEnumerator Write(List<Quest.Dialogue> dialogues)
	{
		Util.EventSystem.Publish(EventID.NPC_Dialogue_Start);
		while (UITextBox.State.Idle != GameManager.Instance.ui_textbox.state)
		{
			yield return null;
		}
		
		int phaseNum = 0;
		GameManager.Instance.ui_textbox.on_close += () =>
		{
			iTween.MoveTo(image.gameObject, iTween.Hash("x", 0.0f, "easeType", "easeInOutExpo", "time", 0.5f));
			Util.EventSystem.Publish(EventID.NPC_Dialogue_Finish);
		};
		GameManager.Instance.ui_textbox.on_next += () =>
		{
			SetSprite(dialogues[phaseNum++].sprite_path);
		};

		SetSprite(dialogues[phaseNum++].sprite_path);
		iTween.MoveBy(image.gameObject, iTween.Hash("x", image.rectTransform.rect.width * widthScale, "easeType", "easeInOutExpo"));

		List<string> scripts = new List<string>();
		foreach (Quest.Dialogue dialogue in dialogues)
		{
			scripts.Add(dialogue.text);
		}
		yield return GameManager.Instance.ui_textbox.Write(scripts.ToArray());
	}

	private void SetSprite(string npcID)
	{
		if ("" != npcID)
		{
			image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			image.sprite = ResourceManager.Instance.Load<Sprite>(npcID);
		}
		else
		{
			image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		}
	}
}
