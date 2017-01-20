using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIEquipmentSlot : UISlot {
	public ItemInfo.Category category;
	public Image arrow;
	void Start( )
	{
		base.Start ();
		arrow = transform.FindChild ("Arrow").GetComponent<Image> ();
	}

	public override void OnDrop()
	{
	}

}
