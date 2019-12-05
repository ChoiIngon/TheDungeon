using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMiniMap : MonoBehaviour
{
	public UIMiniMapRoom roomPrefab;
	private float ROOM_SIZE = 50.0f;
	public Sprite stair_mini_icon;
	public Sprite room_mini_icon;
	public Sprite monster_mini_icon;
	public Sprite treasure_mini_icon;
	public Sprite npc_mini_icon;
	public Color CURRENT_ROOM_COLOR = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	public Color VISIT_ROOM_COLOR = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	public Color REVEAL_ROOM_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	private UIMiniMapRoom[] rooms;
	private Dungeon dungeon;

	void Awake()
	{
		RectTransform rectTransform = GetComponent<RectTransform> ();
		if (null == rectTransform)
		{
			throw new System.Exception("can not find component 'RectTransform'");
		}

		float width = Dungeon.WIDTH * ROOM_SIZE;
		float height = Dungeon.HEIGHT * ROOM_SIZE;
		rectTransform.sizeDelta = new Vector2 (width, height);

		rooms = new UIMiniMapRoom[Dungeon.WIDTH * Dungeon.HEIGHT];
		for (int y = 0; y < Dungeon.HEIGHT; y++)
		{
			for (int x = 0; x < Dungeon.WIDTH; x++)
			{
				UIMiniMapRoom room = GameObject.Instantiate<UIMiniMapRoom> (roomPrefab);
				if (null == room)
				{
					throw new System.Exception("can not instantiate prefab 'UIMiniMapRoom'");
				}

				if (4 != room.next.Length)
				{
					throw new System.Exception("door object count has to be '4'");
				}

				room.transform.SetParent(transform, false);
				room.transform.localPosition = new Vector3 (x * ROOM_SIZE, -y * ROOM_SIZE, 0.0f);
				room.Init();
				rooms [y * Dungeon.WIDTH + x] = room;
			}
		}
	}

	public void Init (Dungeon dungeon)
	{
		this.dungeon = dungeon;
		foreach (Room.Data room in dungeon.data.rooms)
		{
			rooms[room.id].Init();
		}
	}

	public void CurrentPosition(int id)
	{
		if (null == dungeon)
		{
			return;
		}

		for (int direction = 0; direction < Room.DirectionMax; direction++)
		{
			Room.Data nextRoom = dungeon.data.current_room.GetNext(direction);
			if (null != nextRoom)
			{
				RevealRoom(nextRoom);
				if (false == nextRoom.visit)
				{
					UIMiniMapRoom next = rooms[nextRoom.id];
					next.room.sprite = room_mini_icon;
				}
			}
		}

		RevealRoom(dungeon.data.current_room);
	}

	private void RevealRoom(Room.Data room)
	{
		UIMiniMapRoom minimapRoom = rooms[room.id];
		minimapRoom.gameObject.SetActive(true);
		minimapRoom.room.sprite = GetRoomSprite(room);
		minimapRoom.color = REVEAL_ROOM_COLOR;

		if (true == room.visit)
		{
			minimapRoom.color = VISIT_ROOM_COLOR;
			for (int i = 0; i < Room.DirectionMax; i++)
			{
				minimapRoom.next[i].gameObject.SetActive(false);
				Room.Data data = dungeon.data.rooms[room.id].GetNext(i);
				if (null != data)
				{
					minimapRoom.next[i].gameObject.SetActive(true);
					rooms[data.id].next[(i + 2) % 4].gameObject.SetActive(false);
				}
			}
		}

		if (dungeon.data.current_room.id == room.id)
		{
			minimapRoom.color = CURRENT_ROOM_COLOR;
		}
	}

	public void RevealMap()
	{
		foreach (Room.Data room in dungeon.data.rooms)
		{
			if (true == room.visit)
			{
				continue;
			}

			RevealRoom(room);
			if (false == room.visit)
			{
				UIMiniMapRoom minimapRoom = rooms[room.id];
				minimapRoom.room.sprite = this.room_mini_icon;
				if (Room.Type.Exit == dungeon.data.rooms[room.id].type || Room.Type.Lock == dungeon.data.rooms[room.id].type)
				{
					minimapRoom.room.sprite = stair_mini_icon;
				}
			}
		}
		RevealRoom(dungeon.data.current_room);
	}
	public void RevealTreasure()
	{
		foreach (Room.Data room in dungeon.data.rooms)
		{
			if (null == room.item)
			{
				continue;
			}
			bool active = rooms[room.id].gameObject.activeSelf;
			RevealRoom(room);
			if (false == active)
			{
				for (int i = 0; i < Room.DirectionMax; i++)
				{
					rooms[room.id].next[i].gameObject.SetActive(false);
				}
			}
		}
	}
	public void RevealMonster()
	{
		foreach (Room.Data room in dungeon.data.rooms)
		{
			if (null == room.monster)
			{
				continue;
			}
			bool active = rooms[room.id].gameObject.activeSelf;
			RevealRoom(room);
			if (false == active)
			{
				for (int i = 0; i < Room.DirectionMax; i++)
				{
					rooms[room.id].next[i].gameObject.SetActive(false);
				}
			}
		}
	}

	public IEnumerator Hide(float time, float alpha = 0.0f)
    {
        float delta = 1.0f;
        while (alpha < delta)
        {
            for (int id = 0; id < Dungeon.HEIGHT * Dungeon.WIDTH; id++)
            {
                UIMiniMapRoom miniRoom = rooms[id];
                Color color = miniRoom.color;
                color.a *= delta;
                miniRoom.color = color;
            }
            delta = delta - Time.deltaTime/time;
            yield return null;
        }

        for (int id = 0; id < Dungeon.HEIGHT * Dungeon.WIDTH; id++)
        {
            UIMiniMapRoom miniRoom = rooms[id];
            Color color = miniRoom.color;
            color.a = alpha;
            miniRoom.color = color;
        }
    }

    public IEnumerator Show(float time)
    {
        float alpha = 0.0f;
        while (1.0f > alpha)
        {
            for (int id = 0; id < Dungeon.HEIGHT * Dungeon.WIDTH; id++)
            {
                UIMiniMapRoom miniRoom = rooms[id];
                Color color = miniRoom.color;
                color.a = Mathf.Max(color.a, alpha);
                miniRoom.color = color;
				/*
				if (current_room_id == id - 1)
				{
					miniRoom.SetGateColor(Room.West, rooms[current_room_id].next[Room.East].color);
				}
				else if (current_room_id == id + 1)
				{
					miniRoom.SetGateColor(Room.East, rooms[current_room_id].next[Room.West].color);
				}
				else if (current_room_id == id - Dungeon.WIDTH)
				{
					miniRoom.SetGateColor(Room.North, rooms[current_room_id].next[Room.South].color);
				}
				else if (current_room_id == id + Dungeon.WIDTH)
				{
					miniRoom.SetGateColor(Room.South, rooms[current_room_id].next[Room.North].color);
				}
				*/
			}
            alpha += Time.deltaTime / time;
            yield return null;
        }
    }

	private Sprite GetRoomSprite(Room.Data room)
	{
		if (Room.Type.Exit == room.type || Room.Type.Lock == room.type)
		{
			return stair_mini_icon;
		}
		else if (null != room.monster)
		{
			return monster_mini_icon;
		}
		else if (null != room.item)
		{
			return treasure_mini_icon;
		}
		else if ("" != room.npc_sprite_path)
		{
			return npc_mini_icon;
		}
		return this.room_mini_icon;
	}


}

