using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugEdit : MonoBehaviour
{
	public Button add_item;
    // Start is called before the first frame update
    void Start()
    {
		UIUtil.AddPointerUpListener(add_item.gameObject, () =>
		{
			GameManager.Instance.player.inventory.Add(ItemManager.Instance.CreateRandomEquipItem());
		});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
