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
	public void Init () {
		actions = new System.Action[buttons.Length];
		images = new Image[buttons.Length];
		names = new Text[buttons.Length];

		for (int i = 0; i < buttons.Length; i++) {
			int index = i;
			buttons [i].enabled = false;
			images [i] = buttons [i].GetComponent<Image> ();
			names [i] = buttons [i].transform.FindChild ("Text").GetComponent<Text> ();

			EventTrigger trigger = buttons [i].gameObject.AddComponent<EventTrigger> ();
			var entry = new EventTrigger.Entry ();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener (( data) => {
				if(null != actions [index])
				{
					actions[index]();
				}
			});
			trigger.triggers.Add (entry);
		}
	}

	public void Enable(bool flag)
	{
		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].enabled = flag;
		}
	}
	public void Show(float time)
	{
		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].enabled = true;
			iTween.ColorTo (images[i].gameObject, new Color(1.0f, 1.0f, 1.0f, 1.0f), time);
		}
	}

	public void Hide(float time)
	{
		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].enabled = false;
			iTween.ColorTo (images[i].gameObject, new Color(1.0f, 1.0f, 1.0f, 0.0f), time);
		}
	}
}
