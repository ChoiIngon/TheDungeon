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
	private const int EXIT_LOCK_CHANCE = 100;

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
		public Item.Meta item = null;
		public float item_chance = 0.0f;
		public Monster.Meta monster = null;
		public Room[] next = new Room[Max];
		public string npc_sprite_path = "";
		
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
	public int level = 1;	
	// Use this for initialization
	public void Init(int dungeonLevel)
	{
		level = dungeonLevel;
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
				int directionCount = Random.Range(1, 3);
				for (int i = 0; i < directionCount; i++)
				{
					int direction = Random.Range(0, Max);
					for (int j = 0; j < Max; j++)
					{
						Room other = GetNeighbor(room.id, direction);
						if (null == other)
						{
							direction = (direction + 1) % Max;
							continue;
						}

						room.next[direction] = other;
						switch (direction)
						{
							case North:
								other.next[South] = room;
								break;
							case East:
								other.next[West] = room;
								break;
							case South:
								other.next[North] = room;
								break;
							case West:
								other.next[East] = room;
								break;
						}

						if (other.group != room.group)
						{
							if (room.group < other.group)
							{
								ChangeGroupID(other.group, room.group);
							}
							else
							{
								ChangeGroupID(room.group, other.group);
							}
							break;
						}

						direction = (direction + 1) % Max;
					}
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

		List<Room> candidates = new List<Room>(rooms);
		int start = Random.Range(0, candidates.Count);
		rooms[start].type = Room.Type.Start;
		current_room = rooms[start];
		candidates.RemoveAt(start);
		candidates.RemoveAll(room => room.id == current_room.id + WIDTH);
		candidates.RemoveAll(room => room.id == current_room.id + 1);
		candidates.RemoveAll(room => room.id == current_room.id - 1);
		candidates.RemoveAll(room => room.id == current_room.id - WIDTH);

		int end = Random.Range(0, candidates.Count);
		Room exit = candidates[end];
		exit.type = Room.Type.Exit;
		candidates.RemoveAt(end);

		candidates = new List<Room>(rooms);
		candidates.RemoveAll(room => (room.type == Room.Type.Start || room.type == Room.Type.Exit));
		{
			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT monster_id, monster_count, reward_item_chance, reward_item_id FROM meta_dungeon_monster WHERE dungeon_level=" + dungeonLevel
			);

			while (true == reader.Read())
			{
				int monsterCount = reader.GetInt32("monster_count");
				for (int i = 0; i < monsterCount; i++)
				{
					int index = Random.Range(0, candidates.Count);
					Room room = candidates[index];
					room.monster = MonsterManager.Instance.FindMeta(reader.GetString("monster_id"));
					room.item_chance = reader.GetFloat("reward_item_chance");
					if (0.0f < room.item_chance)
					{
						string rewardItemID = reader.GetString("reward_item_id");
						if ("" == rewardItemID)
						{
							room.item = EquipItemManager.Instance.GetRandomMeta();
						}
						else
						{
							room.item = ItemManager.Instance.FindMeta<Item.Meta>(rewardItemID);
						}
					}

					candidates.RemoveAt(index);
				}
			}
		}

		int itemBoxCount = Random.Range(0, 5);
		
		for (int i = 0; i < itemBoxCount; i++)
		{
			if (0 == candidates.Count)
			{
				break;
			}
			int index = Random.Range(0, candidates.Count);
			Room room = candidates[index];
			room.item = ItemManager.Instance.GetRandomExpendableItemMeta();
			candidates.RemoveAt(index);
		}

		candidates = new List<Room>(rooms);
		bool exitLock = false;
		int keyItemCount = 0;
		foreach (Room candidate in candidates)
		{
			if (null == candidate.item)
			{
				continue;
			}
			if (candidate.item.id == "ITEM_KEY")
			{
				exitLock = true;
				keyItemCount = 1;
				break;
			}
		}
		if (false == exitLock)
		{
			exitLock = 30 > Random.Range(0, 100);
		}

		if (true == exitLock)
		{
			exit.type = Dungeon.Room.Type.Lock;
			if(0 == keyItemCount)
			{
				candidates.RemoveAll(room => (room.type == Room.Type.Start || room.type == Room.Type.Lock));
				int index = Random.Range(0, candidates.Count);
				candidates[index].item = ItemManager.Instance.FindMeta<KeyItem.Meta>("ITEM_KEY");
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
		if (0 > direction || Max <= direction)
		{
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
	private List<Room> GetOuterRoomsInGroup(int group)
	{
		List<Room> outerRooms = new List<Room> ();
		foreach (Room room in rooms)
		{
			if (group == room.group)
			{
				for (int direction = 0; direction < Max; direction++)
				{
					Room other = GetNeighbor(room.id, direction);
                    if (null != other && group != other.group)
					{
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
		for (int i = 0; i < WIDTH * HEIGHT; i++)
		{
			Room room = rooms [i];
			if (from == room.group) {
				room.group = to;
			}
		}
	}
}
