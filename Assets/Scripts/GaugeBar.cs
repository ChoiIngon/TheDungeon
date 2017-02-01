using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GaugeBar : MonoBehaviour {
	public float max;
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
			if (0 >= max) {
				scale = 0.0f;
			} else {
				scale = _current / max;
				if (1.0f < scale) {
					scale = 1.0f;
				}
			}
            
			gauge.localScale = new Vector3(scale, gauge.localScale.y, gauge.localScale.z);
			text.text = ((int)_current).ToString() + "/" + max.ToString();
		}

        get
        {
            return _current;
        }
	}

	public IEnumerator DeferredChange(float amount, float time)
	{
		if (null != _coroutine) {
			StopCoroutine (_coroutine);
		}
		_deferredChangeAmount += amount;
		_coroutine = StartCoroutine(_DeferredChange (time)); 
		yield return _coroutine;
	}

	public IEnumerator _DeferredChange(float time)
	{
		float curr = max * gauge.localScale.x;
		float dest = _current + _deferredChangeAmount;
		float amount = dest - curr;

		float delta = 0.0f;
		while (Mathf.Abs(delta) < Mathf.Abs (amount)) {
			float scale = gauge.localScale.x + amount * (Time.deltaTime/time)/max;
			scale = Mathf.Max (0.0f, scale);
			scale = Mathf.Min (1.0f, scale);

			gauge.localScale = new Vector3(scale, gauge.localScale.y, gauge.localScale.z);
			text.text = ((int)(max*gauge.localScale.x)).ToString() + "/" + max.ToString();
			delta += amount * (Time.deltaTime / time);
			yield return null;
		}
		current += _deferredChangeAmount;
		_deferredChangeAmount = 0.0f;
	}

	private void GaugeScale(float value)
	{
		if(0.0f > value)
		{
			value = 0.0f;
		}

		float scale = 1.0f;
		if (0 >= max) {
			scale = 0.0f;
		} else {
			scale = value / max;
			if (1.0f < scale) {
				scale = 1.0f;
			}
		}

		gauge.localScale = new Vector3(scale, gauge.localScale.y, gauge.localScale.z);
		text.text = ((int)value).ToString() + "/" + max.ToString();
	}

	public Transform gauge;
	public Text text;
	public float fillPerSecond;
    float _current;
	float _deferredChangeAmount;
	Coroutine _coroutine;
	// Use this for initialization
	void Start () {
		RectTransform rt = text.gameObject.GetComponent<RectTransform> ();
		text.fontSize = (int)(rt.rect.height - text.lineSpacing);
    }
}