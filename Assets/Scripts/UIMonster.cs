using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMonster : MonoBehaviour {

	public GaugeBar health;
	public void Init(Monster.Info info)
	{
		health = transform.FindChild ("Health").GetComponent<GaugeBar> ();
		Text name = transform.FindChild ("Name").GetComponent<Text> ();
		name.text = info.name;
        RectTransform rect = name.GetComponent<RectTransform>();
        name.fontSize = (int)rect.rect.height;

        health.max = info.health;
		health.current = info.health;

	}
}
