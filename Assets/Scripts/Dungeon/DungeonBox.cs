using System.Collections;
using UnityEngine;

public class DungeonBox : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private float time = 0.2f;
	private Dungeon.Room room;

	private SpriteRenderer lock_icon;
	private TouchInput touch_input;

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
		
		touch_input.onTouchUp += (Vector3 position) =>
		{
			StartCoroutine(Open());
		};
	}

	private void OnEnable()
	{
		Util.EventSystem.Publish<float>(EventID.MiniMap_Hide, 0.0f);
	}

	private void OnDisable()
	{
		Util.EventSystem.Publish(EventID.MiniMap_Show);
	}

	public void Show(Dungeon.Room room)
	{
		this.room = room;
		close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		lock_icon.gameObject.SetActive(true);
		gameObject.SetActive(true);
	}

	public IEnumerator Open()
	{
		if (Inventory.MAX_SLOT_COUNT <= GameManager.Instance.player.inventory.count)
		{
			yield return GameManager.Instance.ui_textbox.Write(GameText.GetText("ERROR/INVENTORY_FULL"));
			yield break;
		}
		lock_icon.gameObject.SetActive(false);
		AudioManager.Instance.Play(AudioManager.BOX_OPEN);
		yield return Util.UITween.Overlap(close, open, time);
		GameManager.Instance.player.inventory.Add(room.item.CreateInstance());
		ProgressManager.Instance.Update(ProgressType.CollectItem, "", 1);
		yield return GameManager.Instance.ui_textbox.Write(GameText.GetText("DUNGEON/HAVE_ITEM", "You", room.item.name));
		gameObject.SetActive(false);
		room.item = null;
	}
}
