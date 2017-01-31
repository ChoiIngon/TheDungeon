using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class Room : MonoBehaviour
{
	public GameObject[] doors;
	public void Init(Dungeon.Room data)
	{
		for (int i = 0; i < doors.Length; i++) {
			if (null != doors [i]) {
				doors [i].SetActive ((bool)(null != data.next [i]));
			}
		}
	}
}