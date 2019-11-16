using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIItemInfo : MonoBehaviour
{
	public enum Action
	{
		Equip_0, Equip_1, Unequip, Drink, Drop, Max
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
		buttons[(int)Action.Equip_0] = UIUtil.FindChild<Button>(transform, "Actions/Equip_0");
		buttons[(int)Action.Equip_1] = UIUtil.FindChild<Button>(transform, "Actions/Equip_1");
		buttons[(int)Action.Unequip] = UIUtil.FindChild<Button>(transform, "Actions/Unequip");
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

		Clear();
	}

	public void SetItemIcon(UIItem item)
	{
		grade.color = UIItem.GetGradeColor(item.data.grade);
		icon.color = Color.white;
		icon.sprite = ResourceManager.Instance.Load<Sprite>(item.data.meta.sprite_path);
	}

	public void SetItemName(string name)
	{
		this.name.text = name;
	}

	public void SetDescription(string description)
	{
		this.description.text = description;
	}

	public void SetButtonListener(UIItemInfo.Action action, System.Action listener)
	{
		buttons[(int)action].gameObject.SetActive(true);
		actions[(int)action] += listener;
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
