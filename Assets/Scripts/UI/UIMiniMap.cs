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
	private int current;
	private Color activateColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	private Color deactivateColor = new Color(1.0f, 1.0f, 1.0f, 0.2f);

	void Awake()
	{
		{
			RectTransform rectTransform = GetComponent<RectTransform> ();
			float width = Dungeon.WIDTH * ROOM_SIZE;
			float height = Dungeon.HEIGHT * ROOM_SIZE;
			rectTransform.sizeDelta = new Vector2 (width, height);
		}

		rooms = new UIMiniMapRoom[Dungeon.WIDTH * Dungeon.HEIGHT];
		for (int y = 0; y < Dungeon.HEIGHT; y++) {
			for (int x = 0; x < Dungeon.WIDTH; x++) {
				UIMiniMapRoom room = GameObject.Instantiate<UIMiniMapRoom> (roomPrefab);
                UnityEngine.Assertions.Assert.AreNotEqual(null, room);
				room.transform.SetParent (this.transform, false);
				room.transform.localPosition = new Vector3 (x * ROOM_SIZE, -y * ROOM_SIZE, 0.0f);
				room.gameObject.SetActive (false);
				rooms [y * Dungeon.WIDTH + x] = room;
			}
		}
	}

	public void Init ()
	{
		for (int y = 0; y < Dungeon.HEIGHT; y++) {
			for (int x = 0; x < Dungeon.WIDTH; x++) {
				UIMiniMapRoom miniRoom = rooms [y * Dungeon.WIDTH + x];
				Dungeon.Room room = Dungeon.Instance.rooms [y * Dungeon.WIDTH + x];
				for (int i = 0; i < Dungeon.Max; i++) {
					miniRoom.next [i].gameObject.SetActive ((bool)(null != room.next [i]));
				}
				miniRoom.room.sprite = roomSprite;
				if (Dungeon.Room.Type.Exit == room.type) {
					miniRoom.room.sprite = stairSprite;
				}
				miniRoom.gameObject.SetActive (false);
			}
		}
		current = Dungeon.Instance.current.id;
		rooms [current].color = activateColor;
	}

	public void CurrentPosition (int id)
	{
		rooms [current].color = deactivateColor;
		rooms [id].color = activateColor;
		rooms [id].gameObject.SetActive (true);
		current = id;
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
                if(current == id)
                {
                    color.a = Mathf.Min(activateColor.a, color.a);
                }
                else
                {
                    color.a = Mathf.Min(deactivateColor.a, color.a);
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
            if (current == id)
            {
                color.a = activateColor.a;
            }
            else
            {
                color.a = deactivateColor.a;
            }
            miniRoom.color = color;
        }
    }
}

