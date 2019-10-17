using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GridLayoutGroup))]
public class UIButtonGroup : MonoBehaviour
{
	public class UIButton
	{
		public Text name;
		public Image image;
		public Button button;
		//public System.Action action;

		public string title
		{
			get { return name.text; }
			set { name.text = value; }
		}
	}
	public List<UIButton> buttons;
	public Button button_prefab;

	public void Init ()
	{
		if (null != buttons)
		{
			foreach (UIButton button in buttons)
			{
				button.button.transform.SetParent(null);
				Object.Destroy(button.button.gameObject);
			}
		}
		buttons = new List<UIButton>();
	}

	public void AddButton(string text, Sprite sprite, System.Action action)
	{
		UIButton button = new UIButton();
		button.button = GameObject.Instantiate<Button>(button_prefab);
		button.button.transform.SetParent(transform, false);
		button.button.gameObject.SetActive(true);
		UIUtil.AddPointerUpListener(button.button.gameObject, action);
		button.image = button.button.transform.GetComponent<Image>();
		button.image.sprite = sprite;
		button.name = UIUtil.FindChild<Text>(button.button.transform, "Text");
		button.name.gameObject.SetActive(true);
		button.title = text;
		buttons.Add(button);
	}

	public void Show(float time = 0.5f)
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].button.enabled = true;
			buttons[i].name.gameObject.SetActive(true);
			StartCoroutine(Util.UITween.ColorTo(buttons[i].image, new Color(1.0f, 1.0f, 1.0f, 1.0f), time));
		}
	}

	public void Hide(float time = 0.5f)
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].button.enabled = false;
			buttons[i].name.gameObject.SetActive(false);
			StartCoroutine(Util.UITween.ColorTo(buttons[i].image, new Color(1.0f, 1.0f, 1.0f, 0.0f), time));
		}
	}
}
