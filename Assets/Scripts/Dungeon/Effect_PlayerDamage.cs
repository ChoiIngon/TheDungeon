using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Effect_PlayerDamage : MonoBehaviour {
	public float fadetime = 1.0f;
	public Sprite[] sprites;
	private Image image;
	// Use this for initialization
	void Start () {
		image = GetComponent<Image> ();
		image.sprite = sprites[Random.Range(0, sprites.Length)];
		StartCoroutine (Fadeout ());
	}
	
	IEnumerator Fadeout()
	{
		float alpha = 1.0f;
		while (0.0f < alpha) {
			alpha -= Time.deltaTime / fadetime;
			Color color = image.color;
			color.a = alpha;
			image.color = color;
			yield return null;
		}
		Destroy (gameObject);
	}
}
