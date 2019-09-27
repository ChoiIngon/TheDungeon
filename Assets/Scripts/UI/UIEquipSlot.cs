using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipSlot : UIItemSlot
{
	public EquipItem.Part part = EquipItem.Part.Invalid;
	public int equip_index = 0; // left or right
}
