using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour {
	// Image border;
	public Image icon;
	public Image grade;
	public ImageOutline outline;

	public Image clone;
	public RectTransform rectTransform;

	public int index;
	public UIInventory inventory;

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
		rectTransform = GetComponent<RectTransform> ();
		icon = transform.FindChild ("ItemIcon").GetComponent<Image> ();
		outline = transform.FindChild ("ItemIcon").GetComponent<ImageOutline> ();
		grade = transform.FindChild ("ItemGrade").GetComponent<Image> ();
		outline.outline = false;
		Activate (false);
	}

	public void OnPointerDown( PointerEventData evt )
	{
		inventory.selected = this;

		clone = Instantiate<Image> (icon);
		clone.transform.SetParent (inventory.transform, false);

		RectTransform rtClone = clone.rectTransform;
		rtClone.anchorMax = new Vector2(0.5f, 0.5f);
		rtClone.anchorMin = new Vector2(0.5f, 0.5f);
		rtClone.localScale = Vector3.one;
		rtClone.sizeDelta = new Vector2 (100.0f, 100.0f); //rtOriginal.sizeDelta;

		clone.transform.position = transform.position;
		OnSelect ();
	}

	public void OnDrag( PointerEventData evt )
	{
		clone.transform.position = evt.position;
	}

	public void OnPointerUp( PointerEventData evt )
	{
		OnDrop ();
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

	public static Color GetGradeColor(EquipmentItem.Grade grade)
	{
		Color color = Color.white;
		switch (grade) {
		case EquipmentItem.Grade.Low:
			color = Color.grey;
			break;
		case EquipmentItem.Grade.Normal:
			color = Color.white;
			break;
		case EquipmentItem.Grade.High:
			color = Color.green;
			break;
		case EquipmentItem.Grade.Magic:
			color = Color.blue;
			break;
		case EquipmentItem.Grade.Rare:
			color = new Color (0xFF, 0x8C, 0x00);
			break;
		case EquipmentItem.Grade.Legendary:
			color = Color.red;
			break;
		}
		return color;
	}
	void onDrawGizmos()
	{
		//Gizmos.color = Color.blue;
		//Gizmos.DrawWireCube(transform.position,new Vector3(rectTransform.rect.x, rectTransform.rect.y, 0.0f));
	}

}
