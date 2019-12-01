using UnityEngine;
using System.Collections.Generic;

public class Dungeon : MonoBehaviour
{
	public const int WIDTH = 5;
	public const int HEIGHT = 5;
	public const int EXIT_LOCK_CHANCE = 100;
	public const float ROOM_SIZE = 7.2f; // walkDistance;
	public const float ROOM_MOVE_SPEED = 17.0f;

	public int level
	{
		get { return data.level; }
	}

	public int max_level
	{
		get { return data.max_level; }
	}
	public class Data
	{
		public Room.Data current_room = null;
		public Room.Data[] rooms = new Room.Data[WIDTH * HEIGHT];
		public int level = 1;
		public int max_level;
		// Use this for initialization
		public void Init(int dungeonLevel)
		{
			level = dungeonLevel;
			max_level = GetMaxDungeonLevel();
			for (int i = 0; i < WIDTH * HEIGHT; i++)
			{
				Room.Data room = new Room.Data();
				room.id = i;
				room.group = i;
				rooms[i] = room;
			}

			int group = 0;
			while (true)
			{
				List<Room.Data> outerRooms = GetOuterRoomsInGroup(group);
				if (0 < outerRooms.Count)
				{
					Room.Data room = outerRooms[Random.Range(0, outerRooms.Count)];
					int directionCount = Random.Range(0, 3);
					for (int i = 0; i < directionCount; i++)
					{
						int direction = Random.Range(0, Room.Max);
						for (int j = 0; j < Room.Max; j++)
						{
							Room.Data other = GetNextRoom(room.id, direction);
							if (null == other)
							{
								direction = (direction + 1) % Room.Max;
								continue;
							}

							room.nexts[direction] = other;
							switch (direction)
							{
								case Room.North:
									other.nexts[Room.South] = room;
									break;
								case Room.East:
									other.nexts[Room.West] = room;
									break;
								case Room.South:
									other.nexts[Room.North] = room;
									break;
								case Room.West:
									other.nexts[Room.East] = room;
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

							direction = (direction + 1) % Room.Max;
						}
					}
				}
				int roomCountInGroupZero = 0;
				foreach (Room.Data room in rooms)
				{
					if (0 == room.group)
					{
						roomCountInGroupZero++;
					}
				}
				if (roomCountInGroupZero == WIDTH * HEIGHT)
				{
					break;
				}
				group = (group + 1) % (WIDTH * HEIGHT);
			}

			List<Room.Data> candidates = new List<Room.Data>(rooms);
			int start = Random.Range(0, candidates.Count);
			rooms[start].type = Room.Type.Start;
			current_room = rooms[start];
			candidates.RemoveAt(start);

			Util.Sqlite.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT monster_id, monster_count, reward_item_chance, reward_item_id FROM meta_dungeon_monster WHERE dungeon_level=" + Mathf.Max(1, dungeonLevel % (max_level+1))
			);
			
			while (true == reader.Read())
			{
				int monsterCount = reader.GetInt32("monster_count");
				for (int i = 0; i < monsterCount; i++)
				{
					if (0 == candidates.Count)
					{
						break;
					}

					int index = Random.Range(0, candidates.Count);
					Room.Data room = candidates[index];
					room.monster = MonsterManager.Instance.FindMeta(reader.GetString("monster_id"));
					room.monster.reward_item_chance = reader.GetFloat("reward_item_chance");
					room.monster.reward.item_id = reader.GetString("reward_item_id");
					candidates.RemoveAt(index);
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
				Room.Data room = candidates[index];
				room.item = ItemManager.Instance.GetRandomExpendableItemMeta();
				candidates.RemoveAt(index);
			}

			{
				int index = Random.Range(0, candidates.Count);
				Room.Data room = candidates[index];
				room.type = Room.Type.Shop;
			}

			bool exitLock = false;
			bool keyItem = false;
			foreach (Room.Data room in rooms)
			{
				if (null == room.item)
				{
					continue;
				}
				if (room.item.id == "ITEM_KEY")
				{
					exitLock = true;
					keyItem = true;
					break;
				}
			}

			if (false == exitLock)
			{
				exitLock = 30 > Random.Range(0, 100);
			}

			if (true == exitLock && false == keyItem)
			{
				int index = Random.Range(0, candidates.Count);
				candidates[index].item = ItemManager.Instance.FindMeta<KeyItem.Meta>("ITEM_KEY");
				candidates.RemoveAt(index);
			}

			candidates.RemoveAll(room => room.id == current_room.id + WIDTH);
			candidates.RemoveAll(room => room.id == current_room.id + 1);
			candidates.RemoveAll(room => room.id == current_room.id - 1);
			candidates.RemoveAll(room => room.id == current_room.id - WIDTH);

			int end = Random.Range(0, candidates.Count);
			Room.Data exit = candidates[end];
			exit.type = Room.Type.Exit;
			if (true == exitLock)
			{
				exit.type = Room.Type.Lock;
			}
			candidates.RemoveAt(end);
		}

		public Room.Data Move(int direction)
		{
			if (null == current_room.nexts[direction])
			{
				return null;
			}
			current_room = current_room.nexts[direction];
			current_room.visit = true;
			return current_room;
		}

		private Room.Data GetNextRoom(int id, int direction)
		{
			if (0 > direction || Room.Max <= direction)
			{
				throw new System.Exception("room id:" + direction + ", invalid direction:" + direction);
			}
			switch (direction)
			{
				case Room.North:
					if (0 < id - Dungeon.WIDTH)
					{
						return rooms[id - WIDTH];
					}
					break;
				case Room.East:
					if ((id + 1) % WIDTH != 0)
					{
						return rooms[id + 1];
					}
					break;
				case Room.South:
					if (id + WIDTH < WIDTH * HEIGHT)
					{
						return rooms[id + WIDTH];
					}
					break;
				case Room.West:
					if ((id % WIDTH) - 1 >= 0)
					{
						return rooms[id - 1];
					}
					break;
			}
			return null;
		}
		private List<Room.Data> GetOuterRoomsInGroup(int group)
		{
			List<Room.Data> outerRooms = new List<Room.Data>();
			foreach (Room.Data room in rooms)
			{
				if (group == room.group)
				{
					for (int direction = 0; direction < Room.Max; direction++)
					{
						Room.Data other = GetNextRoom(room.id, direction);
						if (null != other && group != other.group)
						{
							outerRooms.Add(room);
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
				Room.Data room = rooms[i];
				if (from == room.group)
				{
					room.group = to;
				}
			}
		}

		private int GetMaxDungeonLevel()
		{
			Util.Sqlite.DataReader reader = Database.Execute(Database.Type.MetaData,
				"select ifnull(max(dungeon_level), 0) max_dungeon_level from meta_dungeon_monster;"
			);

			while (true == reader.Read())
			{
				return reader.GetInt32("max_dungeon_level");
			}
			return 0;
		}
	}

	public Data data;
	public Room current_room;
	private readonly Room[] next_rooms = new Room[Room.Max];
	private SpriteRenderer south_door_arrow;
	private void Awake()
	{
		current_room = UIUtil.FindChild<Room>(transform, "Current");
		next_rooms[Room.North] = UIUtil.FindChild<Room>(transform, "North");
		next_rooms[Room.East] = UIUtil.FindChild<Room>(transform, "East");
		next_rooms[Room.South] = UIUtil.FindChild<Room>(transform, "South");
		next_rooms[Room.West] = UIUtil.FindChild<Room>(transform, "West");
		south_door_arrow = UIUtil.FindChild<SpriteRenderer>(transform, "SouthDoorArrow");
		data = new Data();

		Util.EventSystem.Subscribe(EventID.Dungeon_Battle_Start, OnDungeonBattleStart);
		Util.EventSystem.Subscribe<DungeonBattle.BattleResult>(EventID.Dungeon_Battle_Finish, OnDungeonBattleFinish);
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Battle_Start, OnDungeonBattleStart);
		Util.EventSystem.Unsubscribe<DungeonBattle.BattleResult>(EventID.Dungeon_Battle_Finish, OnDungeonBattleFinish);
	}

	private void Start()
	{
		next_rooms[Room.North].transform.position = new Vector3(0.0f, 0.0f, ROOM_SIZE);
		next_rooms[Room.East].transform.position = new Vector3(ROOM_SIZE, 0.0f, 0.0f);
		next_rooms[Room.South].transform.position = new Vector3(0.0f, 0.0f, -ROOM_SIZE);
		next_rooms[Room.West].transform.position = new Vector3(-ROOM_SIZE, 0.0f, 0.0f);
	}

	public void Init(int dungeonLevel)
	{
		data.Init(dungeonLevel);
	}

	public void InitRooms()
	{
		transform.position = Vector3.zero;
		current_room.Init(data.current_room);
		for (int i = 0; i < Room.Max; i++)
		{
			Room.Data room = data.current_room.nexts[i];
			next_rooms[i].Init(room);
		}

		south_door_arrow.gameObject.SetActive(false);
		if (null != data.current_room.nexts[Room.South])
		{
			south_door_arrow.gameObject.SetActive(true);
		}
	}

	private void OnDungeonBattleStart()
	{
		south_door_arrow.gameObject.SetActive(false);
	}

	private void OnDungeonBattleFinish(DungeonBattle.BattleResult result)
	{
		if (null != data.current_room.nexts[Room.South])
		{
			south_door_arrow.gameObject.SetActive(true);
		}
	}
}
