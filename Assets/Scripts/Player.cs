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

	public int level;
	public int exp;
	public int health;
	public float strength;
	public float defense;
	public float speed;
	public float attack {
		get {
			return 1.0f + strength;
		}
	}
	public void Init()
	{
		level = 1;
		exp = 0;
		health = 20;
		strength = 10.0f;
		defense = 4.0f;
		speed = 1.0f;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
