using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
	public const int North = 0;
	public const int East = 1;
	public const int South = 2;
	public const int West = 3;
	public const int Max = 4;

	public enum Type
	{
		Start,
		Exit,
		Lock,
		Shop,
		Normal
	}

	public class Data
	{
		public int id = 0;
		public int group = 0;
		public bool visit = false;
		public Type type = Type.Normal;

		public Data[] nexts = new Data[Max];

		public Item.Meta item = null;
		public float item_chance = 0.0f;
		public Monster.Meta monster = null;
		public string npc_sprite_path = "";

		public Data GetNext(int direction)
		{
			if (0 > direction || Max <= direction)
			{
				throw new System.Exception("room id:" + direction + ", invalid direction:" + direction);
			}
			return nexts[direction];
		}
	}

	public GameObject[] nexts;
	public RoomStair stair;
	public TreasureBox treasure_box;
	public SpriteRenderer npc;
	public int sorting_order;
	private void Awake()
	{
		nexts = new GameObject[Max];
		nexts[North] = UIUtil.FindChild<Transform>(transform, "NorthDoor").gameObject;
		nexts[East] = UIUtil.FindChild<Transform>(transform, "EastDoor").gameObject;
		nexts[West] = UIUtil.FindChild<Transform>(transform, "WestDoor").gameObject;

		stair = UIUtil.FindChild<RoomStair>(transform, "Stair");
		stair.gameObject.SetActive(true);

		treasure_box = UIUtil.FindChild<TreasureBox>(transform, "Box");
		treasure_box.gameObject.SetActive(true);
		npc = UIUtil.FindChild<SpriteRenderer>(transform, "Npc");
		npc.gameObject.SetActive(true);
	}

	private void Start()
	{
		stair.gameObject.SetActive(false);
		treasure_box.gameObject.SetActive(false);
		npc.gameObject.SetActive(false);

		SpriteRenderer roomSprite = GetComponent<SpriteRenderer>();

		SpriteRenderer northDoorSprite = UIUtil.FindChild<SpriteRenderer>(transform, "NorthDoor");
		SpriteRenderer eastDoorSprite = UIUtil.FindChild<SpriteRenderer>(transform, "EastDoor");
		SpriteRenderer westDoorSprite = UIUtil.FindChild<SpriteRenderer>(transform, "WestDoor");

		SpriteRenderer boxLockIcon = UIUtil.FindChild<SpriteRenderer>(transform, "Box/LockIcon");
		SpriteRenderer boxOpen = UIUtil.FindChild<SpriteRenderer>(transform, "Box/Open");
		SpriteRenderer boxClose = UIUtil.FindChild<SpriteRenderer>(transform, "Box/Close");
		SpriteRenderer boxShadow = UIUtil.FindChild<SpriteRenderer>(transform, "Box/Shadow");

		SpriteRenderer stairLockIcon = UIUtil.FindChild<SpriteRenderer>(transform, "Stair/LockIcon");
		SpriteRenderer stairOpen = UIUtil.FindChild<SpriteRenderer>(transform, "Stair/Open");
		SpriteRenderer stairClose = UIUtil.FindChild<SpriteRenderer>(transform, "Stair/Close");

		roomSprite.sortingOrder = sorting_order;
		northDoorSprite.sortingOrder = sorting_order + 1;
		eastDoorSprite.sortingOrder = sorting_order + 1;
		westDoorSprite.sortingOrder = sorting_order + 1;

		boxLockIcon.sortingOrder = sorting_order + 2;
		boxShadow.sortingOrder = sorting_order + 2;
		boxOpen.sortingOrder = sorting_order + 3;
		boxClose.sortingOrder = sorting_order + 3;

		stairLockIcon.sortingOrder = sorting_order + 2;
		stairOpen.sortingOrder = sorting_order + 3;
		stairClose.sortingOrder = sorting_order + 3;

		npc.sortingOrder = sorting_order + 2;
	}

	public void Init(Data data)
	{
		stair.gameObject.SetActive(false);
		treasure_box.gameObject.SetActive(false);
		npc.gameObject.SetActive(false);

		if (null == data)
		{
			return;	
		}

		for (int i = 0; i < nexts.Length; i++)
		{
			if (null != nexts[i])
			{
				nexts[i].SetActive((bool)(null != data.nexts[i]));
			}
		}		   		

		if (Type.Exit == data.type || Type.Lock == data.type)
		{
			stair.Show(data);
		}

		if ("" != data.npc_sprite_path)
		{
			npc.gameObject.SetActive(true);
			npc.sprite = ResourceManager.Instance.Load<Sprite>(data.npc_sprite_path);
		}
	}
}