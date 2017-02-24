using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITicker : MonoBehaviour {
    public UITickerMessage tickerMessagePrefab;

	float heightScale;

    private static UITicker _instance;
    public static UITicker Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = (UITicker)GameObject.FindObjectOfType(typeof(UITicker));
                if (!_instance)
                {
                    GameObject container = new GameObject();
                    container.name = "UITicker";
                    _instance = container.AddComponent<UITicker>();
                }
            }
            return _instance;
        }
    }
	void Start()
	{
		CanvasScaler canvasScaler = Object.FindObjectOfType<CanvasScaler> ();
		heightScale = Screen.height / canvasScaler.referenceResolution.y;
	}
	public void Write(string text)
	{
		UITickerMessage ticker = GameObject.Instantiate<UITickerMessage> (tickerMessagePrefab);
		ticker.transform.SetParent (transform, false);
		ticker.transform.localPosition = Vector3.zero;
		StartCoroutine(ticker.Write(text));
	}
}
