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
			for (int i = 0; i < Room.DirectionMax; i++)
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

	public void Init()
	{
		for (int direction = 0; direction < Room.DirectionMax; direction++)
		{
			next[direction].gameObject.SetActive(false);
		}
		gameObject.SetActive(false);
	}
}
