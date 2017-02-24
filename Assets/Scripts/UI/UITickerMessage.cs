using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITickerMessage : MonoBehaviour {
    public Text contents;
    public Image image;
    public float time;

	public IEnumerator Write(string text)
    {
		iTween.ScaleFrom (gameObject, Vector3.zero, 0.1f);
		yield return new WaitForSeconds (0.1f);
		CanvasScaler canvasScaler = Object.FindObjectOfType<CanvasScaler> ();
		float heightScale = Screen.height/canvasScaler.referenceResolution.y;
		iTween.MoveBy(gameObject, iTween.Hash("y", 200.0f * heightScale, "time", time)); 

        contents.text = text;
        yield return new WaitForSeconds(time * 0.5f);
		float alpha = 1.0f;
        while (0.0f < alpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            contents.color = new Color(contents.color.r, contents.color.g, contents.color.b, alpha);
            alpha -= Time.deltaTime / (time * 0.5f);
            yield return null;
        }

        gameObject.SetActive(false);
		Destroy (gameObject);
    }
}
