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
			itemGrade.color = GetItemGradeColor (value.info.grade);
		}
	}

	private Color GetItemGradeColor(ItemInfo.Grade grade)
	{
		Color color = Color.white;
		switch (grade) {
		case ItemInfo.Grade.Low:
			color = Color.grey;
			break;
		case ItemInfo.Grade.Normal:
			color = Color.white;
			break;
		case ItemInfo.Grade.High:
			color = Color.green;
			break;
		case ItemInfo.Grade.Magic:
			color = Color.blue;
			break;
		case ItemInfo.Grade.Rare:
			color = new Color (0xFF, 0x8C, 0x00);
			break;
		case ItemInfo.Grade.Legendary:
			color = Color.red;
			break;
		}
		return color;
	}
}
