using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoin : MonoBehaviour
{
	public Text text;
	public Image image;
	public Vector3 position;
	int current_amount = 0;

	public void Init()
	{
		text = transform.Find ("Amount").GetComponent<Text> ();
		image = transform.Find ("Image").GetComponent<Image> ();
        position = Camera.main.ScreenToWorldPoint(
			new Vector3(image.rectTransform.position.x + 25.0f, image.rectTransform.position.y, Camera.main.transform.position.z * -1.0f)
        );
		text.text = "0";
    }

	public void ChangeAmount(int amount)
	{
		StartCoroutine (DeferredChange (amount));
	}

	IEnumerator DeferredChange(int amount)
	{
		current_amount = GameManager.Instance.player.inventory.coin - amount;
		int increase = amount / 10;
		while (0 < increase && current_amount < GameManager.Instance.player.inventory.coin) {
			current_amount += increase;
			text.text = current_amount.ToString ();
			yield return new WaitForSeconds (0.1f);
		}
		text.text = GameManager.Instance.player.inventory.coin.ToString ();
	}
}
