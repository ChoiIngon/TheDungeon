using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIMiniMap : MonoBehaviour
{
	public float ROOM_SIZE = 19.0f;
	public float size = 0.0f;
	public GameObject roomPrefab;
	public Image currentPosition;
	private GameObject[] rooms;
	void Awake()
	{
		int roomID = 0;
		RectTransform t = GetComponent<RectTransform> ();
		t.sizeDelta = new Vector2 (t.rect.height, t.sizeDelta.y);
		ROOM_SIZE = t.rect.height / Dungeon.HEIGHT;
		float scale = ROOM_SIZE / 19.0f;
		currentPosition.GetComponent<RectTransform> ().localScale = new Vector3 (scale, scale, 1.0f);

		rooms = new GameObject[Dungeon.WIDTH * Dungeon.HEIGHT];
		for (int y = 0; y < Dungeon.HEIGHT; y++) {
			for (int x = 0; x < Dungeon.WIDTH; x++) {
				GameObject obj = GameObject.Instantiate<GameObject> (roomPrefab);
				obj.transform.SetParent (transform);
				obj.transform.localScale = new Vector3 (scale, scale, 1.0f);
				obj.transform.localPosition = new Vector3 (x * ROOM_SIZE - ROOM_SIZE * Dungeon.WIDTH, -y * ROOM_SIZE, 0.0f);
				obj.SetActive (false);
				rooms [y * Dungeon.WIDTH + x] = obj;
			}
		}
	}
	public void Init ()
	{
		int roomID = 0;
		RectTransform t = GetComponent<RectTransform> ();
		t.sizeDelta = new Vector2 (t.rect.height, t.sizeDelta.y);
		ROOM_SIZE = t.rect.height / Dungeon.HEIGHT;
		float scale = ROOM_SIZE / 19.0f;
		currentPosition.GetComponent<RectTransform> ().localScale = new Vector3 (scale, scale, 1.0f);

		for (int y = 0; y < Dungeon.HEIGHT; y++) {
			for (int x = 0; x < Dungeon.WIDTH; x++) {
				GameObject obj = rooms [y * Dungeon.WIDTH + x];
				Dungeon.Room room = Dungeon.Instance.rooms [roomID++];
				obj.transform.FindChild ("Room/NorthDoor").gameObject.SetActive (true);
				obj.transform.FindChild ("Room/EastDoor").gameObject.SetActive (true);
				obj.transform.FindChild ("Room/SouthDoor").gameObject.SetActive (true);
				obj.transform.FindChild ("Room/WestDoor").gameObject.SetActive (true);
				if (null == room.next [Dungeon.North]) {
					obj.transform.FindChild ("Room/NorthDoor").gameObject.SetActive (false);
				}
				if (null == room.next [Dungeon.East]) {
					obj.transform.FindChild ("Room/EastDoor").gameObject.SetActive (false);
				}
				if (null == room.next [Dungeon.South]) {
					obj.transform.FindChild ("Room/SouthDoor").gameObject.SetActive (false);
				}
				if (null == room.next [Dungeon.West]) {
					obj.transform.FindChild ("Room/WestDoor").gameObject.SetActive (false);
				}
				obj.SetActive (false);
			}
		}

		currentPosition.transform.SetSiblingIndex (transform.childCount);
		CurrentPosition (Dungeon.Instance.current.id);
	}

	public void CurrentPosition (int id)
	{
		int y = id / Dungeon.WIDTH;
		int x = id % Dungeon.WIDTH;
		currentPosition.transform.localPosition = new Vector3 (x * ROOM_SIZE - ROOM_SIZE * Dungeon.WIDTH, -y * ROOM_SIZE, 0.0f);
		rooms [id].SetActive (true);
	}
}
