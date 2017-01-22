using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIItemInfo : MonoBehaviour {
	public Text itemName;
	public Image itemIcon;
	public Image itemGrade;
	public Text itemMainStat;
	public Text itemSecondaryStat;
	public ItemData data {
		set {
			itemName.text = value.info.name;
			itemIcon.sprite = value.info.icon;
			itemGrade.color = UISlot.GetGradeColor (value.info.grade);
			gameObject.SetActive (true);
		}
	}


}
