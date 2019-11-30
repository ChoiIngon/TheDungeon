using System.Collections;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
	private SpriteRenderer close;
	private SpriteRenderer open;
	private float time = 0.2f;
	private Room.Data room;

	private SpriteRenderer lock_icon;
	private TouchInput touch_input;

	void Awake()
    {
		close = UIUtil.FindChild<SpriteRenderer>(transform, "Close");
		open = UIUtil.FindChild<SpriteRenderer>(transform, "Open");

		lock_icon = UIUtil.FindChild<SpriteRenderer>(transform, "LockIcon");
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new MissingComponentException("TouchInput");
		}
		
		touch_input.on_touch_up += (Vector3 position) =>
		{
			StartCoroutine(Open());
		};

		Util.EventSystem.Subscribe(EventID.Shop_Open, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe(EventID.Shop_Close, () => { touch_input.block_count--; });
		Util.EventSystem.Subscribe(EventID.Inventory_Open, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe(EventID.Inventory_Close, () => { touch_input.block_count--; });
		Util.EventSystem.Subscribe(EventID.TextBox_Open, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe(EventID.TextBox_Close, () => { touch_input.block_count--; });
		Util.EventSystem.Subscribe(EventID.NPC_Dialogue_Start, () => { touch_input.block_count++; });
		Util.EventSystem.Subscribe(EventID.NPC_Dialogue_Finish, () => { touch_input.block_count--; });
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Shop_Open);
		Util.EventSystem.Unsubscribe(EventID.Shop_Close);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Open);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Close);
		Util.EventSystem.Unsubscribe(EventID.NPC_Dialogue_Start);
		Util.EventSystem.Unsubscribe(EventID.NPC_Dialogue_Finish);
	}

	private void OnEnable()
	{
		touch_input.touch_collider.enabled = true;
	}

	private void OnDisable()
	{
	}

	public void Show(Room.Data room)
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
			yield return GameManager.Instance.ui_textbox.Write(GameText.GetText("ERROR/INVENTORY_FULL"), false);
			yield break;
		}

		touch_input.touch_collider.enabled = false;
		lock_icon.gameObject.SetActive(false);
		AudioManager.Instance.Play(AudioManager.BOX_OPEN);
		StartCoroutine(Util.UITween.Overlap(close, open, time));

		Item item = room.item.CreateInstance();
		GameManager.Instance.player.inventory.Add(item);
		yield return GameManager.Instance.ui_textbox.Write(GameText.GetText("DUNGEON/HAVE_ITEM", "You", "<color=#" + ColorUtility.ToHtmlStringRGB(UIItem.GetGradeColor(item.grade)) + ">" + room.item.name + "</color>"), false);
		ProgressManager.Instance.Update(ProgressType.CollectItem, "", 1);
		room.item = null;
		gameObject.SetActive(false);
	}
}
