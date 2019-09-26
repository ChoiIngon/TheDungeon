using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GridLayoutGroup))]
public class UIButtonGroup : MonoBehaviour {
	public System.Action[] actions;
	public Button[] buttons;
	public Image[] images;
	public Text[] names;
	// Use this for initialization
	public void Init ()
	{
		actions = new System.Action[buttons.Length];
		images = new Image[buttons.Length];
		names = new Text[buttons.Length];

		for (int i = 0; i < buttons.Length; i++)
		{
			int index = i;
			//buttons [i].enabled = false;
			images[i] = buttons[i].GetComponent<Image>();
			names[i] = UIUtil.FindChild<Text>(buttons[i].transform, "Text");
			UIUtil.AddPointerUpListener(buttons[i].gameObject, () => {
				actions[index]?.Invoke();
			});
		}
	}

	public void Show(float time = 0.5f)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons [i].enabled = true;
			images[i].color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			StartCoroutine(Util.UITween.ColorTo(images[i], new Color(1.0f, 1.0f, 1.0f, 1.0f), time));
		}
	}

	public void Hide(float time = 0.5f)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			buttons [i].enabled = false;
			StartCoroutine(Util.UITween.ColorTo(images[i], new Color(1.0f, 1.0f, 1.0f, 0.0f), time));
		}
	}
}
