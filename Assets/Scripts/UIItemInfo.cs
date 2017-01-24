using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIItemInfo : MonoBehaviour {
	public Text itemName;
	public Image itemIcon;
	public Image itemGrade;
	public Text itemMainStat;
	public Text itemSubStat;
	public Item item {
		set {
			itemName.text = value.name;
			itemIcon.sprite = value.icon;
			itemGrade.color = UISlot.GetGradeColor (value.grade);
			switch(value.type)
			{
			case Item.Type.Equipment:
				EquipmentItem equipment = (EquipmentItem)value;
				itemMainStat.text = equipment.mainStat.description;
				itemSubStat.text = "";
				for (int i = 0; i < equipment.subStats.Count; i++) {
					itemSubStat.text += equipment.subStats [i].description + "\n";
				}
				break;
			default :
				break;
			}
			gameObject.SetActive (true);
		}
	}


}
