using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	private static Player _instance;  
	public static Player Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (Player)GameObject.FindObjectOfType(typeof(Player));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "Player";  
					_instance = container.AddComponent<Player>();  
				}  
			}  
			return _instance;  
		}  
	}
	[System.Serializable]
	public class Data {
		public int level;
		public int exp;
		public int health;
		public float strength;
		public float defense;
		public float speed;
		public Data()
		{
			level = 1;
			exp = 0;
			health = 100;
			strength = 1.0f;
			defense = 1.0f;
			speed = 1.0f;
		}
	}
	public Data data = new Data();
	public float attack {
		get {
			return 1.0f + data.strength;
		}
	}
	public GameObject ui;
	public GameObject damage;
	GaugeBar health = null;
	// Use this for initialization
	void Start () {
		health = ui.transform.FindChild ("Health").GetComponent<GaugeBar> ();
		health.max = 0;
		health.current = 0;
		RectTransform rt = ui.transform.GetComponent<RectTransform> ();
		rt.position = Camera.main.WorldToScreenPoint (transform.position);
	}
}
