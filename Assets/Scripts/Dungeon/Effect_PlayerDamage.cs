using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Effect_PlayerDamage : MonoBehaviour
{
	public float fadetime = 1.0f;
	public Sprite[] sprites;
	private Image image;
	// Use this for initialization
	IEnumerator Start ()
	{
		image = GetComponent<Image> ();
		image.sprite = sprites[Random.Range(0, sprites.Length)];
		yield return Util.UITween.ColorTo(image, new Color(1.0f, 1.0f, 1.0f, 0.0f), fadetime);
		Destroy(gameObject);
	}
}
