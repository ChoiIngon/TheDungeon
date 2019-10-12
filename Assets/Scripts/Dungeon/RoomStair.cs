using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStair : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private SpriteRenderer lock_icon;
	private float time = 1.0f;
	private TouchInput touch_input;
	private BoxCollider2D touch_collider;
	private Dungeon.Room room;
	private Coroutine textbox_coroutine;
	// Start is called before the first frame update
	void Awake()
	{
		close = UIUtil.FindChild<SpriteRenderer>(transform, "Close");
		open = UIUtil.FindChild<SpriteRenderer>(transform, "Open");
		lock_icon = UIUtil.FindChild<SpriteRenderer>(transform, "LockIcon");
		touch_input = lock_icon.GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
		touch_collider = lock_icon.GetComponent<BoxCollider2D>();
		if (null == touch_collider)
		{
			throw new MissingComponentException("BoxCollider2D");
		}

		
	}

	private void Start()
	{
		touch_input.onTouchUp += (Vector3 position) =>
		{
			touch_collider.enabled = false;
			if (Dungeon.Room.Type.Lock == room.type)
			{
				List<KeyItem> keyItems = GameManager.Instance.player.inventory.GetItems<KeyItem>();
				if (0 == keyItems.Count)
				{
					if (null != textbox_coroutine)
					{
						StopCoroutine(textbox_coroutine);
					}
					textbox_coroutine = StartCoroutine(GameManager.Instance.ui_textbox.TypeWrite(GameText.GetText("ERROR/UNLOCK_DOOR")));
					return;
				}
				GameManager.Instance.player.inventory.Remove(keyItems[0].slot_index);
			}
			StartCoroutine(Open());
		};
	}

	public void Show(Dungeon.Room room)
	{
		this.room = room;
		if (Dungeon.Room.Type.Lock == room.type)
		{
			close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			lock_icon.gameObject.SetActive(true);
		}
		else if(Dungeon.Room.Type.Exit == room.type)
		{
			close.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			open.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			lock_icon.gameObject.SetActive(false);
		}
		touch_collider.enabled = true;
		gameObject.SetActive(true);
	}

	private IEnumerator Open()
	{
		AudioManager.Instance.Play(AudioManager.DOOR_OPEN);
		room.type = Dungeon.Room.Type.Exit;
		lock_icon.gameObject.SetActive(false);
		yield return StartCoroutine(Util.UITween.Overlap(close, open, time));
		Util.EventSystem.Publish(EventID.Dungeon_Exit_Unlock);
	}
}
