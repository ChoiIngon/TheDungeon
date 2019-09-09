using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dungeon 
{
	public const int WIDTH = 5;
	public const int HEIGHT = 5;
	public const int North = 0;
	public const int East = 1;
	public const int South = 2;
	public const int West = 3;
	public const int Max = 4;

	[System.Serializable]
	public class LevelInfo
	{
		[System.Serializable]
		public class Item {
			public float chance;
	//		public ItemManager.GradeWeight[] grade_weight;
		}

		public int level;
		public string[] monsters;
		public Item items;
	}

	public class Room
	{
		public enum Type {
			Start,
			Exit,
			Lock,
			Normal
		}
		public int id = 0;
		public int group = 0;
		public bool visit = false;
		public Type type = Type.Normal;
		public Monster.Meta monster_meta = null;
		public Item item = null;
		public Room[] next = new Room[Max];
		
		public Room GetNext(int direction)
		{
			if (0 > direction || Max <= direction)
			{
				throw new System.Exception ("room id:" + direction + ", invalid direction:" + direction);
			}
			return next [direction];
		}
	}

	public Room current_room = null;
	public Room[] rooms = new Room[WIDTH * HEIGHT];
	// Use this for initialization
	public void Init(int dungeonLevel)
	{
		for(int i=0; i<WIDTH*HEIGHT; i++)
		{
			Room room = new Room ();
			room.id = i;
			room.group = i;
			rooms [i] = room;
		}
			
		int group = 0;
		while (true)
		{
			List<Room> outerRooms = GetOuterRoomsInGroup (group);
			if (0 < outerRooms.Count)
			{
				Room room = outerRooms [Random.Range (0, outerRooms.Count)];
				int direction = Random.Range (0, Max);
				for (int j = 0; j < Max; j++)
				{
					Room other = GetNeighbor (room.id, direction);
					if (null == other)
					{
						direction = (direction + 1) % Max;
						continue;
					}

					room.next [direction] = other;
					switch (direction)
					{
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

					if (other.group != room.group)
					{
						if (room.group < other.group)
						{
							ChangeGroupID (other.group, room.group);
						} else {
							ChangeGroupID (room.group, other.group);
						}
						break;
					}

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

		List<Room> candidates = new List<Room> (rooms);

		int start = Random.Range (0, candidates.Count);
		candidates[start].type = Room.Type.Start;
		candidates.RemoveAt (start);
		current_room = candidates[start];

		int end = Random.Range(0, candidates.Count);
		candidates[end].type = Room.Type.Exit;
		candidates.RemoveAt(end);

		Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
			"SELECT monster_id FROM meta_monster where monster_level<=" + dungeonLevel
		);
		List<string> monsterIDs = new List<string>();
		while (true == reader.Read())
		{
			monsterIDs.Add(reader.GetString("monster_id"));
		}

		if (0 < monsterIDs.Count)
		{
			int monsterCount = Random.Range(6, 10);
			for (int i = 0; i < monsterCount; i++)
			{
				int index = Random.Range(0, candidates.Count - 1);
				candidates[index].monster_meta = MonsterManager.Instance.FindMeta(monsterIDs[Random.Range(0, monsterIDs.Count - 1)]);
				candidates.RemoveAt(index);
			}
		}
	}

	public Room Move(int direction)
	{
		if (null == current_room.next [direction]) {
			return null;
		}
		current_room = current_room.next [direction];
		current_room.visit = true;
		return current_room;
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
