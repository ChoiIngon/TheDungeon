using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITickerMessage : MonoBehaviour
{
    public Text contents;
    public Image image;
    public float time;

	public IEnumerator Write(string text)
    {
        contents.text = text;
		iTween.ScaleFrom (gameObject, Vector3.zero, 0.1f);
		yield return new WaitForSeconds (0.1f);
		CanvasScaler canvasScaler = GameManager.Instance.canvas.transform.GetComponent<CanvasScaler>();
		iTween.MoveBy(gameObject, iTween.Hash("y", 200.0f * GameManager.Instance.canvas.scaleFactor, "time", time)); 
		yield return Util.UITween.ColorTo(image, new Color(1.0f, 1.0f, 1.0f, 0.0f), time);
        gameObject.SetActive(false);
		Destroy (gameObject);
    }
}
