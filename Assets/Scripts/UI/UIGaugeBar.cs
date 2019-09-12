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
			_current = value;
			if(0.0f > _current)
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
				if (1.0f < scale) {
					scale = 1.0f;
				}
			}

			iTween.ValueTo(gameObject, iTween.Hash(
				"from", gauge.localScale.x, 
				"to", scale, 
				"onupdate", "OnScaleChange", 
				"time", 0.5f, 
				"oncompletetarget", gameObject, 
				"oncomplete", "OnComplete",
				"easetype", iTween.EaseType.easeOutExpo
			));
		}

		get
        {
            return _current;
        }
	}

    public Transform gauge;
	public Text text;
	
    float _current;
	float _max;

	// Use this for initialization
	void Start ()
	{
		RectTransform rt = text.gameObject.GetComponent<RectTransform> ();
		text.fontSize = (int)(rt.rect.height - text.lineSpacing);
    }

	private void OnScaleChange(float scale)
	{
		gauge.localScale = new Vector3(scale, gauge.localScale.y, gauge.localScale.z);
		text.text = ((int)(max * gauge.localScale.x)).ToString() + "/" + max.ToString();
	}

	private void OnComplete()
	{
		gauge.localScale = new Vector3(_current/max, gauge.localScale.y, gauge.localScale.z);
		text.text = ((int)_current).ToString() + "/" + max.ToString();
	}
}