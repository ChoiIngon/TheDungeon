﻿using UnityEngine;
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
            _text.text = value.ToString() + "/" + max.ToString();
		}
	}
	Transform _gauge;
	TextMesh _text;
	// Use this for initialization
	void Start () {
		_gauge = transform.FindChild("Gauge");
		_text = transform.FindChild("Text").GetComponent<TextMesh>();
		MeshRenderer meshRenderer = transform.FindChild ("Text").GetComponent<MeshRenderer> ();
		meshRenderer.sortingLayerName = "UI";
		meshRenderer.sortingOrder = 3;
    }
}