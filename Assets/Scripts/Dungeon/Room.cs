using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Room : MonoBehaviour
{
	public GameObject[] doors;
	public RoomStair stair;
	private void Start()
	{
		doors = new GameObject[Dungeon.Max];
		doors[Dungeon.North] = UIUtil.FindChild<Transform>(transform, "NorthDoor").gameObject;
		doors[Dungeon.East] = UIUtil.FindChild<Transform>(transform, "EastDoor").gameObject;
		doors[Dungeon.West] = UIUtil.FindChild<Transform>(transform, "WestDoor").gameObject;
		stair = UIUtil.FindChild<RoomStair>(transform, "Stair");
	}
	public void Init(Dungeon.Room data)
	{
		for (int i = 0; i < doors.Length; i++)
		{
			if (null != doors [i])
			{
				doors [i].SetActive ((bool)(null != data.next [i]));
			}
		}
		stair.gameObject.SetActive (false);
		if (Dungeon.Room.Type.Exit == data.type)
		{
			StartCoroutine(stair.Show());
		}
	}
}