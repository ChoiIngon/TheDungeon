using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
	private Button close;

	private void Awake()
	{
		close = UIUtil.FindChild<Button>(transform, "BottomBar/Close");
		UIUtil.AddPointerUpListener(close.gameObject, () => { gameObject.SetActive(false); });
	}

	public void Init()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			Item.Meta meta = ItemManager.Instance.GetRandomExpendableItemMeta();
			Sprite sprite = ResourceManager.Instance.Load<Sprite>(meta.sprite_path);
		}
	}
}
