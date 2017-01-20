using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class TouchInput : MonoBehaviour {
	    //public string buttonName;
	    //public GameObject holder;
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
	private bool pressed;

    void Start()
    {
		lastPressPosition = Input.mousePosition;
		touchCollider = GetComponent<BoxCollider2D> ();
		pressed = false;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) 
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction);
			if (null == hit.collider) {
				return;
			}

			if (touchCollider != hit.collider) {
				return;
			}

			lastPressPosition = Camera.main.ScreenToWorldPoint (
				new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
			);
			if (null != onTouchDown)
			{
				onTouchDown(lastPressPosition);
			}
			pressed = true;
		}
		if(Input.GetMouseButtonUp(0))
		{
			if (null != onTouchUp)
			{
				onTouchUp(lastPressPosition);
			}
			pressed = false;
		}
		if (true == pressed) 
		{
			Vector3 currentPressPosition = Camera.main.ScreenToWorldPoint (
				new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane)
			);
			if(null != onTouchDrag && currentPressPosition != lastPressPosition)
			{
				onTouchDrag(currentPressPosition);
			}
			lastPressPosition = currentPressPosition;
		}
	}
}
