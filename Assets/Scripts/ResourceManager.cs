using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResourceManager : MonoBehaviour {
	private static ResourceManager _instance;  
	public static ResourceManager Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (ResourceManager)GameObject.FindObjectOfType(typeof(ResourceManager));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "ResourceManager";  
					_instance = container.AddComponent<ResourceManager>();  
				}  
			}  

			return _instance;  
		}  
	}
	public void Init()
	{
		resource = new Dictionary<string, Object> ();	

		/*
		#if UNITY_EDITOR
		string path = Application.dataPath;
		{
			IEnumerable<string> files = Directory.GetFiles (path + "/Resources/Sprites", "*.png", SearchOption.AllDirectories);
			foreach (string f in files)
			{
				Sprite sprite = LoadSpite (f);
				resource.Add (Path.GetFileNameWithoutExtension(f), sprite);
			}
		}
		#else
		*/
		resource.Add ("monster_daemon", Resources.Load <Sprite> ("Sprites/monster_daemon"));
		resource.Add ("monster_skeleton", Resources.Load <Sprite> ("Sprites/monster_skeleton"));
		resource.Add ("monster_succubus", Resources.Load <Sprite> ("Sprites/monster_succubus"));
		resource.Add ("item_ring_001", Resources.Load<Sprite> ("Sprites/Item/item_ring_001"));
		resource.Add ("item_ring_002", Resources.Load<Sprite> ("Sprites/Item/item_ring_002"));
		resource.Add ("item_ring_003", Resources.Load<Sprite> ("Sprites/Item/item_ring_003"));
		resource.Add ("item_shield_001", Resources.Load<Sprite> ("Sprites/Item/item_shield_001"));
		resource.Add ("item_shield_002", Resources.Load<Sprite> ("Sprites/Item/item_shield_002"));
		resource.Add ("item_shield_003", Resources.Load<Sprite> ("Sprites/Item/item_shield_003"));
		resource.Add ("item_shirt_001", Resources.Load<Sprite> ("Sprites/Item/item_shirt_001"));
		resource.Add ("item_shirt_002", Resources.Load<Sprite> ("Sprites/Item/item_shirt_002"));
		resource.Add ("item_shirt_003", Resources.Load<Sprite> ("Sprites/Item/item_shirt_003"));
		resource.Add ("item_sword_001", Resources.Load<Sprite> ("Sprites/Item/item_sword_001"));
		resource.Add ("item_sword_002", Resources.Load<Sprite> ("Sprites/Item/item_sword_002"));
		resource.Add ("item_sword_003", Resources.Load<Sprite> ("Sprites/Item/item_sword_003"));
		resource.Add ("item_potion_001", Resources.Load<Sprite> ("Sprites/Item/item_potion_001"));
		resource.Add ("item_potion_002", Resources.Load<Sprite> ("Sprites/Item/item_potion_002"));
		resource.Add ("item_potion_003", Resources.Load<Sprite> ("Sprites/Item/item_potion_003"));
		//#endif
	}

	Dictionary<string, Object> resource;

	Sprite LoadSpite(string filePath) {

		Texture2D tex = null;
		byte[] fileData;

		if (File.Exists(filePath))     {
			fileData = File.ReadAllBytes(filePath);
			tex = new Texture2D(2, 2);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
			Sprite sprite = Sprite.Create(tex, new Rect(0.0f,0.0f,tex.width,tex.height), new Vector2(0.5f,0.5f)); 
			sprite.name = filePath;
			return sprite;
		}
		return null;
	}

	public T Load<T>(string name) where T : Object {
		if (true == resource.ContainsKey (name)) {
			return resource [name] as T;
		}
		return null;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
