﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap : MonoBehaviour {
	private static MiniMap _instance;  
	public static MiniMap Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (MiniMap)GameObject.FindObjectOfType(typeof(MiniMap));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "MiniMap";  
					_instance = container.AddComponent<MiniMap>();  
				}  
			}  

			return _instance;  
		}  
	}

	public const float ROOM_SIZE = 19.0f;

	public GameObject roomPrefab;
	public Image currentPosition;
	private GameObject[] rooms;

	public void Init()
	{
		int roomID = 0;
		RectTransform t = GetComponent<RectTransform> ();
		t.sizeDelta = new Vector2(ROOM_SIZE * Map.WIDTH, ROOM_SIZE * Map.HEIGHT);
		rooms = new GameObject[Map.WIDTH * Map.HEIGHT];
		for (int y = 0; y < Map.HEIGHT; y++) {
			for (int x = 0; x < Map.WIDTH; x++) {
				GameObject obj = GameObject.Instantiate<GameObject> (roomPrefab);
				obj.transform.SetParent (transform);
				obj.transform.localPosition = new Vector3 (x * ROOM_SIZE - ROOM_SIZE*Map.WIDTH, -y * ROOM_SIZE, 0.0f);
				obj.transform.FindChild ("Room/NorthDoor").gameObject.SetActive (true);
				obj.transform.FindChild ("Room/EastDoor").gameObject.SetActive (true);
				obj.transform.FindChild ("Room/SouthDoor").gameObject.SetActive (true);
				obj.transform.FindChild ("Room/WestDoor").gameObject.SetActive (true);
				Room room = Map.Instance.rooms [roomID++];
				if (null == room.doors [Room.North]) {
					obj.transform.FindChild ("Room/NorthDoor").gameObject.SetActive (false);
				}
				if (null == room.doors [Room.East]) {
					obj.transform.FindChild ("Room/EastDoor").gameObject.SetActive (false);
				}
				if (null == room.doors [Room.South]) {
					obj.transform.FindChild ("Room/SouthDoor").gameObject.SetActive (false);
				}
				if (null == room.doors [Room.West]) {
					obj.transform.FindChild ("Room/WestDoor").gameObject.SetActive (false);
				}
				obj.SetActive (false);
				rooms [y * Map.WIDTH + x] = obj;
			}
		}

		currentPosition.transform.SetSiblingIndex (transform.childCount);
		CurrentPosition (Map.Instance.current.id);
	}

	public void CurrentPosition(int id)
	{
		int y = id / Map.WIDTH;
		int x = id % Map.WIDTH;
		currentPosition.transform.localPosition = new Vector3 (x * ROOM_SIZE - ROOM_SIZE * Map.WIDTH, -y * ROOM_SIZE, 0.0f);
		rooms [id].SetActive (true);
	}
}
