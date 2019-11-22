using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Effect_PlayerDamage : MonoBehaviour
{
	public float fadetime = 0.5f;
	public Sprite[] sprites;
	private Image screen_fade;
	private Image scratch;
	private Coroutine coroutine;

	private void Awake()
	{
		screen_fade = GetComponent<Image> ();
		scratch = UIUtil.FindChild<Image>(transform, "Scratch");
	}

	private void OnEnable()
	{
		scratch.transform.Rotate(new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
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
		//blood_mark.sprite = sprites[Random.Range(0, sprites.Length)];
		scratch.color = Color.white;
		screen_fade.color = Color.white;

		StartCoroutine(Util.UITween.ColorTo(screen_fade, new Color(1.0f, 1.0f, 1.0f, 0.0f), fadetime * 0.1f));
		yield return Util.UITween.ColorTo(scratch, new Color(1.0f, 1.0f, 1.0f, 0.0f), fadetime);
		gameObject.SetActive(false);
	}
}
