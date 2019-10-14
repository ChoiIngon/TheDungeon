using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneDungeon : SceneMain
{
	private const float ROOM_SIZE = 7.2f; // walkDistance;
	private const float ROOM_MOVE_SPEED = 17.0f;

	public UIMiniMap mini_map;
	
	private Transform rooms;
	private Room current_room;
	private readonly Room[] next_rooms = new Room[Dungeon.Max];

	public DungeonBattle battle;

	public Button button_inventory;
	public Text text_inventory;
	private Transform ui_player_transform;
	public UIGaugeBar player_health;
	public UIGaugeBar player_exp;

	private Dungeon dungeon;	
    private Text ui_dungeon_level;

    private Transform coin_spot;
    private Coin coin_prefab;
	private DungeonMoveButtons move_buttons;
	
    //private List<QuestData> completeQuests;
	
	public override IEnumerator Run()
	{
		if ("Dungeon" == SceneManager.GetActiveScene().name)
		{
			AsyncOperation operation = SceneManager.LoadSceneAsync("Common", LoadSceneMode.Additive);
			while (false == operation.isDone)
			{
				// loading progress
				yield return null;
			}
			yield return StartCoroutine(GameManager.Instance.Init());
		}

		dungeon = new Dungeon();
		battle = UIUtil.FindChild<DungeonBattle>(transform, "Battle");
		battle.gameObject.SetActive(true);
		rooms = UIUtil.FindChild<Transform>(transform, "Rooms");
		current_room = UIUtil.FindChild<Room>(rooms, "Current");
		current_room.Init(null);
		next_rooms[Dungeon.North] = UIUtil.FindChild<Room>(rooms, "North");
		next_rooms[Dungeon.North].transform.position = new Vector3(0.0f, 0.0f, ROOM_SIZE);
		next_rooms[Dungeon.North].Init(null);
		next_rooms[Dungeon.East] = UIUtil.FindChild<Room>(rooms, "East");
		next_rooms[Dungeon.East].transform.position = new Vector3(ROOM_SIZE, 0.0f, 0.0f);
		next_rooms[Dungeon.East].Init(null);
		next_rooms[Dungeon.South] = UIUtil.FindChild<Room>(rooms, "South");
		next_rooms[Dungeon.South].transform.position = new Vector3(0.0f, 0.0f, -ROOM_SIZE);
		next_rooms[Dungeon.South].Init(null);
		next_rooms[Dungeon.West] = UIUtil.FindChild<Room>(rooms, "West");
		next_rooms[Dungeon.West].transform.position = new Vector3(-ROOM_SIZE, 0.0f, 0.0f);
		next_rooms[Dungeon.West].Init(null);

		move_buttons = UIUtil.FindChild<DungeonMoveButtons>(transform, "UI/MoveButtons");
		
		button_inventory = UIUtil.FindChild<Button>(transform, "UI/ButtonInventory");
		UIUtil.AddPointerUpListener(button_inventory.gameObject, () =>
		{
			GameManager.Instance.ui_inventory.SetActive(true);
		});
		text_inventory = UIUtil.FindChild<Text>(transform, "UI/ButtonInventory/Text");
		
		mini_map = UIUtil.FindChild<UIMiniMap>(transform,		"UI/Dungeon/MiniMap");
		ui_dungeon_level = UIUtil.FindChild<Text>(transform,	"UI/Dungeon/Level");

		ui_player_transform = UIUtil.FindChild<Transform>(transform, "UI/Player");
		player_health = UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Health");
		player_exp =	UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Exp");

		coin_spot = UIUtil.FindChild<Transform>(transform, "CoinSpot");
		coin_spot.gameObject.SetActive(true);
		coin_prefab = UIUtil.FindChild<Coin>(transform, "Prefabs/Coin");
		
		Util.EventSystem.Subscribe<int>(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Subscribe(EventID.Dungeon_Exit_Unlock, () => { StartCoroutine(OnExitUnlock()); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Map_Reveal, () => { mini_map.RevealMap(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Monster_Reveal, () => { mini_map.RevealMonster(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Treasure_Reveal, () => { mini_map.RevealTreasure(); });
		Util.EventSystem.Subscribe(EventID.Player_Stat_Change, OnChangePlayerHealth);
		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Subscribe<Item>(EventID.Inventory_Remove, OnItemRemove);
		
		AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);
		InitScene();
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe<int>(EventID.Dungeon_Move_Start, OnMoveStart);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Exit_Unlock);
		Util.EventSystem.Unsubscribe(EventID.Player_Stat_Change, OnChangePlayerHealth);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Map_Reveal);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Monster_Reveal);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Treasure_Reveal);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Add, OnItemAdd);
		Util.EventSystem.Unsubscribe<Item>(EventID.Inventory_Remove, OnItemRemove);

	}

	private void InitScene()
	{
		GameManager.Instance.ui_inventory.Clear();
		GameManager.Instance.player.Init();

		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
		player_exp.max = GameManager.Instance.player.GetMaxExp();
		player_exp.current = GameManager.Instance.player.exp;
		text_inventory.text = GameManager.Instance.player.inventory.count.ToString() + "/" + Inventory.MAX_SLOT_COUNT;

		dungeon.level = 0;
		InitDungeon();
	}

	private void InitDungeon()
	{
		dungeon.Init(++dungeon.level);

		StartCoroutine(GameManager.Instance.CameraFade(Color.black, new Color(0.0f, 0.0f, 0.0f, 0.0f), 1.5f));
		ui_dungeon_level.text = "<size=" + (ui_dungeon_level.fontSize * 0.8f) + ">B</size> " + dungeon.level.ToString();
		mini_map.Init(dungeon);
		InitRooms();
		StartCoroutine(GameManager.Instance.ui_textbox.TypeWrite(GameText.GetText("DUNGEON/WELCOME", dungeon.level)));
	}

	private void InitRooms()
	{
		rooms.transform.position = Vector3.zero;
		for(int i=0; i<Dungeon.Max; i++)
		{
			Dungeon.Room room = dungeon.current_room.next [i];
			if (null != room)
			{
				next_rooms[i].Init (room);
			}
		}
		mini_map.CurrentPosition (dungeon.current_room.id);
		current_room.Init(dungeon.current_room);
		move_buttons.Init(dungeon.current_room);
	}

	private IEnumerator Move(int direction)
	{
		while (0 < coin_spot.childCount)
		{
			yield return null;
		}

		Dungeon.Room next_room = dungeon.current_room.GetNext(direction);

		if (null == next_room)
		{
			switch (direction)
			{
				case Dungeon.North:
					iTween.PunchPosition(rooms.gameObject, new Vector3(0.0f, 0.0f, 1.0f), 0.5f);
					break;
				case Dungeon.East:
					iTween.PunchPosition(rooms.gameObject, new Vector3(1.0f, 0.0f, 0.0f), 0.5f);
					break;
				case Dungeon.South:
					iTween.PunchPosition(rooms.gameObject, new Vector3(0.0f, 0.0f, -1.0f), 0.5f);
					break;
				case Dungeon.West:
					iTween.PunchPosition(rooms.gameObject, new Vector3(-1.0f, 0.0f, 0.5f), 0.5f);
					break;
				default:
					break;
			}
			Util.EventSystem.Publish(EventID.Dungeon_Move_Finish);
			yield break;
		}

		Vector3 position = Vector3.zero;
		switch (direction)
		{
			case Dungeon.North:
				position = new Vector3(rooms.position.x, rooms.position.y, rooms.position.z - ROOM_SIZE);
				break;
			case Dungeon.East:
				position = new Vector3(rooms.position.x - ROOM_SIZE, rooms.position.y, rooms.position.z);
				break;
			case Dungeon.South:
				position = new Vector3(rooms.position.x, rooms.position.y, rooms.position.z + ROOM_SIZE);
				break;
			case Dungeon.West:
				position = new Vector3(rooms.position.x + ROOM_SIZE, rooms.position.y, rooms.position.z);
				break;
			default:
				break;
		}

		dungeon.Move(direction);
		AudioManager.Instance.Play(AudioManager.DUNGEON_WALK, true);
		yield return MoveTo(rooms.gameObject, iTween.Hash("position", position, "time", ROOM_SIZE / ROOM_MOVE_SPEED, "easetype", iTween.EaseType.linear), true);
		AudioManager.Instance.Stop(AudioManager.DUNGEON_WALK);

		InitRooms();

		yield return OnExitUnlock();

		if (null != dungeon.current_room.item)
		{
			bool yes = false;
			GameManager.Instance.ui_textbox.on_submit += () => {
				yes = true;
				GameManager.Instance.ui_textbox.Close();
			};
			yield return GameManager.Instance.ui_textbox.TypeWrite("Do you want to open box?");
			if (true == yes)
			{
				yield return current_room.box.Open();
				mini_map.CurrentPosition(dungeon.current_room.id);
			}
		}

		if (Dungeon.Room.Type.Exit == dungeon.current_room.type || Dungeon.Room.Type.Lock == dungeon.current_room.type)
		{
			StartCoroutine(mini_map.Hide(0.5f, 0.3f));
		}
		else 
		{
			StartCoroutine(mini_map.Show(0.5f));
		}

		if (null != dungeon.current_room.monster)
		{
			AudioManager.Instance.Stop(AudioManager.DUNGEON_BGM);
			AudioManager.Instance.Play(AudioManager.BATTLE_BGM, true);
			
			StartCoroutine(mini_map.Hide(0.5f));
			button_inventory.gameObject.SetActive(false);
			yield return StartCoroutine(battle.BattleStart(dungeon.current_room.monster));
			StartCoroutine(mini_map.Show(0.5f));
			button_inventory.gameObject.SetActive(true);

			AudioManager.Instance.Stop(AudioManager.BATTLE_BGM);
			AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);
			if (DungeonBattle.BattleResult.Win == battle.battle_result)
			{
				yield return Win(dungeon.current_room.monster);
				dungeon.current_room.monster = null;
				mini_map.CurrentPosition(dungeon.current_room.id);
			}
			else
			{
				yield return Lose();
			}
		}

		Util.EventSystem.Publish(EventID.Dungeon_Move_Finish);
	}

	private IEnumerator Win(Monster.Meta meta)
	{
		yield return GameManager.Instance.ui_textbox.LogWrite(GameText.GetText("DUNGEON/BATTLE/DEFEATED", "You", meta.name));

		int prevPlayerLevel = GameManager.Instance.player.level;
		Stat stat = GameManager.Instance.player.stats;
		int rewardCoin = meta.reward.coin + (int)Random.Range(-meta.reward.coin * 0.1f, meta.reward.coin * 0.1f);
		int bonusCoin = (int)(rewardCoin * stat.GetStat(StatType.CoinBonus) / 100.0f);
		int rewardExp = meta.reward.exp + (int)Random.Range(-meta.reward.exp * 0.1f, meta.reward.exp * 0.1f);
		int bonusExp = (int)(rewardExp * stat.GetStat(StatType.ExpBonus) / 100.0f);

		Item item = null;
		if (meta.reward.item_chance >= Random.Range(0, 100))
		{
			item = ItemManager.Instance.CreateRandomEquipItem();
			GameManager.Instance.player.inventory.Add(item);
		}
		GameManager.Instance.player.coin += rewardCoin + bonusCoin;
		GameManager.Instance.player.AddExp(rewardExp + bonusExp);
		ProgressManager.Instance.Update(Achieve.AchieveType_CollectCoin, "", rewardCoin + bonusCoin);
		ProgressManager.Instance.Update(Achieve.AchieveType_PlayerLevel, "", GameManager.Instance.player.level);

		CreateCoins(rewardCoin + bonusCoin);
		for (int i = prevPlayerLevel; i < GameManager.Instance.player.level; i++)
		{
			yield return player_exp.SetCurrent(player_exp.max, 0.3f);
			player_exp.max = GameManager.Instance.player.GetMaxExp(i);
			yield return player_exp.SetCurrent(0.0f, 0.0f);
		}
		
		player_exp.max = GameManager.Instance.player.GetMaxExp();
		player_exp.current = GameManager.Instance.player.exp;
		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;

		yield return GameManager.Instance.ui_textbox.LogWrite("Coins : +" + rewardCoin + (0 < bonusCoin ? "(" + bonusCoin + " bonus)" : ""));
		yield return GameManager.Instance.ui_textbox.LogWrite("Exp : +" + rewardExp + (0 < bonusExp ? "(" + bonusExp + " bonus)" : ""));
		if (prevPlayerLevel < GameManager.Instance.player.level)
		{
			yield return GameManager.Instance.ui_textbox.LogWrite("Level : " + prevPlayerLevel + " -> " + GameManager.Instance.player.level);
		}
		if (null != item)
		{
			yield return GameManager.Instance.ui_textbox.LogWrite(GameText.GetText("DUNGEON/HAVE_ITEM", "You", "<color=#" + ColorUtility.ToHtmlStringRGBA(UIItem.GetGradeColor(item.grade)) +">" + item.meta.name + "</color>"));
		}

		GameManager.Instance.ui_textbox.ActiveCloseButton();
		/*
		QuestManager.Instance.Update("KillMonster", info.id);
		yield return StartCoroutine(CheckCompleteQuest());
		*/
	}

	private IEnumerator Lose()
	{
		ProgressManager.Instance.Update(Achieve.AchieveType_DieCount, "", 1);

		Rect prev = GameManager.Instance.ui_textbox.Resize(1000 /*Screen.currentResolution.height * GameManager.Instance.canvas.scaleFactor * 0.8f*/);
		yield return StartCoroutine(GameManager.Instance.ui_textbox.TypeWrite(
			"You died.\n" +
			"Your body will be carried to village.\n" +
			"See you soon..\n" +
			"Kill monster count : 100\n" +
			"item count : 100\n" +
			"turn : 100\n"
		));
		GameManager.Instance.ui_textbox.Resize(prev.height);
		yield return GameManager.Instance.CameraFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.black, 1.5f);

		StartCoroutine(GameManager.Instance.advertisement.ShowAds());
		//InitScene();

		yield return AsyncLoadScene("Start");
		yield return AsyncUnloadScene("Dungeon");
	}

	private IEnumerator GoDown()
	{
		Vector3 position = Camera.main.transform.position;
		StartCoroutine(GameManager.Instance.CameraFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.black, 1.5f));
		yield return StartCoroutine(MoveTo(Camera.main.gameObject, iTween.Hash(
			"x", current_room.stair.transform.position.x,
			"y", current_room.stair.transform.position.y,
			"z", current_room.stair.transform.position.z - 0.5f,
			"time", 1.0f
		), true));
		InitDungeon();
		Camera.main.transform.position = position;
	}

	private IEnumerator OnExitUnlock()
	{
		if (Dungeon.Room.Type.Exit == dungeon.current_room.type)
		{
			move_buttons.touch_input.AddBlockCount();
			bool yes = false;
			GameManager.Instance.ui_textbox.on_submit += () => {
				yes = true;
				GameManager.Instance.ui_textbox.Close();
			};
			yield return GameManager.Instance.ui_textbox.TypeWrite("Do you want to go down the stair?");
			if (true == yes)
			{
				yield return StartCoroutine(GoDown());
			}
			move_buttons.touch_input.ReleaseBlockCount();
		}
	}

	private void OnChangePlayerHealth()
	{
		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
	}
	
	private void CreateCoins(int amount)
	{
		int total = amount;
		int multiply = 1;
		float scale = 1.0f;

		while (0 < total)
		{
			int countCount = Random.Range(5, 10);
			for (int i = 0; i < countCount; i++)
			{
				Coin coin = GameObject.Instantiate<Coin>(coin_prefab);
				coin.amount = Mathf.Min(total, multiply);
				coin.transform.SetParent(coin_spot, false);
				coin.transform.localScale = new Vector3(scale, scale, 1.0f);
				coin.transform.localPosition = Vector3.zero;
				coin.gameObject.SetActive(true);
				iTween.MoveBy(coin.gameObject, new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(0.0f, 0.5f), 0.0f), 0.5f);

				total -= coin.amount;
				if (0 >= total)
				{
					return;
				}
			}
			multiply *= 10;
			scale += 0.1f;
		}
	}

	private void OnMoveStart(int direction)
	{
		StartCoroutine(Move(direction));
	}

	private void OnItemAdd(Item item)
	{
		string text = GameManager.Instance.player.inventory.count.ToString() + "/" + Inventory.MAX_SLOT_COUNT.ToString();

		if (GameManager.Instance.player.inventory.count == Inventory.MAX_SLOT_COUNT)
		{
			text_inventory.text = "<color=red>" + text + "</color>";
		}
		else
		{
			text_inventory.text = text;
		}
	}

	private void OnItemRemove(Item item)
	{
		text_inventory.text = GameManager.Instance.player.inventory.count.ToString() + "/" + Inventory.MAX_SLOT_COUNT.ToString();
	}
}
