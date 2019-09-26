using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIItemInfo : MonoBehaviour
{
	public enum Action
	{
		Drink, Drop, Max
	}

	public new Text name;
	public Image icon;
	public Image grade;
	public Text description;
	public Button[] buttons;
	public System.Action[] actions;

	public void Init()
	{
        name = UIUtil.FindChild<Text>(transform, "ItemName");
		description = UIUtil.FindChild<Text>(transform, "Description");
        icon = UIUtil.FindChild<Image>(transform, "Image/ItemIcon");
		grade = UIUtil.FindChild<Image>(transform, "Image/ItemGrade");

		buttons = new Button[(int)Action.Max];
		buttons[(int)Action.Drink] = UIUtil.FindChild<Button>(transform, "Actions/Drink");
		buttons[(int)Action.Drop] = UIUtil.FindChild<Button>(transform, "Actions/Drop");

		actions = new System.Action[(int)Action.Max];
		for (int i = 0; i < (int)Action.Max; i++) {
			int index = i;
			EventTrigger trigger = buttons[i].gameObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener (( data) => { actions[index]();	});
			trigger.triggers.Add (entry);
		}

		name.text = "";
		description.text = "";
	}

	public UIItem slot {
		set {
			if (null == value)
			{
				return;
			}
			gameObject.SetActive (true);
			Item item = value.item_data;
			name.text = item.meta.name;
			grade.color = UIItem.GetGradeColor(item.grade);
			description.text = item.meta.description;
			icon.sprite = ResourceManager.Instance.Load<Sprite>(item.meta.sprite_path);
			icon.color = Color.white;
			for (int i = 0; i < (int)Action.Max; i++)
			{
				buttons [i].gameObject.SetActive (false);
				actions [i] = null;
			}
		}
	}

	public void Clear()
	{
		name.text = "";
		grade.color = UIItem.GetGradeColor(Item.Grade.Low);
		description.text = "";
		icon.color = new Color(1.0f, 1.0f, 1.0f, 0.0f) ;

		for (int i = 0; i < (int)Action.Max; i++)
		{
			buttons[i].gameObject.SetActive(false);
			actions[i] = null;
		}
	}
}
