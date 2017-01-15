using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class UIEquipment : MonoBehaviour {
	public Button close;
	void Start()
	{
		close.onClick.AddListener (() => {
			gameObject.SetActive(false);
		});	
	}

	void OnEnable()
	{
		TheDungeon.Controller.Instance.SetState (TheDungeon.Controller.State.Popup);
	}
	void OnDisable()
	{
		TheDungeon.Controller.Instance.SetState (TheDungeon.Controller.State.Idle);
	}
}
