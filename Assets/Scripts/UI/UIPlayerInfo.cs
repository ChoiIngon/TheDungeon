using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour {
	public Text text;
   
    public void Init()
	{
        text = UIUtil.FindChild<Text>(transform, "Text");
        Refresh();
    }

    public void Refresh()
    {
        text.text = "Lv. " + GameManager.Instance.player.level + "\n";
		text.text += "hp:" + GameManager.Instance.player.cur_health + "/" + GameManager.Instance.player.max_health + "\n";
		text.text += "<b>atk</b> : " + GameManager.Instance.player.attack + "\t";
        text.text += "<b>def</b> : " + GameManager.Instance.player.defense + "\n";
        text.text += "<b>spd</b> : " + GameManager.Instance.player.speed + "\t";
        text.text += "<b>cri</b> : " + GameManager.Instance.player.critcal + "%\n";
	}
}
