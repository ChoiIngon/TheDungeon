using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICoin : MonoBehaviour {
	/*
	public Text text;
	public Image image;
	public Vector3 position;
	private int currentCount;
	private static UICoin _instance;  
	public static UICoin Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (UICoin)GameObject.FindObjectOfType(typeof(UICoin));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "UICoin";  
					_instance = container.AddComponent<UICoin>();  
				}  
			}  

			return _instance;  
		}  
	}	
	public void Init () {
		text = transform.Find ("Amount").GetComponent<Text> ();
		image = transform.Find ("Image").GetComponent<Image> ();
    	
        position = Camera.main.ScreenToWorldPoint(
			new Vector3(image.rectTransform.position.x + 25.0f, image.rectTransform.position.y, Camera.main.transform.position.z * -1.0f)
        );

		text.text = Player.Instance.coins.ToString();

        Util.EventSystem.Subscribe<int>(EventID.CoinAmountChanged, ChangeAmount);
    }

	public void ChangeAmount(int amount)
	{
		Player.Instance.coins += amount;
		Player.Instance.Save ();
		StartCoroutine (DeferredChange (amount));
	}

	IEnumerator DeferredChange(int amount)
	{
		currentCount = Player.Instance.coins - amount;
		int increase = amount / 10;
		while (0 < increase && currentCount < Player.Instance.coins) {
			currentCount += increase;
			text.text = currentCount.ToString ();
			yield return new WaitForSeconds (0.1f);
		}
		text.text = Player.Instance.coins.ToString ();
	}
	*/
}
