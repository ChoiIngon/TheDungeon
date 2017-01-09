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
		resource.Add ("dungeon_001____", Resources.Load <Sprite> ("Sprites/dungeon_001____"));
		resource.Add ("dungeon_001_W__", Resources.Load <Sprite> ("Sprites/dungeon_001_W__"));
		resource.Add ("dungeon_001_WN_", Resources.Load <Sprite> ("Sprites/dungeon_001_WN_"));
		resource.Add ("dungeon_001_W_E", Resources.Load <Sprite> ("Sprites/dungeon_001_W_E"));
		resource.Add ("dungeon_001_WNE", Resources.Load <Sprite> ("Sprites/dungeon_001_WNE"));
		resource.Add ("dungeon_001__N_", Resources.Load <Sprite> ("Sprites/dungeon_001__N_"));
		resource.Add ("dungeon_001__NE", Resources.Load <Sprite> ("Sprites/dungeon_001__NE"));
		resource.Add ("dungeon_001___E", Resources.Load <Sprite> ("Sprites/dungeon_001___E"));
		resource.Add ("monster_skeleton", Resources.Load <Sprite> ("Sprites/monster_skeleton"));
		#endif
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

	public Object Load(string name) {
		if (true == resource.ContainsKey (name)) {
			return resource [name];
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
