using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class TouchInput : MonoBehaviour {
	public Action<Vector3> onTouchDown;
    public Action<Vector3> onTouchUp;
    public Action<Vector3> onTouchDrag;

	public void AddBlockCount()
	{
		touchBlockCount++;
	}

	public void ReleaseBlockCount()
	{
		touchBlockCount--;
	}

	private int touchBlockCount = 0;
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
		touchBlockCount = 0;
	}

	void OnMouseDown()
	{
        if (0 < touchBlockCount)
        {
            return;
        }
		isButtonDown = true;
		lastPressPosition = Camera.main.ScreenToWorldPoint (
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
		);
        onTouchDown?.Invoke(lastPressPosition);
    }
	void OnMouseDrag() {
		if (0 < touchBlockCount || false == isButtonDown)
		{
			return;
		}
		Vector3 currentPressPosition = Camera.main.ScreenToWorldPoint (
			new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
		);
		if(currentPressPosition != lastPressPosition)
		{
			onTouchDrag?.Invoke(currentPressPosition);
		}
		lastPressPosition = currentPressPosition;
	}
	void OnMouseUp() {
        if (0 < touchBlockCount || false == isButtonDown)
        {
            return;
        }
		isButtonDown = false;
		onTouchUp?.Invoke(lastPressPosition);
	}
}
