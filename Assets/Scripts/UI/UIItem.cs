﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIItem : MonoBehaviour
{
	public Image icon = null;
	public Image grade = null;
    public UIItem clone = null;

	public RectTransform rectTransform;
	public UIInventory inventory;

    public Item data;

    public virtual void Awake()
	{
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
        Init (data);
	}

	public virtual void Init(Item item)
	{
		this.data = item;
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

	public virtual void OnSelect() 
	{
		
	}
	public virtual void OnDrop() {}
	public virtual void OnEquipSlotDrop(UIEquipSlot slot) {}
    public virtual void OnItemSlotDrop(UIItemSlot slot) {}
	
	private void OnPointerDown(PointerEventData evt)
    {
		if (null == data)
        {
            return;
        }

        clone = Instantiate<UIItem>(this);
        if (null == clone)
        {
            throw new System.Exception("can not clone icon image");
        }

		clone.grade.gameObject.SetActive(true);
		clone.icon.gameObject.SetActive(true);
        clone.transform.SetParent(inventory.transform, false);

        RectTransform rtClone = clone.rectTransform;
        rtClone.anchorMax = new Vector2(0.5f, 0.5f);
        rtClone.anchorMin = new Vector2(0.5f, 0.5f);
        rtClone.localScale = Vector3.one;
        rtClone.sizeDelta = new Vector2(100.0f, 100.0f); //rtOriginal.sizeDelta;

		clone.transform.position = transform.position;

		icon.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
		grade.color = new Color(grade.color.r, grade.color.g, grade.color.b, 0.5f);

		OnSelect();
    }

    private void OnDrag(PointerEventData evt)
    {
		if (null == clone)
		{
			return;
		}

		Vector3 clonePosition = evt.position;
		clonePosition.y += clone.rectTransform.rect.height * GameManager.Instance.canvas.scaleFactor * 0.5f;
		clone.transform.position = clonePosition;
    }

    private void OnPointerUp(PointerEventData evt)
    {
		if (null == data)
        {
            return;
        }

		icon.color = Color.white;
		grade.color = GetGradeColor(data.grade);

		do
		{
			bool overlap = false;
			foreach (UIItemSlot slot in inventory.inventory_slots)
			{
				if (true == slot.Contains(this))
				{
					OnItemSlotDrop(slot);
					overlap = true;
				}
			}

			if (true == overlap)
			{
				break;
			}
			foreach (var itr in inventory.equip_slots)
			{
				UIEquipSlot slot = itr.Value;
				if (true == slot.Overlaps(this))
				{
					OnEquipSlotDrop(slot);
					overlap = true;
				}
			}

			if (true == overlap)
			{
				break;
			}
			OnDrop();
		} while (false);
		
		clone.transform.SetParent(null);
		Object.Destroy(clone.gameObject);
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
