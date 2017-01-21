using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {
	public const int WIDTH = 5;
	public const int HEIGHT = 5;

	private static Map _instance;  
	public static Map Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (Map)GameObject.FindObjectOfType(typeof(Map));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "Map";  
					_instance = container.AddComponent<Map>();  
				}  
			}  

			return _instance;  
		}  
	}

	public Room current = null;
	public Room[] rooms = new Room[WIDTH * HEIGHT];
	public UIMap miniMap;
	// Use this for initialization
	public void Init() {
		for(int i=0; i<WIDTH*HEIGHT; i++) {
			rooms [i] = new Room ();
			Room room = rooms [i];
			room.id = i;
			room.group = i;
		}
			
		int group = 0;
		while (true) {
			List<Room> outerRooms = GetOuterRoomsInGroup (group);
			if (0 < outerRooms.Count) {
				Room room = outerRooms [Random.Range (0, outerRooms.Count)];
				int direction = Random.Range (0, Room.Max);

				for (int j = 0; j < Room.Max; j++) {
					Room other = room.Connect (direction);
					if (null != other) {
						if (other.group != room.group) {
							if (room.group < other.group) {
								ChangeGroupID (other.group, room.group);
							} else {
								ChangeGroupID (room.group, other.group);
							}
						}
						break;
					}
					direction = (direction + 1) % Room.Max;
				}
			}
			int roomCountInGroup = 0;
			foreach (Room room in rooms) {
				if (0 == room.group) {
					roomCountInGroup++;
				}
			}
			if (roomCountInGroup == Map.WIDTH * Map.HEIGHT) {
				break;
			}
			group = (group + 1) % (Map.WIDTH * Map.HEIGHT);
		}

		foreach (Room room in rooms) {
			string file = "dungeon_001_";
			if (null != room.next [Room.West]) {
				file += "W";
			} else {
				file += "_";
			}
			if (null != room.next [Room.North]) {
				file += "N";
			} else {
				file += "_";
			}
			if (null != room.next [Room.East]) {
				file += "E";
			} else {
				file += "_";
			}
			room.sprite = ResourceManager.Instance.Load<Sprite> (file);
		}
		int startIndex = Random.Range (0, WIDTH * HEIGHT);
		int exitIndex = 0;
		do {
			exitIndex = Random.Range (0, WIDTH * HEIGHT);
		} while(exitIndex == startIndex);
		current = rooms [startIndex];

		miniMap.Init ();
		miniMap.CurrentPosition (current.id);
	}

	public Room Move(int direction)
	{
		if (null == current.next [direction]) {
			return null;
		}
		current = current.next [direction];
		current.visit = true;
		miniMap.CurrentPosition (current.id);
		return current;
	}

	private List<Room> GetOuterRoomsInGroup(int group) {
		List<Room> outerRooms = new List<Room> ();
		foreach (Room room in rooms) {
			if (group == room.group) {
				for (int direction = 0; direction < Room.Max; direction++) {
                    Room other = room.GetNeighbor(direction);
                    if (null != other && group != other.group) {
						outerRooms.Add (room);
						break;
					}
				}
			}
		}
		return outerRooms;
	}

	private void ChangeGroupID(int from, int to)
	{
		for (int i = 0; i < Map.WIDTH * Map.HEIGHT; i++) {
			Room room = rooms [i];
			if (from == room.group) {
				room.group = to;
			}
		}
	}
}
