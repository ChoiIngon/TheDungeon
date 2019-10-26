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
			for (int i = 0; i < Room.Max; i++)
			{
				next[i].color = value;
			}
		}
        get
        {
            return room.color;
        }
	}

	public void SetGateColor(int direction, Color color)
	{
		if (true == gameObject.activeSelf)
		{
			next[direction].color = color;
		}
	}
}
