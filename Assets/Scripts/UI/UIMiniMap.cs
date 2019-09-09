using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMiniMap : MonoBehaviour
{
	public float ROOM_SIZE = 50.0f;
	public UIMiniMapRoom roomPrefab;
	public Sprite stairSprite;
	public Sprite roomSprite;
	private UIMiniMapRoom[] rooms;
	private int current_room_id;
	private Color ROOM_ACTIVATE_COLOR = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	private Color ROOM_DEACTIVATE_COLOR = new Color(1.0f, 1.0f, 1.0f, 0.2f);

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
		for (int y = 0; y < Dungeon.HEIGHT; y++)
		{
			for (int x = 0; x < Dungeon.WIDTH; x++)
			{
				UIMiniMapRoom miniRoom = rooms [y * Dungeon.WIDTH + x];
				Dungeon.Room room = dungeon.rooms [y * Dungeon.WIDTH + x];
				for (int i = 0; i < Dungeon.Max; i++)
				{
					miniRoom.next [i].gameObject.SetActive ((bool)(null != room.next [i]));
				}
				miniRoom.room.sprite = roomSprite;
				if (Dungeon.Room.Type.Exit == room.type)
				{
					miniRoom.room.sprite = stairSprite;
				}
				miniRoom.gameObject.SetActive (false);
			}
		}
		current_room_id = dungeon.current_room.id;
		CurrentPosition(current_room_id);
	}

	public void CurrentPosition (int id)
	{
		rooms [current_room_id].color = ROOM_DEACTIVATE_COLOR;
		rooms [id].color = ROOM_ACTIVATE_COLOR;
		rooms [id].gameObject.SetActive (true);
		current_room_id = id;
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

