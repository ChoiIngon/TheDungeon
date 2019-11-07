using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GridLayoutGroup))]
public class UIButtonGroup : MonoBehaviour
{
	public class UIButton : MonoBehaviour
	{
		public Image background;
		public Image image;
		public Text title;
		public Button button;
		//public System.Action action;

		private void Awake()
		{
			button = GetComponent<Button>();
			if (null == button)
			{
				throw new MissingComponentException("Button");
			}

			background = GetComponent<Image>();
			if (null == background)
			{
				throw new MissingComponentException("Button");
			}
			image = UIUtil.FindChild<Image>(transform, "Image");
			title = UIUtil.FindChild<Text>(transform, "Text");
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

	public UIButton AddButton(Sprite sprite, string text, System.Action action)
	{
		Button button = GameObject.Instantiate<Button>(button_prefab);
		UIButton uiButton = button.gameObject.AddComponent<UIButton>();
		button.gameObject.SetActive(true);
		button.transform.SetParent(transform, false);
		UIUtil.AddPointerUpListener(button.gameObject, action);
		uiButton.title.text = text;
		uiButton.background.sprite = sprite;
		uiButton.image.sprite = sprite;
		buttons.Add(uiButton);
		return uiButton;
	}
}
