using System.Collections;
using UnityEngine;

public class DungeonBox : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private float time = 0.2f;
	private Dungeon.Room room;

    void Awake()
    {
		close = UIUtil.FindChild<SpriteRenderer>(transform, "Close");
		open = UIUtil.FindChild<SpriteRenderer>(transform, "Open");
	}

	private void Start()
	{
	}

	public void Show(Dungeon.Room room)
	{
		this.room = room;
		close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		gameObject.SetActive(true);
	}

	public IEnumerator Open()
	{
		if (Inventory.MAX_SLOT_COUNT <= GameManager.Instance.player.inventory.count)
		{
			yield return GameManager.Instance.ui_textbox.TypeWrite(GameText.GetText("ERROR/INVENTORY_FULL"));
			yield break;
		}
		AudioManager.Instance.Play(AudioManager.BOX_OPEN);
		yield return Util.UITween.Overlap(close, open, time);
		GameManager.Instance.player.inventory.Add(room.item);
		yield return GameManager.Instance.ui_textbox.TypeWrite(GameText.GetText("DUNGEON/HAVE_ITEM", "You", room.item.meta.name));
		gameObject.SetActive(false);
		room.item = null;
	}
}
