﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISlot : MonoBehaviour
{
	// Image border;
	public Image icon = null;
	public Image grade = null;
    public Image clone = null;
    public ImageOutline outline = null;
	public RectTransform rectTransform;

    public Item item;
    private Canvas canvas = null;

    public virtual void Start()
	{
        canvas = FindObjectOfType<Canvas>();
        if (null == canvas)
        {
            throw new System.Exception("can not find child component(name:Canvas)");
        }

        EventTrigger trigger = GetComponent<EventTrigger>( );
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener ((data) => {
				OnPointerDown ((PointerEventData)data);
			});
			trigger.triggers.Add (entry);
		}
		{
			EventTrigger.Entry entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener ((data) => {
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
		icon = transform.Find ("ItemIcon").GetComponent<Image> ();
        grade = transform.Find("ItemGrade").GetComponent<Image>();
        outline = transform.Find ("ItemIcon").GetComponent<ImageOutline> ();
		outline.outline = false;

        Init (item);

        Util.EventSystem.Subscribe<UISlot>(EventID.Inventory_Slot_Select, OnSlotSelectNotify);
        Util.EventSystem.Subscribe<UISlot>(EventID.Inventory_Slot_Release, OnSlotReleaseNotify);
	}

	protected virtual void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<UISlot>(EventID.Inventory_Slot_Select, OnSlotSelectNotify);
		Util.EventSystem.Unsubscribe<UISlot>(EventID.Inventory_Slot_Release, OnSlotReleaseNotify);
	}

	public virtual void Init(Item item)
	{
		this.item = item;
		icon.gameObject.SetActive (false);
		grade.gameObject.SetActive (false);
        if (null == item)
        {
            return;
        }
		icon.gameObject.SetActive (true);
        icon.sprite = ResourceManager.Instance.Load<Sprite>(item.meta.sprite_path);
        if (null == icon.sprite)
        {
            throw new System.Exception("can not find sprite(sprite_path:" + item.meta.sprite_path + ")");
        }
        grade.gameObject.SetActive (true);
		grade.color = GetGradeColor (item.grade);
	}

    public virtual void OnSlotSelectNotify(UISlot other)
    {
    }

    public virtual void OnSlotReleaseNotify(UISlot other) {}
	
	public bool Overlaps(UISlot other)
    {
        if(this == other)
        {
            return true;
        }

        Rect rhs = other.clone.rectTransform.rect;
        Rect lhs = rectTransform.rect;

        rhs.width *= canvas.scaleFactor;
        rhs.height *= canvas.scaleFactor;

        lhs.width *= canvas.scaleFactor;
        lhs.height *= canvas.scaleFactor;

        

        return lhs.Overlaps(rhs);
    }

    public bool Contains(UISlot other)
    {
        if(this == other)
        {
            return true;
        }

        Rect rhs = other.clone.rectTransform.rect;
        Rect lhs = rectTransform.rect;

        rhs.width *= canvas.scaleFactor;
        rhs.height *= canvas.scaleFactor;

        lhs.width *= canvas.scaleFactor;
        lhs.height *= canvas.scaleFactor;

        rhs.position = (Vector2)other.clone.transform.position;
        lhs.position = (Vector2)transform.position;

        Vector2 point = new Vector2(rhs.x+rhs.width/2, rhs.y + rhs.height/2);
        return lhs.Contains(point);
    }

    private void OnPointerDown(PointerEventData evt)
    {
        if (null == item)
        {
            return;
        }

        clone = Instantiate<Image>(icon);
        if (null == clone)
        {
            throw new System.Exception("can not clone icon image");
        }

        Transform inventory = transform.parent.parent;
        clone.transform.SetParent(inventory, false);

        RectTransform rtClone = clone.rectTransform;
        rtClone.anchorMax = new Vector2(0.5f, 0.5f);
        rtClone.anchorMin = new Vector2(0.5f, 0.5f);
        rtClone.localScale = Vector3.one;
        rtClone.sizeDelta = new Vector2(100.0f, 100.0f); //rtOriginal.sizeDelta;

        clone.transform.position = transform.position;
        Util.EventSystem.Publish<UISlot>(EventID.Inventory_Slot_Select, this);
        outline.outline = true;
    }

    private void OnDrag(PointerEventData evt)
    {
		if (null == clone)
		{
			return;
		}
        clone.transform.position = evt.position;
    }

    private void OnPointerUp(PointerEventData evt)
    {
        if (null == item)
        {
            return;
        }

        Util.EventSystem.Publish<UISlot>(EventID.Inventory_Slot_Release, this);
        clone.transform.SetParent(null);
        Destroy(clone.gameObject);
        clone = null;
    }
    public static Color GetGradeColor(Item.Grade grade)
	{
		Color color = Color.white;
		switch (grade) {
		case Item.Grade.Low:
			color = new Color (0x80, 0x80, 0x80); // gray
			break;
		case Item.Grade.Normal:
			color = Color.white;
			break;
		case Item.Grade.High:
			color = new Color (0x00, 0x80, 0x00); // green
			break;
		case Item.Grade.Magic:
			color = new Color (0x00, 0x00, 0xA0); // dark blue
			break;
		case Item.Grade.Rare:
			color = new Color (0x80, 0x00, 0x80); // purple
			break;
		case Item.Grade.Legendary:
			color = new Color (0xFF, 0xA5, 0x00); // orange
			break;
		}
		return color;
	}
}
