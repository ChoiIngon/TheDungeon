﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerInfo : MonoBehaviour
{
	public Text attack;
	public Text critical;
	public Text speed;
	public Text defense;
	public Text health;

	public void Init()
	{
        attack = UIUtil.FindChild<Text>(transform, "Attack/Value");
		critical = UIUtil.FindChild<Text>(transform, "Critical/Value");
		speed = UIUtil.FindChild<Text>(transform, "Speed/Value");
		defense = UIUtil.FindChild<Text>(transform, "Defense/Value");
		health = UIUtil.FindChild<Text>(transform, "Health/Value");
		Refresh();
    }

    public void Refresh()
    {
		health.text = GameManager.Instance.player.cur_health.ToString() + "/" + GameManager.Instance.player.max_health.ToString();
		attack.text = GameManager.Instance.player.attack.ToString();
        critical.text = GameManager.Instance.player.critical.ToString() + "%";
        speed.text = GameManager.Instance.player.speed.ToString();
        defense.text = GameManager.Instance.player.defense.ToString();
	}
}
