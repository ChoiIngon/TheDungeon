using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipItemSlot : UIItemSlot
{
	public EquipItem.Part part = EquipItem.Part.Invalid;
	public int equip_index = 0; // left or right
	private Image guide_arrow = null;

	public override void Awake()
	{
		base.Awake();
		guide_arrow = UIUtil.FindChild<Image>(transform, "Arrow");
	}

	public override void OnSlotSelectNotify(UIItem item)
	{
		SetActiveGuideArrow(false);
		
		if (Item.Type.Equipment != item.item_data.meta.type)
		{
			return;
		}

		EquipItem equipItem = item.item_data as EquipItem;
		if (null == equipItem)
		{
			throw new System.InvalidCastException("item type is not equip");
		}

		if (part != equipItem.part)
		{
			return;
		}

		if (this == item)
		{
			return;
		}

		SetActiveGuideArrow(true);
		/*
         
		inventory.itemInfo.buttons [(int)UIItemInfo.Action.Drop].gameObject.SetActive (true);
		inventory.itemInfo.actions[(int)UIItemInfo.Action.Drop] += () => {
			inventory.Pull (data.index);
			inventory.TurnEquipGuideArrowOff();
			inventory.itemInfo.gameObject.SetActive (false);
		}
        */
	}

	public override void OnSlotReleaseNotify(UIItem item)
	{
		if (false == Overlaps(item))
		{
			return;
		}

		if (null == item.item_data)
		{
			return;
		}
		item.OnEquipSlotDrop(this);
	}

	private void SetActiveGuideArrow(bool active)
	{
		if (true == active)
		{
			guide_arrow.gameObject.SetActive(active);
			iTween.MoveBy(guide_arrow.gameObject,
				iTween.Hash("y", 20, "easeType", "linear", "loopType", "pingPong", "delay", 0.0f, "time", 0.5f)
			);
		}
		else
		{
			iTween.Stop(guide_arrow.gameObject);
			guide_arrow.transform.localPosition = Vector3.zero;
			guide_arrow.gameObject.SetActive(false);
		}
	}
}
