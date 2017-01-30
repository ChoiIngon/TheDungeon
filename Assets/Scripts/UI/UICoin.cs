using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoin : MonoBehaviour {
	public int coinCount;
	public int currentCount;
	public Text text;
	public Image image;
	public Vector3 position;
	// Use this for initialization
	void Start () {
		text = transform.FindChild ("Amount").GetComponent<Text> ();
		image = transform.FindChild ("Image").GetComponent<Image> ();
		position = Camera.main.ScreenToWorldPoint (
			new Vector3(image.rectTransform.position.x, image.rectTransform.position.y, DungeonMain.DISTANCE)
		);
		Debug.Log (position);
		text.text = coinCount.ToString (); 
	}

	public void Add(int amount)
	{
		coinCount += amount;
		StartCoroutine (Increase (amount));
	}

	IEnumerator Increase(int amount)
	{
		currentCount = coinCount - amount;
		int increase = amount / 10;
		while (0 < increase && currentCount < coinCount) {
			currentCount += increase;
			text.text = currentCount.ToString ();
			yield return new WaitForSeconds (0.1f);
		}
		text.text = coinCount.ToString ();
	}
}
