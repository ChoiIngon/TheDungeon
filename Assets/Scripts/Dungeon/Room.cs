using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
	public GameObject[] doors;
	public RoomStair stair;
	public DungeonBox box;
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
	}

	public void Init(Dungeon.Room data)
	{
		box.gameObject.SetActive(false);
		stair.gameObject.SetActive(false);

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

		if (null != data.item)
		{
			box.Show(data);
		}

		if (Dungeon.Room.Type.Exit == data.type || Dungeon.Room.Type.Lock == data.type)
		{
			stair.Show(data);
		}
	}
}