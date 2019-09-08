using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UISlotEquip : UISlot
{
    public EquipItem.Part part = EquipItem.Part.Invalid;
    public int equip_index = 0; // left or right
    public Image guide_arrow = null;
 
    public override void Start()
    {
        base.Start();
        guide_arrow = transform.Find("Arrow").GetComponent<Image>();
        if (null == guide_arrow)
        {
            throw new System.Exception("can not find child component(name:Arrow)");
        }

        Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Equip, OnItemEquip);
        Util.EventSystem.Subscribe<ItemEquipEvent>(EventID.Item_Unequip, OnItemUnequip);
    }

    public override void OnSlotSelectNotify(UISlot other)
    {
        if(this == other)
        {
            outline.outline = true;
        }
        else
        {
            outline.outline = false;
        }
        if (Item.Type.Equipment != other.item.meta.type)
        {
            return;
        }

        EquipItem equipItem = other.item as EquipItem;
        if (null == equipItem)
        {
            throw new System.InvalidCastException("item type is not equip");
        }

        if (part != equipItem.part)
        {
            SetActiveGuideArrow(false);
            return;
        }

        if (this == other)
        {
            return;
        }

        SetActiveGuideArrow(true);
        /*
        inventory.TurnEquipGuideArrowOn(part, index);
        inventory.itemInfo.slot = this;
        inventory.itemInfo.stats.text = item.mainStat.value + " " + item.mainStat.description + "\n";
        for (int i = 0; i < item.subStats.Count; i++)
        {
            inventory.itemInfo.stats.text += "<color=green> +" + item.subStats[i].value + " " + item.subStats[i].description + "</color>\n";
        }
        inventory.itemInfo.buttons[(int)UIItemInfo.Action.Drop].gameObject.SetActive(true);
        inventory.itemInfo.actions[(int)UIItemInfo.Action.Drop] += () => {
            Player.Instance.UnequipItem(part, index);
            Init(null);
            inventory.TurnEquipGuideArrowOff();
            inventory.itemInfo.gameObject.SetActive(false);
        };
        */
    }

    public override void OnSlotReleaseNotify(UISlot other)
    {
        EquipItem equipItem = (EquipItem)other.item;
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

        if (false == Contains(other))
        {
            return;
        }

        Debug.Log(name + " is selected");
        if(false == equipItem.equip)
        {
            GameManager.Instance.player.inventory.Remove(equipItem.slot_index);
            Item prev = GameManager.Instance.player.Equip(equipItem, equip_index);
            GameManager.Instance.player.inventory.Add(prev);
        }
        else
        {
            UISlotEquip equipSlot = other as UISlotEquip;
            EquipItem a = GameManager.Instance.player.Unequip(part, equip_index);
            EquipItem b = GameManager.Instance.player.Unequip(part, equipSlot.equip_index);
            GameManager.Instance.player.Equip(a, equipSlot.equip_index);
            GameManager.Instance.player.Equip(b, equip_index);
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
}
