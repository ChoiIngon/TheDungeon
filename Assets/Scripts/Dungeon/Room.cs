using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
	public GameObject[] doors;
	public RoomStair stair;
	public DungeonBox box;
	public SpriteRenderer npc;
	private void Awake()
	{
		doors = new GameObject[Dungeon.Max];
		doors[Dungeon.North] = UIUtil.FindChild<Transform>(transform, "NorthDoor").gameObject;
		doors[Dungeon.East] = UIUtil.FindChild<Transform>(transform, "EastDoor").gameObject;
		doors[Dungeon.West] = UIUtil.FindChild<Transform>(transform, "WestDoor").gameObject;
		stair = UIUtil.FindChild<RoomStair>(transform, "Stair");
		stair.gameObject.SetActive(true);
		box = UIUtil.FindChild<DungeonBox>(transform, "Box");
		box.gameObject.SetActive(true);
		npc = UIUtil.FindChild<SpriteRenderer>(transform, "Npc");
	}

	public void Init(Dungeon.Room data)
	{
		box.gameObject.SetActive(false);
		stair.gameObject.SetActive(false);
		npc.gameObject.SetActive(false);

		if (null == data)
		{
			return;	
		}

		for (int i = 0; i < doors.Length; i++)
		{
			if (null != doors[i])
			{
				doors[i].SetActive((bool)(null != data.next[i]));
			}
		}		   		

		if (Dungeon.Room.Type.Exit == data.type || Dungeon.Room.Type.Lock == data.type)
		{
			stair.Show(data);
		}

		if ("" != data.npc_sprite_path)
		{
			npc.gameObject.SetActive(true);
			npc.sprite = ResourceManager.Instance.Load<Sprite>(data.npc_sprite_path);
		}
	}
}