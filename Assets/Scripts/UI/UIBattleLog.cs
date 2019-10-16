using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBattleLog : MonoBehaviour
{
	private int line_count;
	private Text text;

	private void Awake()
	{
		text = UIUtil.FindChild<Text>(transform, "Viewport/Content");
	}
	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Init()
	{
		text.text = "";
		line_count = 0;
	}

	public void AsyncWrite(string text)
	{
		StartCoroutine(Write(text));
	}

	public IEnumerator Write(string text)
	{
		this.text.text += text + "\n";
		if (6 <= line_count)
		{
			int index = this.text.text.IndexOf('\n');
			this.text.text = this.text.text.Substring(index + 1);
			Debug.Log("index:" + index + ", " + this.text.text);
			yield break;
		}
		line_count++;
	}
}
