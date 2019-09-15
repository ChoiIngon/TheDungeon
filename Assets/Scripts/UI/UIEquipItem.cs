using UnityEngine;
using UnityEngine.UI;

public class UIEquipItem : UIItem
{
    public EquipItem.Part part = EquipItem.Part.Invalid;
    public int equip_index = 0; // left or right
    public Image guide_arrow = null;
 
    public override void Awake()
    {
        base.Awake();
        guide_arrow = transform.Find("Arrow").GetComponent<Image>();
        if (null == guide_arrow)
        {
            throw new System.Exception("can not find child component(name:Arrow)");
        }

        //Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
        //Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
    }

	protected override void OnDestroy()
	{
		//Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
		//Util.EventSystem.Unsubscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
		base.OnDestroy();
	}

	public override void OnEquipSlotDrop(UIEquipItemSlot slot)
	{
		EquipItem equipItem = (EquipItem)item_data;
		if (slot.part != equipItem.part)
		{
			return;
		}

		if (null != slot.item)
		{
			EquipItem unequipItem = GameManager.Instance.player.Unequip(slot.part, slot.equip_index);
			GameManager.Instance.player.inventory.Add(unequipItem);
		}

		if (true == equipItem.equip)
		{
			GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
		}
		else
		{
			GameManager.Instance.player.inventory.Remove(equipItem.slot_index);
		}
		GameManager.Instance.player.Equip((EquipItem)item_data, slot.equip_index);
	}

	public override void OnItemSlotDrop(UIItemSlot slot)
	{
		EquipItem equipItem = (EquipItem)item_data;
		if (true == equipItem.equip)
		{
			GameManager.Instance.player.Unequip(equipItem.part, equipItem.equip_index);
			GameManager.Instance.player.inventory.Add(equipItem);
		}
		else
		{
			int prevSlotIndex = equipItem.slot_index;
			GameManager.Instance.player.inventory.Remove(equipItem.slot_index);
			Item prev = GameManager.Instance.player.inventory.Remove(slot.slot_index);
			if (null != prev)
			{
				GameManager.Instance.player.inventory.Add(prev, prevSlotIndex);
			}
			GameManager.Instance.player.inventory.Add(equipItem, slot.slot_index);
		}
	}
	/*
public override void OnSlotSelectNotify(UIItem other)
{
	SetActiveGuideArrow(false);
	if(this == other)
	{
		outline.outline = true;
	}
	else
	{
		outline.outline = false;
	}
	if (Item.Type.Equipment != other.item_data.meta.type)
	{
		return;
	}

	EquipItem equipItem = other.item_data as EquipItem;
	if (null == equipItem)
	{
		throw new System.InvalidCastException("item type is not equip");
	}

	if (part != equipItem.part)
	{
		return;
	}

	if (this == other)
	{
		return;
	}

	SetActiveGuideArrow(true);
}

public override void OnSlotReleaseNotify(UIItem other)
{
	if (false == Overlaps(other))
	{
		return;
	}

	if (null == other.item_data)
	{
		return;
	}
	switch (other.item_data.meta.type)
	{
		case Item.Type.Equipment: {
			EquipItem equipItem = (EquipItem)other.item_data;
			if (null == equipItem)
			{
				return;
			}

			if (part != equipItem.part)
			{
				return;
			}

			if (this == other)
			{
				return;
			}

			if (null == other.clone)
			{
				return;
			}

			Debug.Log(name + " is selected");
			if (false == equipItem.equip)
			{
				Item prev = GameManager.Instance.player.Equip(equipItem, equip_index);
				GameManager.Instance.player.inventory.Add(prev);
				GameManager.Instance.player.inventory.Remove(equipItem.slot_index);

			}
			else
			{
				UIEquipItem equipSlot = other as UIEquipItem;
				EquipItem a = GameManager.Instance.player.Unequip(part, equip_index);
				EquipItem b = GameManager.Instance.player.Unequip(part, equipSlot.equip_index);
				GameManager.Instance.player.Equip(a, equipSlot.equip_index);
				GameManager.Instance.player.Equip(b, equip_index);
			}
		}
		break;
		case Item.Type.Scroll: {
				if (null == item_data)
				{
					return;
				}
		}
		break;
	}
}

	private void OnItemEquip(ItemEquipEvent evt)
    {
        SetActiveGuideArrow(false);
    }

    private void OnItemUnequip(ItemEquipEvent evt)
    {
        SetActiveGuideArrow(false);
    }

    private void SetActiveGuideArrow(bool active)
    {
        if(true == active)
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
	*/
}
