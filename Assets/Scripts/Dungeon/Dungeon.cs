using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dungeon : MonoBehaviour
{
	public const int WIDTH = 5;
	public const int HEIGHT = 5;
	
	private const int EXIT_LOCK_CHANCE = 100;
	private const float ROOM_SIZE = 7.2f; // walkDistance;
	private const float ROOM_MOVE_SPEED = 17.0f;

	public int level
	{
		get { return data.level; }
	}

	public class Data
	{
		public Room.Data current_room = null;
		public Room.Data[] rooms = new Room.Data[WIDTH * HEIGHT];
		public int level = 1;

		// Use this for initialization
		public void Init(int dungeonLevel)
		{
			level = dungeonLevel;
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
					int directionCount = Random.Range(1, 3);
					for (int i = 0; i < directionCount; i++)
					{
						int direction = Random.Range(0, Room.Max);
						for (int j = 0; j < Room.Max; j++)
						{
							Room.Data other = GetNeighbor(room.id, direction);
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

			Util.Database.DataReader reader = Database.Execute(Database.Type.MetaData,
				"SELECT monster_id, monster_count, reward_item_chance, reward_item_id FROM meta_dungeon_monster WHERE dungeon_level=" + dungeonLevel
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

		private Room.Data GetNeighbor(int id, int direction)
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
						Room.Data other = GetNeighbor(room.id, direction);
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
	}

	public Data data;
	public Room current_room;
	private readonly Room[] next_rooms = new Room[Room.Max];
	private void Awake()
	{
		current_room = UIUtil.FindChild<Room>(transform, "Current");
		next_rooms[Room.North] = UIUtil.FindChild<Room>(transform, "North");
		next_rooms[Room.East] = UIUtil.FindChild<Room>(transform, "East");
		next_rooms[Room.South] = UIUtil.FindChild<Room>(transform, "South");
		next_rooms[Room.West] = UIUtil.FindChild<Room>(transform, "West");

		data = new Data();
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
		InitRooms();
	}

	private void InitRooms()
	{
		transform.position = Vector3.zero;

		current_room.Init(data.current_room);

		for (int i = 0; i < Room.Max; i++)
		{
			Room.Data room = data.current_room.nexts[i];
			if (null != room)
			{
				next_rooms[i].Init(room);
			}
		}
	}

	public IEnumerator Move(int direction)
	{
		Room.Data nextRoom = data.current_room.GetNext(direction);

		if (null == nextRoom)
		{
			switch (direction)
			{
				case Room.North:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(0.0f, 0.0f, 1.0f), 0.5f);
					break;
				case Room.East:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(1.0f, 0.0f, 0.0f), 0.5f);
					break;
				case Room.South:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(0.0f, 0.0f, -1.0f), 0.5f);
					break;
				case Room.West:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(-1.0f, 0.0f, 0.5f), 0.5f);
					break;
				default:
					break;
			}
			yield break;
		}

		GameManager.Instance.player.move_count++;

		Vector3 position = Vector3.zero;
		switch (direction)
		{
			case Room.North:
				position = new Vector3(transform.position.x, transform.position.y, transform.position.z - ROOM_SIZE);
				break;
			case Room.East:
				position = new Vector3(transform.position.x - ROOM_SIZE, transform.position.y, transform.position.z);
				break;
			case Room.South:
				position = new Vector3(transform.position.x, transform.position.y, transform.position.z + ROOM_SIZE);
				break;
			case Room.West:
				position = new Vector3(transform.position.x + ROOM_SIZE, transform.position.y, transform.position.z);
				break;
			default:
				break;
		}

		data.Move(direction);
		AudioManager.Instance.Play(AudioManager.DUNGEON_WALK, true);
		move_complete = false;
		iTween.MoveTo(gameObject, iTween.Hash("position", position, "time", ROOM_SIZE / ROOM_MOVE_SPEED, "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "OnMoveComplete"));
		while (false == move_complete)
		{
			yield return null;
		}
		AudioManager.Instance.Stop(AudioManager.DUNGEON_WALK);

		InitRooms();
	}
	
	private bool move_complete = false;
	void OnMoveComplete()
	{
		move_complete = true;
	}
}
