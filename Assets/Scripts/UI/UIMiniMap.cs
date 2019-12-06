using UnityEngine;
using System.Collections;

public class UIMiniMap : MonoBehaviour
{
	private const float ROOM_SIZE = 50.0f;

	public UIMiniMapRoom roomPrefab;
	public Sprite stair_mini_icon;
	public Sprite room_mini_icon;
	public Sprite monster_mini_icon;
	public Sprite treasure_mini_icon;
	public Sprite npc_mini_icon;
	public Color CURRENT_ROOM_COLOR = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	public Color VISIT_ROOM_COLOR = new Color(0.5f, 0.5f, 0.5f, 1.0f);
	public Color REVEAL_ROOM_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);

	private UIMiniMapRoom[] rooms;

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
				rooms [y * Dungeon.WIDTH + x] = room;
			}
		}
	}

	public void Init (Dungeon dungeon)
	{
		foreach (Room.Data room in dungeon.data.rooms)
		{
			rooms[room.id].Init(room);
		}
	}

	public void CurrentPosition(int id)
	{
		UIMiniMapRoom minimapRoom = rooms[id];

		for (int direction = 0; direction < Room.DirectionMax; direction++)
		{
			Room.Data nextRoom = minimapRoom.data.GetNext(direction);
			if (null == nextRoom)
			{
				continue;
			}

			RevealRoom(rooms[nextRoom.id]);
		}

		RevealRoom(minimapRoom);
		minimapRoom.color = CURRENT_ROOM_COLOR;
	}

	public void RevealMap()
	{
		foreach (UIMiniMapRoom minimapRoom in rooms)
		{
			if (true == minimapRoom.data.visit)
			{
				continue;
			}

			RevealRoom(minimapRoom);
			
			if (Room.Type.Exit == minimapRoom.data.type || Room.Type.Lock == minimapRoom.data.type)
			{
				minimapRoom.room.sprite = stair_mini_icon;
			}

			for (int i = 0; i < Room.DirectionMax; i++)
			{
				minimapRoom.next[i].gameObject.SetActive(false);
				Room.Data nextRoomData = minimapRoom.data.GetNext(i);
				if (null != nextRoomData)
				{
					minimapRoom.next[i].gameObject.SetActive(true);
				}
			}
		}
	}

	public void RevealTreasure()
	{
		foreach (UIMiniMapRoom minimapRoom in rooms)
		{
			if (null == minimapRoom.data.item)
			{
				continue;
			}

			RevealRoom(minimapRoom);
			minimapRoom.room.sprite = treasure_mini_icon;
		}
	}

	public void RevealMonster()
	{
		foreach (UIMiniMapRoom minimapRoom in rooms)
		{
			if (null == minimapRoom.data.monster)
			{
				continue;
			}
			
			RevealRoom(minimapRoom);
			minimapRoom.room.sprite = monster_mini_icon;
		}
	}

	public IEnumerator Hide(float time, float alpha = 0.0f)
    {
        float delta = 1.0f;
        while (alpha < delta)
        {
			foreach (UIMiniMapRoom minimapRoom in rooms)
			{
                Color color = minimapRoom.color;
                color.a *= delta;
				minimapRoom.color = color;
            }
            delta = delta - Time.deltaTime/time;
            yield return null;
        }

		foreach (UIMiniMapRoom minimapRoom in rooms)
		{
            Color color = minimapRoom.color;
            color.a = alpha;
			minimapRoom.color = color;
        }
    }

    public IEnumerator Show(float time)
    {
        float alpha = 0.0f;
        while (1.0f > alpha)
        {
			foreach (UIMiniMapRoom minimapRoom in rooms)
			{
                Color color = minimapRoom.color;
                color.a = Mathf.Max(color.a, alpha);
				minimapRoom.color = color;
			}
            alpha += Time.deltaTime / time;
            yield return null;
        }
    }

	private void RevealRoom(UIMiniMapRoom minimapRoom)
	{
		minimapRoom.gameObject.SetActive(true);
		minimapRoom.room.sprite = GetRoomSprite(minimapRoom);
		minimapRoom.color = REVEAL_ROOM_COLOR;

		if (true == minimapRoom.data.visit)
		{
			minimapRoom.color = VISIT_ROOM_COLOR;
			for (int i = 0; i < Room.DirectionMax; i++)
			{
				minimapRoom.next[i].gameObject.SetActive(false);
				Room.Data nextRoomData = minimapRoom.data.GetNext(i);
				if (null != nextRoomData)
				{
					minimapRoom.next[i].gameObject.SetActive(true);
					rooms[nextRoomData.id].next[(i + 2) % 4].gameObject.SetActive(false);
				}
			}
		}
	}

	private Sprite GetRoomSprite(UIMiniMapRoom minimapRoom)
	{
		Room.Data room = minimapRoom.data;
		if (true == room.visit)
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
		}
		else if (true == minimapRoom.gameObject.activeSelf)
		{
			return minimapRoom.room.sprite;
		}
		
		return this.room_mini_icon;
	}
}

