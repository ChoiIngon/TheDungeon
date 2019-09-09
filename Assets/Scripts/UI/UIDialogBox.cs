using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIDialogBox : Util.MonoSingleton<UIDialogBox> {
	public Button submit;
	public Button cancel;

	public event System.Action onSubmit;
	public Text content;
	private RectTransform rectTransform;
	private bool active;
	void Start()
	{
	}
	// Use this for initialization
	public void Init ()
	{
		rectTransform = GetComponent<RectTransform> ();
		gameObject.SetActive (false);
		submit.onClick.AddListener (Submit);
		cancel.onClick.AddListener (Close);
		active = false;
	}

	private void Submit() {
		content.text = "";
		gameObject.SetActive(false);
		onSubmit?.Invoke();
		active = false;
		onSubmit = null;
		Util.EventSystem.Publish(EventID.Dialog_Close);
	}

	private void Close() {
		content.text = "";
		gameObject.SetActive(false);
		active = false;
		onSubmit = null;
		Util.EventSystem.Publish(EventID.Dialog_Close);
	}
		
	public int width {
		set {
			rectTransform.sizeDelta = new Vector2 (value, rectTransform.rect.height);
		}
	}

	public IEnumerator Write(string text, TextAnchor alinement = TextAnchor.MiddleLeft)
	{
		Util.EventSystem.Publish(EventID.Dialog_Open);
		active = true;
		gameObject.SetActive(true);
		content.text = text;
		content.alignment = alinement;
		rectTransform.sizeDelta = new Vector2 (rectTransform.rect.width, content.preferredHeight+100);

		while(active)
		{
			yield return null;
		}
	}
	 /*
	public void Active(string text, TextAnchor alinement = TextAnchor.MiddleLeft)
	{
		Util.EventSystem.Publish(EventID.Dialog_Open);
		active = true;
		gameObject.SetActive(true);
		content.text = text;
		content.alignment = alinement;
		rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, content.preferredHeight + 100);
	}
	*/
}