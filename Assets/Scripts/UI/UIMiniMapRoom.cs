using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMapRoom : MonoBehaviour
{
	public Image room;
	public Image[] next;
	public Color color
	{
		set
		{
			room.color = value;
			for (int i = 0; i < Dungeon.Max; i++)
			{
				next [i].color = value;
			}
		}
        get
        {
            return room.color;
        }
	}
}
