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
			description.text = item.description;
			icon.sprite = item.icon;
			stats.text = "";
			for (int i = 0; i < (int)Action.Max; i++) {
				buttons [i].gameObject.SetActive (false);
				buttons [i].onClick.RemoveAllListeners ();
			}
		}
	}
}
