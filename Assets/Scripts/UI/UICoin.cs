using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICoin : MonoBehaviour
{
	private Text text;
	private int current_amount = 0;

	private void Awake()
	{
		text = UIUtil.FindChild<Text>(transform, "Amount");
	}

	private void OnEnable()
	{
		Util.EventSystem.Subscribe(EventID.CoinAmountChanged, OnCoinAmountChanged);
	}

	private void OnDisable()
	{
		Util.EventSystem.Unsubscribe(EventID.CoinAmountChanged, OnCoinAmountChanged);
	}
	
	public IEnumerator ChangeAmount()
	{
		int changedAmount = GameManager.Instance.player.coin - current_amount;
		int increase = changedAmount / 10;
		while (0 < increase && current_amount < GameManager.Instance.player.coin) {
			current_amount += increase;
			text.text = current_amount.ToString ();
			yield return new WaitForSeconds (0.1f);
		}

		current_amount = GameManager.Instance.player.coin;
		text.text = GameManager.Instance.player.coin.ToString ();
	}

	public void OnCoinAmountChanged()
	{
		StartCoroutine(ChangeAmount());
	}
}
