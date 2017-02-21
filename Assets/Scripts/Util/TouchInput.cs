﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class TouchInput : MonoBehaviour {
	public delegate void OnTouchDownDelegate(Vector3 position);
	public delegate void OnTouchUpDelegate(Vector3 position);
	public delegate void OnTouchDragDelegate(Vector3 position);
    public OnTouchDownDelegate onTouchDown;
    public OnTouchUpDelegate onTouchUp;
    public OnTouchDragDelegate onTouchDrag;
	public Vector2 size {
		set {
			touchCollider.size = value;
		}
		get {
			return touchCollider.size;
		}
	}
	public Vector3 offset {
		
		set {
			touchCollider.offset = value;
		}
		get {
			return touchCollider.offset;
		}
	}

	private BoxCollider2D touchCollider;
	private Vector3 lastPressPosition;
	private bool isButtonDown;
    void Start()
    {
		lastPressPosition = Input.mousePosition;
		touchCollider = GetComponent<BoxCollider2D> ();
		isButtonDown = false;
	}

	void OnMouseDown() {
		if (!enabled) 
			return;
		isButtonDown = true;
		Debug.Log ("OnMouseDown:" + name);
		lastPressPosition = Camera.main.ScreenToWorldPoint (
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
		);
		if (null != onTouchDown)
		{
			onTouchDown(lastPressPosition);
		}
	}
	void OnMouseDrag() {
		if (!enabled || false == isButtonDown) 
			return;
		Debug.Log ("OnMouseDrag:" + name);
		Vector3 currentPressPosition = Camera.main.ScreenToWorldPoint (
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
		);
		if(null != onTouchDrag && currentPressPosition != lastPressPosition)
		{
			onTouchDrag(currentPressPosition);
		}
		lastPressPosition = currentPressPosition;
	}
	void OnMouseUp() {
		if (!enabled || false == isButtonDown) 
			return;
		isButtonDown = false;
		Debug.Log ("OnMouseUp:" + name);
		if (null != onTouchUp)
		{
			onTouchUp(lastPressPosition);
		}
	}
}
