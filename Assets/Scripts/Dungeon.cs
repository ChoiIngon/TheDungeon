using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dungeon : MonoBehaviour {
	public const int WIDTH = 5;
	public const int HEIGHT = 5;
	public const int North = 0;
	public const int East = 1;
	public const int South = 2;
	public const int West = 3;
	public const int Max = 4;

	private static Dungeon _instance;  
	public static Dungeon Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (Dungeon)GameObject.FindObjectOfType(typeof(Dungeon));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "Dungeon";  
					_instance = container.AddComponent<Dungeon>();  
				}  
			}  

			return _instance;  
		}  
	}

	public class Room
	{
		public enum Type {
			Start,
			Exit,
			Lock,
			Normal
		}
		public int id;
		public int group;
		public bool visit;
		public Type type;
		public Monster.Info monster;
		public Item item;
		public Room[] next;
		public Room() {
			id = 0;
			group = 0;
			visit = false;
			type = Type.Normal;
			monster = null;
			item = null;
			next = new Room[Max];
			for (int direction = 0; direction < Max; direction++) {
				next [direction] = null;
			}
		}

		public Room GetNext(int direction)
		{
			if (0 > direction || Max <= direction) {
				throw new System.Exception ("room id:" + direction + ", invalid direction:" + direction);
			}
			return next [direction];
		}
	}
	public Room current = null;
	public int exit;
	public int move;
	public Room[] rooms = new Room[WIDTH * HEIGHT];
	public UIMiniMap miniMap;
	// Use this for initialization
	public void Init() {
		exit = 0;
		move = 0;

		for(int i=0; i<WIDTH*HEIGHT; i++) {
			Room room = new Room ();
			room.id = i;
			room.group = i;
			rooms [i] = room;
		}
			
		int group = 0;
		while (true) {
			List<Room> outerRooms = GetOuterRoomsInGroup (group);
			if (0 < outerRooms.Count) {
				Room room = outerRooms [Random.Range (0, outerRooms.Count)];
				int direction = Random.Range (0, Max);

				for (int j = 0; j < Max; j++) {
					Room other = GetNeighbor (room.id, direction);
					if (null == other) {
						continue;
					}
					room.next [direction] = other;
					switch (direction) {
					case North:
						other.next [South] = room;
						break;
					case East:
						other.next [West] = room;
						break;
					case South:
						other.next [North] = room;
						break;
					case West:
						other.next [East] = room;
						break;
					}

					if (other.group != room.group) {
						if (room.group < other.group) {
							ChangeGroupID (other.group, room.group);
						} else {
							ChangeGroupID (room.group, other.group);
						}
					}
					break;

					direction = (direction + 1) % Max;
				}
			}
			int roomCountInGroupZero = 0;
			foreach (Room room in rooms) {
				if (0 == room.group) {
					roomCountInGroupZero++;
				}
			}
			if (roomCountInGroupZero == WIDTH * HEIGHT) {
				break;
			}
			group = (group + 1) % (WIDTH * HEIGHT);
		}

		int start = Random.Range (0, WIDTH * HEIGHT);
		rooms[start].type = Room.Type.Start;
		current = rooms [start];

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
		
	private Room GetNeighbor(int id, int direction)
	{
		if (0 > direction || Max <= direction) {
			throw new System.Exception ("room id:" + direction + ", invalid direction:" + direction);
		}
		switch (direction) {
		case North:
			if (0 < id - Dungeon.WIDTH) {
				return rooms [id - WIDTH];
			}
			break;
		case East:
			if ((id + 1) % WIDTH != 0) {
				return rooms [id + 1];
			}
			break;
		case South:
			if (id + WIDTH < WIDTH * HEIGHT) {
				return rooms [id + WIDTH];
			}
			break;
		case West:
			if ((id % WIDTH) - 1 >= 0) {
				return rooms [id - 1];
			}
			break;
		}
		return null;
	}
	private List<Room> GetOuterRoomsInGroup(int group) {
		List<Room> outerRooms = new List<Room> ();
		foreach (Room room in rooms) {
			if (group == room.group) {
				for (int direction = 0; direction < Max; direction++) {
					Room other = GetNeighbor(room.id, direction);
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
		for (int i = 0; i < WIDTH * HEIGHT; i++) {
			Room room = rooms [i];
			if (from == room.group) {
				room.group = to;
			}
		}
	}
}
