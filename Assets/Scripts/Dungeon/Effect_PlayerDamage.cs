using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Effect_PlayerDamage : MonoBehaviour
{
	public float fadetime = 0.5f;
	public Sprite[] sprites;
	private Image image;
	private Coroutine coroutine;

	private void Awake()
	{
		image = GetComponent<Image> ();
	}

	private void OnEnable()
	{
		if (null != coroutine)
		{
			StopCoroutine(coroutine);
			coroutine = null;
		}

		coroutine = StartCoroutine(SetActive());
	}

	private void OnDisable()
	{
	}

	private IEnumerator SetActive()
	{
		image.sprite = sprites[Random.Range(0, sprites.Length)];
		image.color = Color.white;
		yield return Util.UITween.ColorTo(image, new Color(1.0f, 1.0f, 1.0f, 0.0f), fadetime);
		gameObject.SetActive(false);
	}
}
