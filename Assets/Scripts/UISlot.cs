using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour {
	Image icon;
	Image border;
	public Image clone;
	public Rect rect {
		get {
			return border.rectTransform.rect;
		}
	}
	Image grade;
	public ImageOutline outline;
	public int index;
	public UIInventory inventory;
	public ItemData item;
	public void Start()
	{
		EventTrigger trigger = GetComponent<EventTrigger>( );
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener (( data) => {
				OnPointerDown ((PointerEventData)data);
			});
			trigger.triggers.Add (entry);
		}
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener (( data) => {
				OnPointerUp ((PointerEventData)data);
			});
			trigger.triggers.Add (entry);
		}
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.Drag;
			entry.callback.AddListener (( data) => {
				OnDrag ((PointerEventData)data);
			});
			trigger.triggers.Add (entry);
		}
		icon = transform.FindChild ("ItemIcon").GetComponent<Image> ();
		outline = transform.FindChild ("ItemIcon").GetComponent<ImageOutline> ();
		grade = transform.FindChild ("ItemGrade").GetComponent<Image> ();
		border = transform.FindChild("Border").GetComponent<Image>();
		outline.size = 0;
		Activate (false);
	}

	public void OnPointerDown( PointerEventData data )
	{
		if (null == item) {
			return;
		}
		inventory.SetSelectedSlot (this);

		clone = Instantiate<Image> (icon);
		clone.transform.SetParent (inventory.transform);

		RectTransform rtClone = clone.rectTransform;
		rtClone.anchorMax = new Vector2(0.5f, 0.5f);
		rtClone.anchorMin = new Vector2(0.5f, 0.5f);
		rtClone.localScale = Vector3.one;
		rtClone.sizeDelta = new Vector2 (100.0f, 100.0f); //rtOriginal.sizeDelta;
		clone.transform.position = transform.position;
		OnSelect ();
	}

	public void OnDrag( PointerEventData data )
	{
		if (null == item) {
			return;
		}
		clone.transform.position = data.position;
	}

	public void OnPointerUp( PointerEventData data )
	{
		if (null == item) {
			return;
		}
		clone.transform.SetParent (null);
		Destroy (clone.gameObject);
		clone = null;
	}

	public virtual void OnSelect()
	{
	}
	public virtual void OnDrop()
	{
	}

	public virtual void Activate(bool flag)
	{
		icon.gameObject.SetActive (flag);
		grade.gameObject.SetActive (flag);
	}
}
