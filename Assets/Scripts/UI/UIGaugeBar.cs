using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIGaugeBar : MonoBehaviour
{
	public float max {
		set {
			_max = value;
			text.text = ((int)_current).ToString() + "/" + max.ToString();
		}
		get {
			return _max;
		}
	}

	public float current
	{
		set
		{
			StartCoroutine(SetCurrent(value, 0.5f));
		}

		get
        {
            return _current;
        }
	}

    public Image gauge;
	public Text text;
	public Rect rect;
	
    float _current;
	float _max;
	bool _complete;

	private void Awake()
	{
		gauge = UIUtil.FindChild<Image>(transform, "Gauge");
		RectTransform rt = text.gameObject.GetComponent<RectTransform>();
		text.fontSize = (int)(rt.rect.height - text.lineSpacing);
		rect = rt.rect;
	}

	public IEnumerator SetCurrent(float current, float time)
	{
		_complete = false;
		_current = current;
		if (0.0f > _current)
		{
			_current = 0.0f;
		}

		float scale = 1.0f;
		if (0 >= max)
		{
			scale = 0.0f;
		}
		else
		{
			scale = _current / max;
			if (1.0f < scale)
			{
				scale = 1.0f;
			}
		}

		if (0.0f < time)
		{
			iTween.ValueTo(gameObject, iTween.Hash(
				"from", gauge.fillAmount,
				"to", scale,
				"onupdate", "OnScaleChange",
				"time", time,
				"oncompletetarget", gameObject,
				"oncomplete", "OnComplete",
				"easetype", iTween.EaseType.easeOutExpo
			));
			while (false == _complete)
			{
				yield return null;
			}
		}
	}

	private void OnScaleChange(float scale)
	{
		gauge.fillAmount = scale;
		text.text = ((int)(max * gauge.fillAmount)).ToString() + "/" + max.ToString();
	}

	private void OnComplete()
	{
		gauge.fillAmount = _current/max;
		text.text = ((int)_current).ToString() + "/" + max.ToString();
		_complete = true;
	}
}