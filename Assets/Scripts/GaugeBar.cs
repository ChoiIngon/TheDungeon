using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GaugeBar : MonoBehaviour {
	public float max;
	public float current
	{
		set
		{
			
			if (0 >= max) {
				return;
			}
			if (null == _gauge) {
				return;
			}

			float scale = value / max;
			if (0.0f > scale) {
				scale = 0.0f;
			}
			if (1.0f < scale) {
				scale = 1.0f;
			}

			_gauge.localScale = new Vector3(scale, _gauge.localScale.y, _gauge.localScale.z);
            _text.text = value.ToString() + "/" + max.ToString();
		}
	}
	Transform _gauge;
	Text _text;
	// Use this for initialization
	void Start () {
		_gauge = transform.FindChild("Gauge");
		_text = transform.FindChild("Text").GetComponent<Text>();
		RectTransform rt = _text.gameObject.GetComponent<RectTransform> ();
		_text.fontSize = (int)(rt.rect.height - _text.lineSpacing);
    }
}