using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Effect_MonsterDamage : MonoBehaviour
{
	public int damage = 0;
	public bool critical = false;
	private float fadeout_time = 1.0f;
	private TrailRenderer trail_renderer;
	private MeshRenderer mesh_renderer;
	private TextMesh text_mesh;
	private Coroutine coroutine;
	// Start is called before the first frame update
	private void Awake()
	{
		trail_renderer = UIUtil.FindChild<TrailRenderer>(transform, "Trail");
		mesh_renderer = UIUtil.FindChild<MeshRenderer>(transform, "Text");
		text_mesh = UIUtil.FindChild<TextMesh>(transform, "Text");
	}
	void Start()
    {
		trail_renderer.sortingLayerName = "Effect";
		trail_renderer.sortingOrder = 1;
		mesh_renderer.sortingLayerName = "Effect";
		mesh_renderer.sortingOrder = 2;
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
		Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
		const float length = 4.0f;
		direction = length * direction.normalized;
		direction.y += 0.5f;

		Vector3 start = -1.0f * direction;
		start.y += 0.5f;

		trail_renderer.transform.localPosition = start;

		transform.position = Vector3.Lerp(start, direction, Random.Range(0.4f, 0.6f));

		if (true == critical)
		{
			text_mesh.text = "<size=" + text_mesh.fontSize * 3.0f + ">" + damage.ToString() + "</size>";
		}
		else
		{
			text_mesh.text = damage.ToString();
		}

		iTween.ShakePosition(gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.1f);
		iTween.MoveTo(text_mesh.gameObject, direction * Random.Range(0.1f, 0.6f), Random.Range(0.1f, 0.3f));
		iTween.MoveTo(trail_renderer.gameObject, direction, 0.2f);

		yield return new WaitForSeconds(0.5f);
		gameObject.SetActive(false);
	}

}
