using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMoveButtons : MonoBehaviour
{
	// Start is called before the first frame update
	private Button[] buttons = new Button[Dungeon.Max];
	private void Awake()
	{
		buttons[Dungeon.North] = UIUtil.FindChild<Button>(transform, "North");
		buttons[Dungeon.East] = UIUtil.FindChild<Button>(transform, "East");
		buttons[Dungeon.West] = UIUtil.FindChild<Button>(transform, "West");
		buttons[Dungeon.South] = UIUtil.FindChild<Button>(transform, "South");
	}

	void Start()
    {
		for (int i = 0; i < Dungeon.Max; i++)
		{
			buttons[i].enabled = false;
			buttons[i].image.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		}
		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Finish, OnMoveFinish);
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Finish, OnMoveFinish);
	}

	public void Init(Dungeon.Room room)
	{
		for (int i = 0; i < Dungeon.Max; i++)
		{
			Dungeon.Room nextRoom = room.next[i];
			if (null != nextRoom)
			{
				buttons[i].enabled = true;
				buttons[i].image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			}
			else
			{
				buttons[i].enabled = false;
				buttons[i].image.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
			}
		}
	}
	private void OnMoveStart()
	{
		gameObject.SetActive(false);
	}

	private void OnMoveFinish()
	{
		gameObject.SetActive(true);
	}
}
