using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class UIInventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public void OnPointerDown(PointerEventData evt)
    {
        Debug.Log("pointer down");
    }

    public void OnPointerUp(PointerEventData evt)
    {
        Debug.Log("pointer up");
    }
    /*
	public Inventory.Slot slot;
	public Character.EquipPart equipPart;
	public void Init(Inventory.Slot slot)
	{
		this.slot = slot;
		GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Texture/Item/"+slot.item.info.id);
		Transform count = transform.FindChild ("Count");
		count.GetComponent<Text> ().text = slot.count.ToString ();
	}
	public void OnClick()
	{
		if (null == slot) {
			return;
		}
		ItemInfoView.Instance.Init (this);
	}
	*/
}
