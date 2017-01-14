using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	private Vector3 velocity = Vector3.zero;
	public float groundPos;
	private float sleepThreshold = 0.0025f;
	private float bounceCooef = 0.6f;
	private float gravity = -9.8f;

	void OnEnable ()
	{
		groundPos = transform.localPosition.y;
		velocity = transform.forward * Random.Range(0.5f, 1.5f);
		// Throw upwards
		velocity.y = Random.Range(4.0f, 6.0f);
		StartCoroutine(Bounce());
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
		iTween.MoveTo (gameObject, new Vector3 (-2.5f, 5.0f, 0.0f), t);
		DestroyObject (gameObject, t);

	}
}
