using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDamageText : MonoBehaviour
{
	float life = 0.8f;
	TMPro.TextMeshProUGUI text;
	private void Awake()
	{
		text = GetComponent<TMPro.TextMeshProUGUI>();
	}

	public void Init(Unit.AttackResult attackResult)
	{
		int damage = (int)attackResult.damage;
		if (true == attackResult.critical)
		{
			text.text = "<size=" + text.fontSize * 2.0f + ">" + attackResult.type + " -" + damage.ToString() + "</size>";
		}
		else
		{
			text.text = attackResult.type + " -" + damage.ToString();
		}
	}

	private void OnEnable()
	{
		text.faceColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		iTween.Stop(gameObject);
		iTween.MoveBy(gameObject, iTween.Hash("y", 40.0f * GameManager.Instance.canvas.scaleFactor, "easeType", "easeOutExpo", "time", life));
		iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "easeType", "easeInCirc", "onupdate", "OnUpdate", "time", life, "oncomplete", "OnComplete", "oncompletetarget", gameObject));
	}

	private void OnUpdate(float value)
	{
		text.faceColor = new Color(1.0f, 1.0f, 1.0f, value);
	}

	private void OnComplete()
	{
		transform.SetParent(null);
		Destroy(gameObject);
	}
}
