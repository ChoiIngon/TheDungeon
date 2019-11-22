using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMove : MonoBehaviour
{
	private float DRAG_MIN_THRESHOLD = 0.1f;
	private Dungeon dungeon;
	private Vector3 touch_point = Vector3.zero;
	private int move_direction = 0;

	public TouchInput touch_input;

	private void Awake()
	{
		dungeon = GetComponent<Dungeon>();
		if (null == dungeon)
		{
			throw new MissingComponentException(typeof(Dungeon).Name);
		}

		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException(typeof(TouchInput).Name);
		}

		Util.EventSystem.Subscribe(EventID.Dungeon_Battle_Start, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe<DungeonBattle.BattleResult>(EventID.Dungeon_Battle_Finish, (DungeonBattle.BattleResult result) => { touch_input.block_count--; });
		Util.EventSystem.Subscribe(EventID.Shop_Open, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe(EventID.Shop_Close, () => { touch_input.block_count--; });
		Util.EventSystem.Subscribe(EventID.Inventory_Open, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe(EventID.Inventory_Close, () => { touch_input.block_count--; });
		Util.EventSystem.Subscribe(EventID.TextBox_Open, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe(EventID.TextBox_Close, () => { touch_input.block_count--; });
		Util.EventSystem.Subscribe(EventID.NPC_Dialogue_Start, () => { touch_input.block_count++;  });
		Util.EventSystem.Subscribe(EventID.NPC_Dialogue_Finish, () => { touch_input.block_count--; });
	}

	void Start()
    {
		touch_input.block_count++;
		touch_input.on_touch_down += (Vector3 position) =>
		{
			move_direction = -1;
			touch_point = position;
		};
		touch_input.on_touch_drag += (Vector3 position) =>
		{
			float distance = Vector3.Distance(touch_point, position);
			if (DRAG_MIN_THRESHOLD > distance || Dungeon.ROOM_SIZE * 0.49f < distance)
			{
				return;
			}

			Vector3 delta = position - touch_point;

			if (0 > move_direction)
			{
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					transform.position = new Vector3(delta.x, transform.position.y, transform.position.z);
					if (0.0f > delta.x)
					{
						move_direction = Room.East;
					}
					else
					{
						move_direction = Room.West;
					}
				}
				else
				{
					transform.position = new Vector3(transform.position.x, transform.position.y, delta.y);
					if (0.0f > delta.y)
					{
						move_direction = Room.North;
					}
					else
					{
						move_direction = Room.South;
					}
				}
			}
			else
			{
				if (Room.East == move_direction || Room.West == move_direction)
				{
					transform.position = new Vector3(delta.x, transform.position.y, transform.position.z);
				}
				else if (Room.North == move_direction || Room.South == move_direction)
				{
					transform.position = new Vector3(transform.position.x, transform.position.y, delta.y);
				}
			}
		};
		touch_input.on_touch_up += (Vector3 position) =>
		{
			if (0 > move_direction)
			{
				return;
			}
			StartCoroutine(Move(move_direction));
		};
		touch_input.block_count--;
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Battle_Start);
		Util.EventSystem.Unsubscribe<DungeonBattle.BattleResult>(EventID.Dungeon_Battle_Finish);
		Util.EventSystem.Unsubscribe(EventID.Shop_Open);
		Util.EventSystem.Unsubscribe(EventID.Shop_Close);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Open);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Close);
		Util.EventSystem.Unsubscribe(EventID.NPC_Dialogue_Start);
		Util.EventSystem.Unsubscribe(EventID.NPC_Dialogue_Finish);
	}

	public IEnumerator Move(int direction)
	{
		Room.Data nextRoom = dungeon.data.current_room.GetNext(direction);
		if (null == nextRoom)
		{
			switch (direction)
			{
				case Room.North:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(0.0f, 0.0f, 1.0f), 0.5f);
					break;
				case Room.East:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(1.0f, 0.0f, 0.0f), 0.5f);
					break;
				case Room.South:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(0.0f, 0.0f, -1.0f), 0.5f);
					break;
				case Room.West:
					iTween.PunchPosition(Camera.main.gameObject, new Vector3(-1.0f, 0.0f, 0.5f), 0.5f);
					break;
				default:
					break;
			}
			transform.position = Vector3.zero;
			yield break;
		}

		GameManager.Instance.player.move_count++;

		Vector3 position = Vector3.zero;
		float distance = 0.0f;
		switch (direction)
		{
			case Room.North:
				distance = Dungeon.ROOM_SIZE + transform.position.z;
				position = new Vector3(transform.position.x, transform.position.y, /*transform.position.z*/ -Dungeon.ROOM_SIZE);
				break;
			case Room.East:
				distance = Dungeon.ROOM_SIZE + transform.position.x;
				position = new Vector3(/*transform.position.x */ -Dungeon.ROOM_SIZE, transform.position.y, transform.position.z);
				break;
			case Room.South:
				distance = Dungeon.ROOM_SIZE - transform.position.z;
				position = new Vector3(transform.position.x, transform.position.y, /*transform.position.z + */Dungeon.ROOM_SIZE);
				break;
			case Room.West:
				distance = Dungeon.ROOM_SIZE - transform.position.x;
				position = new Vector3(/*transform.position.x + */Dungeon.ROOM_SIZE, transform.position.y, transform.position.z);
				break;
			default:
				break;
		}

		dungeon.data.Move(direction);
		AudioManager.Instance.Play(AudioManager.DUNGEON_WALK, true);
		move_complete = false;
		iTween.MoveTo(gameObject, iTween.Hash("position", position, "time", distance / Dungeon.ROOM_MOVE_SPEED, "easetype", iTween.EaseType.linear, "oncompletetarget", gameObject, "oncomplete", "OnMoveComplete"));
		while (false == move_complete)
		{
			yield return null;
		}
		AudioManager.Instance.Stop(AudioManager.DUNGEON_WALK);
		dungeon.InitRooms();
		Util.EventSystem.Publish<int>(EventID.Dungeon_Move, direction);
	}

	private bool move_complete = false;
	void OnMoveComplete()
	{
		move_complete = true;
	}
}
