using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_MonsterDamage : MonoBehaviour
{
	public int damage = 0;
	private float fadetime = 1.0f;
	// Start is called before the first frame update
	void Start()
    {
		Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f);
		const float length = 4.0f;
		direction = length * direction.normalized;
		direction.y += 0.5f;

		Vector3 start = -1.0f * direction;
		start.y += 0.5f;

		TrailRenderer trail = UIUtil.FindChild<TrailRenderer>(transform, "Trail");
		trail.sortingLayerName = "Effect";
		trail.sortingOrder = 1;
		trail.transform.localPosition = start;

		transform.position = Vector3.Lerp(start, direction, Random.Range(0.4f, 0.6f));
		MeshRenderer renderer = UIUtil.FindChild<MeshRenderer>(transform, "Text");
		renderer.sortingLayerName = "Effect";
		renderer.sortingOrder = 2;

		TextMesh text = UIUtil.FindChild<TextMesh>(transform, "Text");
		text.text = damage.ToString();
		iTween.MoveTo(trail.gameObject, direction, 0.2f);
		iTween.ShakePosition(gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.1f);

		Destroy(gameObject, 1.0f);
	}


	
}
