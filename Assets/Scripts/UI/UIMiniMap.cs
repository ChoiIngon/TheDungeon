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
}
