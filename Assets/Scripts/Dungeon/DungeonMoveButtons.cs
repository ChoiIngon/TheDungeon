using UnityEngine;
using UnityEngine.UI;

public class DungeonMoveButtons : MonoBehaviour
{
	public TouchInput touch_input;
	private Vector3 touch_point = Vector3.zero;
	private bool touch_finish = false;

	private Button[] buttons = new Button[Room.Max];
	private void Awake()
	{
		touch_input = UIUtil.FindChild<TouchInput>(transform, "../../");
		buttons[Room.North] = UIUtil.FindChild<Button>(transform, "North");
		buttons[Room.East] = UIUtil.FindChild<Button>(transform, "East");
		buttons[Room.West] = UIUtil.FindChild<Button>(transform, "West");
		buttons[Room.South] = UIUtil.FindChild<Button>(transform, "South");

		Util.EventSystem.Subscribe<int>(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Finish, OnMoveFinish);
		Util.EventSystem.Subscribe(EventID.Shop_Open, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Shop_Close, () => { touch_input.ReleaseBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Inventory_Open, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Inventory_Close, () => { touch_input.ReleaseBlockCount(); });
		Util.EventSystem.Subscribe(EventID.TextBox_Open, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.TextBox_Close, () => { touch_input.ReleaseBlockCount(); });
	}

	void Start()
    {
		UIUtil.AddPointerUpListener(buttons[Room.North].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.North);
		});
		UIUtil.AddPointerUpListener(buttons[Room.East].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.East);
		});
		UIUtil.AddPointerUpListener(buttons[Room.West].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.West);
		});
		UIUtil.AddPointerUpListener(buttons[Room.South].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.South);
		});

		touch_input.AddBlockCount();
		touch_input.onTouchDown += (Vector3 position) =>
		{
			touch_point = position;
			touch_finish = false;
		};
		touch_input.onTouchDrag += (Vector3 position) =>
		{
			if (true == touch_finish)
			{
				return;
			}

			float distance = Vector3.Distance(touch_point, position);
			if (0.01f > distance)
			{
				Debug.Log("not enough drag distance(" + distance + ")");
				return;
			}

			touch_finish = true;
			Vector3 delta = position - touch_point;

			if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				if (0.0f > delta.x)
				{
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.East);
				}
				else
				{
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.West);
				}
			}
			else
			{
				if (0.0f > delta.y)
				{
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.North);
				}
				else
				{
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Room.South);
				}
			}
			touch_point = Vector3.zero;
		};
		touch_input.ReleaseBlockCount();
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<int>(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Finish, OnMoveFinish);
		Util.EventSystem.Unsubscribe(EventID.Shop_Open, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Unsubscribe(EventID.Shop_Close, () => { touch_input.ReleaseBlockCount(); });
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Open);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Close);
	}

	public void Init(Room.Data room)
	{
		for (int i = 0; i < Room.Max; i++)
		{
			Room.Data nextRoom = room.nexts[i];
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
	private void OnMoveStart(int direction)
	{
		gameObject.SetActive(false);
		touch_input.AddBlockCount();
	}

	private void OnMoveFinish()
	{
		gameObject.SetActive(true);
		touch_input.ReleaseBlockCount();
	}
}
