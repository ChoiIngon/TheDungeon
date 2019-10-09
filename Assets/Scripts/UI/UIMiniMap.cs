using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMiniMap : MonoBehaviour
{
	public UIMiniMapRoom roomPrefab;
	private float ROOM_SIZE = 50.0f;
	public Sprite stair;
	public Sprite room;
	public Sprite monster;
	public Sprite box;

	private UIMiniMapRoom[] rooms;
	private int current_room_id;
	private Color ROOM_ACTIVATE_COLOR = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	private Color ROOM_DEACTIVATE_COLOR = new Color(1.0f, 1.0f, 1.0f, 0.2f);
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
				room.transform.SetParent (this.transform, false);
				room.transform.localPosition = new Vector3 (x * ROOM_SIZE, -y * ROOM_SIZE, 0.0f);
				room.gameObject.SetActive (false);
				rooms [y * Dungeon.WIDTH + x] = room;
			}
		}
	}

	public void Init (Dungeon dungeon)
	{
		this.dungeon = dungeon;
		foreach (Dungeon.Room room in dungeon.rooms)
		{
			UIMiniMapRoom miniRoom = rooms[room.id];

			if (Dungeon.Room.Type.Exit == room.type || Dungeon.Room.Type.Lock == room.type)
			{
				miniRoom.room.sprite = stair;
			}
			else if (null != room.monster)
			{
				miniRoom.room.sprite = monster;
			}
			else if (null != room.item)
			{
				miniRoom.room.sprite = box;
			}
			else
			{
				miniRoom.room.sprite = this.room;
			}
			miniRoom.gameObject.SetActive(false);
		}

		current_room_id = dungeon.current_room.id;
		CurrentPosition(current_room_id);
		RevealBox();
	}

	public void CurrentPosition (int id)
	{
		UIMiniMapRoom minimapRoom = rooms[current_room_id];
		if (null != dungeon.rooms[current_room_id].monster)
		{
			minimapRoom.room.sprite = monster;
		}
		else if (null != dungeon.rooms[current_room_id].item)
		{
			minimapRoom.room.sprite = box;
		}
		else
		{
			minimapRoom.room.sprite = this.room;
		}
		minimapRoom.color = ROOM_DEACTIVATE_COLOR;

		RevealRoom(id, ROOM_ACTIVATE_COLOR);

		current_room_id = id;
	}

	private void RevealRoom(int id, Color color)
	{
		UIMiniMapRoom minimapRoom = rooms[id];
		minimapRoom.color = color;
		minimapRoom.gameObject.SetActive(true);
		for (int i = 0; i < Dungeon.Max; i++)
		{
			minimapRoom.next[i].gameObject.SetActive((bool)(null != dungeon.rooms[id].next[i]));
		}
	}
	public void RevealMap()
	{
		foreach (Dungeon.Room room in dungeon.rooms)
		{
			if (true == room.visit)
			{
				continue;
			}
			RevealRoom(room.id, ROOM_DEACTIVATE_COLOR);
		}
	}
	public void RevealBox()
	{
		foreach (Dungeon.Room room in dungeon.rooms)
		{
			if (true == room.visit)
			{
				continue;
			}

			if (null == room.item)
			{
				continue;
			}

			RevealRoom(room.id, ROOM_DEACTIVATE_COLOR);
			for (int i = 0; i < Dungeon.Max; i++)
			{
				rooms[room.id].next[i].gameObject.SetActive(false);
			}
		}
	}
	public void RevealMonster()
	{
		foreach (Dungeon.Room room in dungeon.rooms)
		{
			if (true == room.visit)
			{
				continue;
			}
			if (null == room.monster)
			{
				continue;
			}

			RevealRoom(room.id, ROOM_DEACTIVATE_COLOR);
			for (int i = 0; i < Dungeon.Max; i++)
			{
				rooms[room.id].next[i].gameObject.SetActive(false);
			}
		}
	}
	public IEnumerator Hide(float time)
    {
        float alpha = 1.0f;
        while (0.0f < alpha)
        {
            for (int id = 0; id < Dungeon.HEIGHT * Dungeon.WIDTH; id++)
            {
                UIMiniMapRoom miniRoom = rooms[id];
                Color color = miniRoom.color;
                color.a *= alpha;
                miniRoom.color = color;
            }
            alpha = alpha - Time.deltaTime/time;
            yield return null;
        }
        for (int id = 0; id < Dungeon.HEIGHT * Dungeon.WIDTH; id++)
        {
            UIMiniMapRoom miniRoom = rooms[id];
            Color color = miniRoom.color;
            color.a = 0.0f;
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
                color.a = alpha;
                if(current_room_id == id)
                {
                    color.a = Mathf.Min(ROOM_ACTIVATE_COLOR.a, color.a);
                }
                else
                {
                    color.a = Mathf.Min(ROOM_DEACTIVATE_COLOR.a, color.a);
                }
                miniRoom.color = color;
            }
            alpha += Time.deltaTime / time;
            yield return null;
        }
        for (int id = 0; id < Dungeon.HEIGHT * Dungeon.WIDTH; id++)
        {
            UIMiniMapRoom miniRoom = rooms[id];
            Color color = miniRoom.color;
            if (current_room_id == id)
            {
                color.a = ROOM_ACTIVATE_COLOR.a;
            }
            else
            {
                color.a = ROOM_DEACTIVATE_COLOR.a;
            }
            miniRoom.color = color;
        }
    }
}

