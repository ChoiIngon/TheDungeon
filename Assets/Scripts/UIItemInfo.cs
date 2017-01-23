using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIItemInfo : MonoBehaviour {
	public Text itemName;
	public Image itemIcon;
	public Image itemGrade;
	public Text itemMainStat;
	public Text itemSubStat;
	public ItemData data {
		set {
			itemName.text = value.info.name;
			itemIcon.sprite = value.info.icon;
			itemGrade.color = UISlot.GetGradeColor (value.info.grade);
			itemMainStat.text = "";
			if (null != value.info.mainStat) {
				itemMainStat.text = value.info.mainStat.description;
			}
			gameObject.SetActive (true);
		}
	}


}
