	using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILog : MonoBehaviour
{
	private int line_count;
	private Text text;

	private void Awake()
	{
		text = UIUtil.FindChild<Text>(transform, "Viewport/Content");
		Init();		
	}
	
	public void Init()
	{
		text.text = "";
		line_count = 0;
	}

	public void Write(string text)
	{
		this.text.text += text + "\n";
		if (4 <= line_count)
		{
			int index = this.text.text.IndexOf('\n');
			this.text.text = this.text.text.Substring(index + 1);
		}
		line_count++;
	}
}
