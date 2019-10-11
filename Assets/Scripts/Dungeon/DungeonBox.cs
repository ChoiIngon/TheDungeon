using System.Collections;
using UnityEngine;

public class DungeonBox : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private float time = 0.2f;
	//private TouchInput touch_input;
	//private BoxCollider2D touch_collider;
	private Dungeon.Room room;
    // Start is called before the first frame update
    void Awake()
    {
		close = UIUtil.FindChild<SpriteRenderer>(transform, "Close");
		open = UIUtil.FindChild<SpriteRenderer>(transform, "Open");
		/*
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
		touch_collider = GetComponent<BoxCollider2D>();
		if (null == touch_collider)
		{
			throw new MissingComponentException("BoxCollider2D");
		}
		*/
	}

	private void Start()
	{
		/*
		touch_input.onTouchUp += (Vector3 position) =>
		{
			Debug.Log("show box touch input");
			touch_collider.enabled = false;
			StartCoroutine(Open());
		};
		*/
	}

	public void Show(Dungeon.Room room)
	{
		this.room = room;
		close.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		open.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		//touch_collider.enabled = true;		
		gameObject.SetActive(true);
	}

	public IEnumerator Open()
	{
		AudioManager.Instance.Play(AudioManager.BOX_OPEN);
		yield return Util.UITween.Overlap(close, open, time);
		GameManager.Instance.player.inventory.Add(room.item);
		string text = room.item.meta.name + " 아이템을 획득 했습니다.";
		yield return GameManager.Instance.ui_textbox.TypeWrite(text);
		gameObject.SetActive(false);
		room.item = null;
	}
}
