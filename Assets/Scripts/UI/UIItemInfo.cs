using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
	public System.Action[] actions;

	public void Init()
	{
		actions = new System.Action[(int)Action.Max];
		for (int i = 0; i < (int)Action.Max; i++) {
			int index = i;
			EventTrigger trigger = buttons[i].gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener (( data) => { actions[index]();	});
			trigger.triggers.Add (entry);
		}
	}

	public UISlot slot {
		set {
			if (null == value) {
				return;
			}
			gameObject.SetActive (true);
			Item item = value.data.item;
			name.text = item.name;
			grade.color = UISlot.GetGradeColor(item.grade);
			description.text = item.description;
			icon.sprite = item.icon;
			stats.text = "";
			for (int i = 0; i < (int)Action.Max; i++) {
				buttons [i].gameObject.SetActive (false);
				actions [i] = null;
			}
		}
	}
}
