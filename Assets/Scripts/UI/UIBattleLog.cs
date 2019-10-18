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
		if (8 <= line_count)
		{
			int index = this.text.text.IndexOf('\n');
			this.text.text = this.text.text.Substring(index + 1);
			yield break;
		}
		line_count++;
	}
}
