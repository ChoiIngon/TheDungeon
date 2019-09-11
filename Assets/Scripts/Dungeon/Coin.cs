using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
	private Vector3 velocity = Vector3.zero;
	private float groundPos;
	private float sleepThreshold = 0.0025f;
	private float bounceCooef = 0.6f;
	private float gravity = -9.8f;
	private Animator animator;
	private Coroutine coroutine;
	// Use this for initialization

	public int amount;
	public void Init(int amount)
	{
		this.amount = amount;
	}

	void Start ()
	{
		animator = UIUtil.FindChild<Animator>(transform, "Sprite");
		animator.Play("Spin", -1, Random.Range(0.0f, 1.0f));
		groundPos = transform.localPosition.y;
		velocity = transform.forward * Random.Range(0.5f, 1.0f);
		// Throw upwards
		velocity.y = Random.Range(4.0f, 6.0f);
		coroutine = StartCoroutine(Bounce());
	}

	public void Stop()
	{
		if (null != coroutine)
		{
			StopCoroutine(coroutine);
		}
		iTween.MoveTo (gameObject, GameManager.Instance.ui_coin.position, 0.1f);
        Object.Destroy(gameObject, 0.1f);
	}

	IEnumerator Bounce ()
	{
		while ( velocity.sqrMagnitude > sleepThreshold )
		{
			if (transform.localPosition.y > groundPos) {
				velocity.y += gravity * Time.deltaTime;
			}
			Vector3 delta = velocity * Time.deltaTime;
			delta.z = 0.0f;
			transform.position += delta;

			if (transform.localPosition.y <= groundPos)
			{
				transform.localPosition = new Vector3(transform.localPosition.x, groundPos, transform.localPosition.z);
				velocity.y = -velocity.y;
				velocity *= bounceCooef;
			}

			yield return null;
		}

		float t = Random.Range (0.5f, 0.8f);
		iTween.MoveTo (gameObject, GameManager.Instance.ui_coin.position, t);
		Object.Destroy (gameObject, t);
	}

	private void OnDestroy()
	{
		if (true == Application.isPlaying)
		{
			GameManager.Instance.ui_coin.ChangeAmount (amount);
		}
	}
}
