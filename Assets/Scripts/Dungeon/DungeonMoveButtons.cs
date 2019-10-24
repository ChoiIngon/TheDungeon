using UnityEngine;
using UnityEngine.UI;

public class DungeonMoveButtons : MonoBehaviour
{
	public TouchInput touch_input;
	private Vector3 touch_point = Vector3.zero;
	private bool touch_finish = false;

	private Button[] buttons = new Button[Dungeon.Max];
	private void Awake()
	{
		touch_input = UIUtil.FindChild<TouchInput>(transform, "../../");
		buttons[Dungeon.North] = UIUtil.FindChild<Button>(transform, "North");
		buttons[Dungeon.East] = UIUtil.FindChild<Button>(transform, "East");
		buttons[Dungeon.West] = UIUtil.FindChild<Button>(transform, "West");
		buttons[Dungeon.South] = UIUtil.FindChild<Button>(transform, "South");

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
		UIUtil.AddPointerUpListener(buttons[Dungeon.North].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.North);
		});
		UIUtil.AddPointerUpListener(buttons[Dungeon.East].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.East);
		});
		UIUtil.AddPointerUpListener(buttons[Dungeon.West].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.West);
		});
		UIUtil.AddPointerUpListener(buttons[Dungeon.South].gameObject, () =>
		{
			Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.South);
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
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.East);
				}
				else
				{
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.West);
				}
			}
			else
			{
				if (0.0f > delta.y)
				{
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.North);
				}
				else
				{
					Util.EventSystem.Publish<int>(EventID.Dungeon_Move_Start, Dungeon.South);
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
