using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour {
	public Text text;

	void OnEnable()
	{
		Init ();
	}

	public void Init()
	{
		text.text = "Lv. " + Player.Instance.level + "\n";
		Unit.Stat stat = Player.Instance.GetStat ();
		//text.text += "H.P:" + Player.Instance.health.current + "/" + stat.health + "\n";
		text.text += "<b>ATK</b> : " + stat.attack + "\t"; text.text += "<b>DEF</b>:" + stat.defense + "\n";
		text.text += "<b>SPD</b> : " + stat.speed + "\t"; text.text += "<b>CRI</b>:" + stat.critcal + "%\n";
	}
}
