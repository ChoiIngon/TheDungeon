using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneDungeon : SceneMain
{
	public UIButtonGroup main_buttons;
    public UIMiniMap mini_map;
	
	private float room_size = 7.2f; // walkDistance;
	private float room_move_speed = 17.0f;
	private Transform rooms;
	private Room current_room;
	private readonly Room[] next_rooms = new Room[Dungeon.Max];

	private TouchInput touch_input;
	private Vector3 touch_point = Vector3.zero;
	private bool touch_finish = false;

	public DungeonBattle battle;
	public DungeonBox box;

	private Transform ui_player_transform;
	public UIGaugeBar player_health;
	public UIGaugeBar player_exp;

	private Dungeon dungeon;
	public int dungeon_level = 1;
    public Text ui_dungeon_level;

    public Transform coin_spot;
    public Coin coin_prefab;

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
		box = UIUtil.FindChild<DungeonBox>(transform, "Box");
		box.gameObject.SetActive(true);
		rooms = UIUtil.FindChild<Transform>(transform, "Rooms");
		current_room = UIUtil.FindChild<Room>(rooms, "Current");
		next_rooms[Dungeon.North] = UIUtil.FindChild<Room>(rooms, "North");
		next_rooms[Dungeon.North].transform.position = new Vector3(0.0f, 0.0f, room_size);
		next_rooms[Dungeon.East] = UIUtil.FindChild<Room>(rooms, "East");
		next_rooms[Dungeon.East].transform.position = new Vector3(room_size, 0.0f, 0.0f);
		next_rooms[Dungeon.South] = UIUtil.FindChild<Room>(rooms, "South");
		next_rooms[Dungeon.South].transform.position = new Vector3(0.0f, 0.0f, -room_size);
		next_rooms[Dungeon.West] = UIUtil.FindChild<Room>(rooms, "West");
		next_rooms[Dungeon.West].transform.position = new Vector3(-room_size, 0.0f, 0.0f);

		mini_map = UIUtil.FindChild<UIMiniMap>(transform, "UI/Dungeon/MiniMap");
		ui_dungeon_level = UIUtil.FindChild<Text>(transform, "UI/Dungeon/Level");

		ui_player_transform = UIUtil.FindChild<Transform>(transform, "UI/Player");
		player_health = UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Health");
		player_exp =	UIUtil.FindChild<UIGaugeBar>(ui_player_transform, "Exp");
		main_buttons =	UIUtil.FindChild<UIButtonGroup>(ui_player_transform, "MainButtonGroup");
		
		main_buttons.Init();
		main_buttons.actions[0] += () => {
			GameManager.Instance.ui_inventory.SetActive(true);
		};
		main_buttons.buttons[1].gameObject.SetActive(false);
		main_buttons.buttons[2].gameObject.SetActive(false);
		main_buttons.buttons[3].gameObject.SetActive(false);
		
		coin_spot = UIUtil.FindChild<Transform>(transform, "CoinSpot");
		coin_spot.gameObject.SetActive(true);
		coin_prefab = UIUtil.FindChild<Coin>(transform, "Prefabs/Coin");
		
		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new System.Exception("can not find component 'TouchInput'");
		}
		touch_input.AddBlockCount();
		touch_input.onTouchDown += (Vector3 position) => 
		{
			touch_point = position;
			touch_finish = false;
		};
		touch_input.onTouchDrag += (Vector3 position) =>
		{
			if (true == touch_finish)
			{
				return;
			}

			float distance = Vector3.Distance(touch_point, position);

			if (0.01f > distance)
			{
				Debug.Log("not enough drag distance(" + distance + ")");
				return;
			}

			touch_finish = true;
			Vector3 delta = position - touch_point;

			if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				if (0.0f > delta.x)
				{
					StartCoroutine(Move(Dungeon.East));
				}
				else
				{
					StartCoroutine(Move(Dungeon.West));
				}
			}
			else
			{
				if (0.0f > delta.y)
				{
					StartCoroutine(Move(Dungeon.North));
				}
				else
				{
					StartCoroutine(Move(Dungeon.South));
				}
			}
			touch_point = Vector3.zero;
		};

		Util.EventSystem.Subscribe(EventID.Inventory_Open, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Inventory_Close, () => { touch_input.ReleaseBlockCount(); });

		Util.EventSystem.Subscribe(EventID.Dialog_Open, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Dialog_Close, () => { touch_input.ReleaseBlockCount(); });
		Util.EventSystem.Subscribe(EventID.TextBox_Open, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.TextBox_Close, () => { touch_input.ReleaseBlockCount(); });

		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Start, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Finish, () => { touch_input.ReleaseBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Exit_Unlock, () =>	{
			StartCoroutine(OnExitUnlock());
		});

		Util.EventSystem.Subscribe(EventID.Player_Change_Health, OnChangePlayerHealth);
		
		/*
        completeQuests = new List<QuestData> ();
		StartCoroutine (Init ());
		*/
		
		InitScene();

		AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);
		string[] scripts = new string[] {
			"오래전 이 던전엔 자신의 부와 젊음을 위해 백성들을 악마의 제물로 바쳤다는 피의 여왕이 살았다고 하네. ",
			"시간이 지나 이젠 전설이 되었지만 한가지 확실한건 저 곳엔 여왕이 남긴 엄청난 보물이 있다는 거야.",
			"하지만 지금까지 저곳으로 들어서 무사히 돌아나온 사람은 없다는군."
		};
		yield return StartCoroutine(GameManager.Instance.ui_npc.Talk("", scripts));
		touch_input.ReleaseBlockCount();
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Open);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Close);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Open);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Close);

		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Start);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Finish);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Exit_Unlock);
		Util.EventSystem.Unsubscribe(EventID.Player_Change_Health, OnChangePlayerHealth);
	}

	private void InitScene()
	{
		GameManager.Instance.ui_inventory.Clear();
		GameManager.Instance.player.Init();

		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
		player_exp.max = GameManager.Instance.player.GetMaxExp();
		player_exp.current = GameManager.Instance.player.exp;

		dungeon_level = 1;
		InitDungeon();

		/*
		yield return StartCoroutine(QuestManager.Instance.Init ());
		QuestManager.Instance.onComplete += (QuestData data) => {
			completeQuests.Add(data);
		};
		QuestManager.Instance.Update (QuestProgress.Type.CrrentLocation, "Dungeon");
		yield return StartCoroutine(CheckCompleteQuest ());
		*/
	}

	private void InitDungeon()
	{
		StartCoroutine(GameManager.Instance.CameraFade(Color.black, new Color(0.0f, 0.0f, 0.0f, 0.0f), 1.5f));
		ui_dungeon_level.text = "<size=" + (ui_dungeon_level.fontSize * 0.8f) + ">B</size> " + dungeon_level.ToString();
		dungeon.Init(dungeon_level);
		mini_map.Init(dungeon);
		InitRooms();
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
	}

	private IEnumerator Lose()
	{
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(
			"You died.\n" +
			"Your body will be carried to village.\n" +
			"See you soon.."
		));
		yield return StartCoroutine(GameManager.Instance.CameraFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.black, 1.5f));
				
		StartCoroutine(GameManager.Instance.ads.ShowAds());
		InitScene();
		//yield return StartCoroutine (CameraFadeTo (Color.black, iTween.Hash ("amount", 1.0f, "time", 1.0f), true));
		//SceneManager.LoadScene("Village");
	}

	private IEnumerator GoDown()
	{
		Vector3 position = Camera.main.transform.position;
		StartCoroutine(GameManager.Instance.CameraFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), Color.black, 1.5f));
		yield return StartCoroutine(MoveTo (Camera.main.gameObject, iTween.Hash(
			"x", current_room.stair.transform.position.x,
			"y", current_room.stair.transform.position.y,
			"z", current_room.stair.transform.position.z-0.5f,
			"time", 1.0f
		), true));
		dungeon_level += 1;
		Camera.main.transform.position = position;
	}

	private IEnumerator Move(int direction)
	{
		while (0 < coin_spot.childCount)
		{
			yield return null;
		}

		box.gameObject.SetActive(false);

		Util.EventSystem.Publish(EventID.Dungeon_Move_Start);
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
		iTween.EaseType easeType = iTween.EaseType.easeInOutExpo;
		switch (direction)
		{
			case Dungeon.North:
				easeType = iTween.EaseType.easeInQuart;
				position = new Vector3(rooms.position.x, rooms.position.y, rooms.position.z - room_size);
				break;
			case Dungeon.East:
				easeType = iTween.EaseType.easeInOutExpo;
				position = new Vector3(rooms.position.x - room_size, rooms.position.y, rooms.position.z);
				break;
			case Dungeon.South:
				easeType = iTween.EaseType.easeInQuart;
				position = new Vector3(rooms.position.x, rooms.position.y, rooms.position.z + room_size);
				break;
			case Dungeon.West:
				easeType = iTween.EaseType.easeInOutExpo;
				position = new Vector3(rooms.position.x + room_size, rooms.position.y, rooms.position.z);
				break;
			default:
				break;
		}

		dungeon.Move(direction);
		AudioManager.Instance.Play(AudioManager.DUNGEON_WALK, true);
		
		yield return MoveTo(rooms.gameObject, iTween.Hash("position", position, "time", room_size / room_move_speed, "easetype", easeType), true);
		AudioManager.Instance.Stop(AudioManager.DUNGEON_WALK);

		InitRooms();

		yield return OnExitUnlock();

		if (null != dungeon.current_room.item)
		{
			box.Show(dungeon.current_room);
		}

		if (null != dungeon.current_room.monster)
		{
			AudioManager.Instance.Stop(AudioManager.DUNGEON_BGM);
			AudioManager.Instance.Play(AudioManager.BATTLE_BGM, true);
			
			main_buttons.Hide(0.1f);
			yield return StartCoroutine(mini_map.Hide(0.5f));
			yield return StartCoroutine(battle.BattleStart(dungeon.current_room.monster));
			main_buttons.Show(0.5f);
			yield return StartCoroutine(mini_map.Show(0.5f));

			AudioManager.Instance.Stop(AudioManager.BATTLE_BGM);
			AudioManager.Instance.Play(AudioManager.DUNGEON_BGM, true);
			if (true == battle.battle_result)
			{
				yield return StartCoroutine(Win(dungeon.current_room.monster));
				dungeon.current_room.monster = null;
			}
			else
			{
				yield return StartCoroutine(Lose());
			}
		}

		Debug.Log("Dungeon_Move_Finish");
		Util.EventSystem.Publish(EventID.Dungeon_Move_Finish);
	}

	private IEnumerator Win(Monster.Meta meta)
	{
		string text = "";
		text += "You defeated \'" + meta.name + "\'\n";

		int prevPlayerLevel = GameManager.Instance.player.level;
		Stat stat = GameManager.Instance.player.stats;
		int rewardCoin = meta.reward.coin + (int)Random.Range(-meta.reward.coin * 0.1f, meta.reward.coin * 0.1f);
		int bonusCoin = (int)(rewardCoin * stat.GetStat(StatType.CoinBonus) / 100.0f);
		int rewardExp = meta.reward.exp + (int)Random.Range(-meta.reward.exp * 0.1f, meta.reward.exp * 0.1f);
		int bonusExp = (int)(rewardExp * stat.GetStat(StatType.ExpBonus) / 100.0f);

		Item item = null;
		if (meta.reward.item_chance >= Random.Range(0, 100))
		{
			item = ItemManager.Instance.CreateRandomEquipItem(meta.level);
			GameManager.Instance.player.inventory.Add(item);
		}
		GameManager.Instance.player.coin += rewardCoin + bonusCoin;
		GameManager.Instance.player.AddExp(rewardExp + bonusExp);
		ProgressManager.Instance.Update(Achieve.AchieveType_CollectCoin, "", rewardCoin + bonusCoin);
		ProgressManager.Instance.Update(Achieve.AchieveType_Level, "", GameManager.Instance.player.level);

		CreateCoins(rewardCoin + bonusCoin);
		for (int i = prevPlayerLevel; i < GameManager.Instance.player.level; i++)
		{
			yield return StartCoroutine(player_exp.SetCurrent(player_exp.max, 0.3f));
			player_exp.max = GameManager.Instance.player.GetMaxExp(i);
			yield return StartCoroutine(player_exp.SetCurrent(0.0f, 0.0f));
		}
		
		player_exp.max = GameManager.Instance.player.GetMaxExp();
		player_exp.current = GameManager.Instance.player.exp;
		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;

		text += "Coins : +" + rewardCoin + (0 < bonusCoin ? "(" + bonusCoin + " bonus)" : "") + "\n";
		text += "Exp : +" + rewardExp + (0 < bonusExp ? "(" + bonusExp + " bonus)" : "") + "\n";
		if (prevPlayerLevel < GameManager.Instance.player.level)
		{
			text += "Level : " + prevPlayerLevel + " -> " + GameManager.Instance.player.level + "\n";
		}
		if (null != item)
		{
			text += "Item : " + item.meta.name + "\n";
		}

		/*
		QuestManager.Instance.Update("KillMonster", info.id);
		yield return StartCoroutine(CheckCompleteQuest());
		*/

		Transform playerParent = ui_player_transform.parent;
		ui_player_transform.SetParent(GameManager.Instance.ui_textbox.transform);
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(text));
		ui_player_transform.SetParent(playerParent);
	}

	private void OnChangePlayerHealth()
	{
		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
	}

	private IEnumerator OnExitUnlock()
	{
		if (Dungeon.Room.Type.Exit == dungeon.current_room.type)
		{
			touch_input.AddBlockCount();
			bool goDown = false;
			GameManager.Instance.ui_dialogbox.onSubmit += () => {
				goDown = true;
			};
			yield return StartCoroutine(GameManager.Instance.ui_dialogbox.Write("Do you want to go down the stair?"));
			if (true == goDown)
			{
				yield return StartCoroutine(GoDown());
				InitDungeon();
			}
			touch_input.ReleaseBlockCount();
		}
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

	/*
	IEnumerator CheckCompleteQuest()
	{
		state = State.Popup;
		foreach(QuestData quest in completeQuests)
		{
			if (null == quest.completeDialouge) {
				continue;
			}
			if (null == quest.completeDialouge.dialouge) {
				continue;
			}

			state = State.Popup;
			yield return StartCoroutine (UINpc.Instance.Talk (quest.completeDialouge.speaker, quest.completeDialouge.dialouge));
			state = State.Idle;
		}
		completeQuests.Clear ();
		state = State.Idle;
	}
	*/
}
