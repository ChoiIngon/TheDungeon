using UnityEngine;
using System.Collections;

public class GaugeBar : MonoBehaviour {
	public float max;
	public float current
	{
		set
		{
			float scale = value / max;
			if(0.0f > scale)
			{
				scale = 0.0f;
			}
			if(1.0f < scale)
			{
				scale = 1.0f;
			}
			if (null == _gauge) {
				return;
			}
			_gauge.localScale = new Vector3(scale, _gauge.localScale.y, _gauge.localScale.z);
		}
	}
	Transform _gauge;
	// Use this for initialization
	void Start () {
		_gauge = transform.FindChild("Gauge");
	}
}