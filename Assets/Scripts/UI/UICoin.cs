using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICoin : MonoBehaviour
{
	public Vector3 position;
	private Text text;
	private int current_amount = 0;

	public void Init()
	{
		text = UIUtil.FindChild<Text>(transform, "Amount");
		text.text = "0";

		Image image = UIUtil.FindChild<Image>(transform, "Image");
        position = Camera.main.ScreenToWorldPoint(
			new Vector3(image.rectTransform.position.x + 25.0f, image.rectTransform.position.y, Camera.main.transform.position.z * -1.0f)
        );
    }

	public IEnumerator ChangeAmount(int amount)
	{
		current_amount = GameManager.Instance.player.coin - amount;
		int increase = amount / 10;
		while (0 < increase && current_amount < GameManager.Instance.player.coin) {
			current_amount += increase;
			text.text = current_amount.ToString ();
			yield return new WaitForSeconds (0.1f);
		}
		text.text = GameManager.Instance.player.coin.ToString ();
	}
}
