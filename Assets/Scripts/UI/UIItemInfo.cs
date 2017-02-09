using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIItemInfo : MonoBehaviour {
	public enum Action
	{
		Use, Throw, Drop, Max
	}
	public new Text name;
	public Image icon;
	public Image grade;
	public Text description;
	public Text stats;
	public Button[] buttons;
	public UISlot slot {
		set {
			if (null == value) {
				return;
			}
			gameObject.SetActive (true);
			Item item = value.data.item;
			name.text = item.name;
			icon.sprite = item.icon;
			grade.color = UISlot.GetGradeColor (item.grade);

			for (int i = 0; i < (int)Action.Max; i++) {
				buttons [i].gameObject.SetActive (false);
				buttons [i].onClick.RemoveAllListeners ();
			}

			switch(item.type)
			{
			case Item.Type.Equipment:
				EquipmentItem equipment = (EquipmentItem)item;;
				description.text = equipment.description;
				stats.text = equipment.mainStat.description + "\n";
				for (int i = 0; i < equipment.subStats.Count; i++) {
					stats.text += equipment.subStats [i].description + "\n";
				}
				break;
			case Item.Type.Potion:
				PotionItem potion = (PotionItem)item;
				description.text = potion.description;
				stats.text = "";
				buttons [(int)Action.Use].gameObject.SetActive (true);
				buttons [(int)Action.Use].onClick.AddListener (() => {
					potion.Use(Player.Instance);
					Player.Instance.inventory.Pull(value.data.index);
					gameObject.SetActive(false);
				});
				buttons [(int)Action.Drop].gameObject.SetActive (true);
				buttons [(int)Action.Drop].onClick.AddListener (() => {
					Player.Instance.inventory.Pull(value.data.index);
					gameObject.SetActive(false);
				});
				break;
			default :
				break;
			}


			gameObject.SetActive (true);
		}
	}
}
