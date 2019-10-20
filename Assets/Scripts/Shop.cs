using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	public IEnumerator Init()
	{
		bool yes = false;
		GameManager.Instance.ui_textbox.on_submit += () => {
			yes = true;
		};
		yield return GameManager.Instance.ui_npc.Write("npc_graverobber", new string[] { "안녕하신가\n필요한 물건이 있는지 한번 살펴 보겠나" });
		if (true == yes)
		{
			
		}
	}
}
