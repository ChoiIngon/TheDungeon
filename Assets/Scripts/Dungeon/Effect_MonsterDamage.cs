using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Effect_MonsterDamage : MonoBehaviour
{
	public int damage = 0;
	public bool critical = false;
	
	private TrailRenderer trail_renderer;
	private Coroutine coroutine;
	
	private void Awake()
	{
		trail_renderer = UIUtil.FindChild<TrailRenderer>(transform, "Trail");
	}
	void Start()
    {
		trail_renderer.sortingLayerName = "Effect";
		trail_renderer.sortingOrder = 1;
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

	private IEnumerator SetActive()
	{
		Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
		const float length = 4.0f;
		direction = length * direction.normalized;
		direction.y += 0.5f;

		Vector3 start = -1.0f * direction;
		start.y += 0.5f;

		trail_renderer.transform.localPosition = start;

		transform.position = Vector3.Lerp(start, direction, Random.Range(0.4f, 0.6f));
		iTween.ShakePosition(gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.1f);
		iTween.MoveTo(trail_renderer.gameObject, direction, 0.2f);

		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
	}

}
