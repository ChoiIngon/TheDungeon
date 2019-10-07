using System.Collections;
using UnityEngine;

public class DungeonBox : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private float time = 0.2f;
	private TouchInput touch_input;
	private BoxCollider2D box_collier;
	private Dungeon.Room room;
    // Start is called before the first frame update
    void Awake()
    {
		close = UIUtil.FindChild<SpriteRenderer>(transform, "Close");
		open = UIUtil.FindChild<SpriteRenderer>(transform, "Open");
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
		box_collier = GetComponent<BoxCollider2D>();
		
	}

	private void Start()
	{
		touch_input.onTouchUp += (Vector3 position) =>
		{
			Debug.Log("show box touch input");
			box_collier.enabled = false;
			StartCoroutine(Open());
		};
	}

	public void Show(Dungeon.Room room)
	{
		Debug.Log("show box");
		this.room = room;
		close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		box_collier.enabled = true;		
		gameObject.SetActive(true);
	}

	public IEnumerator Open()
	{
		AudioManager.Instance.Play(AudioManager.BOX_OPEN);
		yield return StartCoroutine(Util.UITween.Overlap(close, open, time));
		GameManager.Instance.player.inventory.Add(room.item);
		string text = room.item.meta.name + " 아이템을 획득 했습니다.";
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(text));
		gameObject.SetActive(false);
		room.item = null;
	}
}
