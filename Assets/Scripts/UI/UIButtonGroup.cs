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
		public System.Action action;

		public string title
		{
			get { return name.text; }
			set { name.text = value; }
		}
	}
	public List<UIButton> buttons;
	// Use this for initialization
	public void Init ()
	{
		buttons = new List<UIButton>();
		for (int i = 0; i < transform.childCount; i++)
		{
			int index = i;
			UIButton button = new UIButton();
			button.image = transform.GetChild(i).GetComponent<Image>();
			button.button = transform.GetChild(i).GetComponent<Button>();
			button.name = UIUtil.FindChild<Text>(transform.GetChild(i).transform, "Text");
			UIUtil.AddPointerUpListener(button.button.gameObject, () => {
				button.action?.Invoke();
			});
			buttons.Add(button);
		}
	}

	public void Show(float time = 0.5f)
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].button.enabled = true;
			buttons[i].name.gameObject.SetActive(false);
			StartCoroutine(Util.UITween.ColorTo(buttons[i].image, new Color(1.0f, 1.0f, 1.0f, 1.0f), time));
		}
	}

	public void Hide(float time = 0.5f)
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].button.enabled = false;
			buttons[i].name.gameObject.SetActive(true);
			StartCoroutine(Util.UITween.ColorTo(buttons[i].image, new Color(1.0f, 1.0f, 1.0f, 0.0f), time));
		}
	}
}
