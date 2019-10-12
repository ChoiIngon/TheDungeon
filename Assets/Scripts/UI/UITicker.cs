using UnityEngine;
using UnityEngine.UI;

public class UITicker : MonoBehaviour
{
    public UITickerMessage tickerMessagePrefab;
		
	public void Write(string text)
	{
		UITickerMessage ticker = GameObject.Instantiate<UITickerMessage> (tickerMessagePrefab);
		ticker.transform.SetParent (transform, false);
		StartCoroutine(ticker.Write(text));
	}
}
