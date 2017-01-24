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
                throw new System.Exception("max underflow exception"); ;
			}
			if (null == _gauge) {
                throw new System.Exception("guage is null");
			}

            if(0.0f > value)
            {
                value = 0.0f;
            }

			float scale = value / max;

            if (1.0f < scale) {
				scale = 1.0f;
			}

            _current = current;
			_gauge.localScale = new Vector3(scale, _gauge.localScale.y, _gauge.localScale.z);
            _text.text = value.ToString() + "/" + max.ToString();
		}

        get
        {
            return _current;
        }
	}
	Transform _gauge;
	Text _text;
    float _current;
	// Use this for initialization
	void Start () {
		_gauge = transform.FindChild("Gauge");
		_text = transform.FindChild("Text").GetComponent<Text>();
		RectTransform rt = _text.gameObject.GetComponent<RectTransform> ();
		_text.fontSize = (int)(rt.rect.height - _text.lineSpacing);
    }
}