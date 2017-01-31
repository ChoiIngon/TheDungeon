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

            if(0.0f > value)
            {
                value = 0.0f;
            }

			float scale = value / max;

            if (1.0f < scale) {
				scale = 1.0f;
			}

            _current = value;
			gauge.localScale = new Vector3(scale, gauge.localScale.y, gauge.localScale.z);
            text.text = value.ToString() + "/" + max.ToString();
		}

        get
        {
            return _current;
        }
	}
	public Transform gauge;
	public Text text;
	public float fillPerSecond;
    float _current;
	// Use this for initialization
	void Start () {
		RectTransform rt = text.gameObject.GetComponent<RectTransform> ();
		text.fontSize = (int)(rt.rect.height - text.lineSpacing);
    }

	IEnumerator Fill(float scale)
	{
		/*
		_current = value;
		gauge.localScale = new Vector3(scale, gauge.localScale.y, gauge.localScale.z);
		text.text = value.ToString() + "/" + max.ToString();
		*/
		yield return new WaitForSeconds (0.1f);
	}
}