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
				_instance.Init ();
				DontDestroyOnLoad (_instance);
			}  

			return _instance;  
		}  
	}	
	void Init () {
		text = transform.FindChild ("Amount").GetComponent<Text> ();
		image = transform.FindChild ("Image").GetComponent<Image> ();
    	text.text = count.ToString ();
        position = Camera.main.ScreenToWorldPoint(
            new Vector3(image.rectTransform.position.x + 25.0f, image.rectTransform.position.y, DungeonMain.Instance.walkDistance)
        );
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
