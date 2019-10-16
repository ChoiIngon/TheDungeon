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
	private bool complete = false;
	private bool stop = false;
	// Use this for initialization

	public int amount;
	public void Init(int amount)
	{
		this.amount = amount;
	}

	void Awake ()
	{
		Util.EventSystem.Subscribe(EventID.TextBox_Close, Stop);
		animator = UIUtil.FindChild<Animator>(transform, "Sprite");
		animator.Play("Spin", -1, Random.Range(0.0f, 1.0f));
		groundPos = transform.localPosition.y;
		velocity = transform.forward * Random.Range(0.5f, 1.0f);
		// Throw upwards
		velocity.y = Random.Range(4.0f, 6.0f);
		coroutine = StartCoroutine(Bounce());
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.TextBox_Close, Stop);
	}

	IEnumerator Bounce()
	{
		while (velocity.sqrMagnitude > sleepThreshold && false == stop)
		{
			if (transform.localPosition.y > groundPos)
			{
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

		iTween.MoveTo(gameObject, iTween.Hash(
			"position", GameManager.Instance.ui_coin.position,
			"time", 0.25f,
			"easetype", iTween.EaseType.easeOutQuint,
			"oncomplete", "OnComplete",
			"oncompletetarget", gameObject)
		);
		while (false == complete)
		{
			yield return null;
		}

		yield return GameManager.Instance.ui_coin.ChangeAmount();
		Destroy(gameObject);
	}

	public void Stop()
	{
		stop = true;
	}

	private void OnComplete()
	{
		complete = true;
	}
}
