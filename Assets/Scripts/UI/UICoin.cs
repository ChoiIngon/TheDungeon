using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoin : MonoBehaviour {
	public int count;
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
		text.text = count.ToString (); 
	}

	public void ChangeAmount(int amount)
	{
		count += amount;
		StartCoroutine (DeferredChange (amount));
	}

	IEnumerator DeferredChange(int amount)
	{
		currentCount = count - amount;
		int increase = amount / 10;
		while (0 < increase && currentCount < count) {
			currentCount += increase;
			text.text = currentCount.ToString ();
			yield return new WaitForSeconds (0.1f);
		}
		text.text = count.ToString ();
	}
}
