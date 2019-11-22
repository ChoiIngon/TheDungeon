using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStair : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private SpriteRenderer lock_icon;
	
	private TouchInput touch_input;
	//private BoxCollider2D touch_collider;
	private Room.Data room;
	private Coroutine textbox_coroutine;
	
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
		/*
		touch_collider = lock_icon.GetComponent<BoxCollider2D>();
		if (null == touch_collider)
		{
			throw new MissingComponentException("BoxCollider2D");
		}
		*/
	}

	private void Start()
	{
		touch_input.on_touch_up += (Vector3 position) =>
		{
			touch_input.gameObject.SetActive(false);
			//touch_collider.enabled = false;
			if (Room.Type.Lock == room.type)
			{
				List<KeyItem> keyItems = GameManager.Instance.player.inventory.GetItems<KeyItem>();
				if (0 == keyItems.Count)
				{
					if (null != textbox_coroutine)
					{
						StopCoroutine(textbox_coroutine);
					}
					textbox_coroutine = StartCoroutine(GameManager.Instance.ui_textbox.Write(GameText.GetText("ERROR/UNLOCK_DOOR"), false));
					touch_input.gameObject.SetActive(true);
					return;
				}
				GameManager.Instance.player.inventory.Remove(keyItems[0].slot_index);
			}
			StartCoroutine(Open());
		};
	}

	public void Show(Room.Data room)
	{
		this.room = room;
		if (Room.Type.Lock == room.type)
		{
			close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			lock_icon.gameObject.SetActive(true);
		}
		else if(Room.Type.Exit == room.type)
		{
			close.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
			open.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			lock_icon.gameObject.SetActive(false);
		}
		//touch_collider.enabled = true;
		gameObject.SetActive(true);
	}

	private IEnumerator Open()
	{
		AudioManager.Instance.Play(AudioManager.DOOR_OPEN);
		room.type = Room.Type.Exit;
		lock_icon.gameObject.SetActive(false);
		yield return StartCoroutine(Util.UITween.Overlap(close, open, 1.0f));
		Util.EventSystem.Publish(EventID.Dungeon_Exit_Unlock);
	}
}
