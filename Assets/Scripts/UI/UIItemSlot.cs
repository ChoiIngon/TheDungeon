using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemSlot : MonoBehaviour
{
	public UIItem item = null;
	public UIInventory inventory = null;
	public int slot_index = -1;
	private Image guide_arrow = null;
	public RectTransform rectTransform;

	public virtual void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		if (null == rectTransform)
		{
			throw new System.Exception("can not find componernt 'RectTransform'");
		}

		guide_arrow = transform.Find("Arrow").GetComponent<Image>();
		if (null == guide_arrow)
		{
			throw new System.Exception("can not find child component(name:Arrow)");
		}
	}

	// Start is called before the first frame update
	public void SetItem(UIItem item)
	{
		if (null == item)
		{
			if(null != this.item)
			{
				this.item.transform.SetParent(null, false);
				Object.Destroy(this.item.gameObject);
			}
			this.item = null;
			return;
		}
		this.item = item;
		this.item.inventory = this.inventory;
		item.transform.SetParent(transform, false);
		item.transform.localPosition = Vector3.zero;
		item.rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
	}

	public bool Overlaps(UIItem item)
	{
		if (this == item)
		{
			return true;
		}

		Rect rhs = item.clone.rectTransform.rect;
		Rect lhs = rectTransform.rect;

		rhs.width *= GameManager.Instance.canvas.scaleFactor;
		rhs.height *= GameManager.Instance.canvas.scaleFactor;

		lhs.width *= GameManager.Instance.canvas.scaleFactor;
		lhs.height *= GameManager.Instance.canvas.scaleFactor;

		rhs.position = (Vector2)item.clone.transform.position;
		lhs.position = (Vector2)transform.position;

		return lhs.Overlaps(rhs);
	}

	public bool Contains(UIItem item)
	{
		if (this == item)
		{
			return true;
		}

		Rect rhs = item.clone.rectTransform.rect;
		Rect lhs = rectTransform.rect;

		rhs.width *= GameManager.Instance.canvas.scaleFactor;
		rhs.height *= GameManager.Instance.canvas.scaleFactor;

		lhs.width *= GameManager.Instance.canvas.scaleFactor;
		lhs.height *= GameManager.Instance.canvas.scaleFactor;

		rhs.position = (Vector2)item.clone.transform.position;
		lhs.position = (Vector2)transform.position;

		Vector2 point = new Vector2(rhs.x + rhs.width / 2, rhs.y + rhs.height / 2);
		return lhs.Contains(point);
	}

	public void SetActiveGuideArrow(bool active)
	{
		if (true == active)
		{
			guide_arrow.transform.SetParent(inventory.transform);
			iTween.MoveBy(guide_arrow.gameObject, iTween.Hash("y", 20, "easeType", "linear", "loopType", "pingPong", "delay", 0.0f, "time", 0.5f));
		}
		else
		{
			guide_arrow.transform.SetParent(transform);
			iTween.Stop(guide_arrow.gameObject);
			guide_arrow.transform.localPosition = Vector3.zero;
		}
		guide_arrow.gameObject.SetActive(active);
	}

}
