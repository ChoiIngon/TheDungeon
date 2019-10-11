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
			yield return GameManager.Instance.ui_textbox.TypeWrite("인벤토리가 가득 찼습니다.");
			yield break;
		}
		AudioManager.Instance.Play(AudioManager.BOX_OPEN);
		yield return Util.UITween.Overlap(close, open, time);
		GameManager.Instance.player.inventory.Add(room.item);
		string text = room.item.meta.name + " 아이템을 획득 했습니다.";
		yield return GameManager.Instance.ui_textbox.TypeWrite(text);
		gameObject.SetActive(false);
		room.item = null;
	}
}
