using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneDungeon : SceneMain
{
	public UIButtonGroup main_buttons;
    public UIMiniMap mini_map;
	
	private float room_size = 7.2f; // walkDistance;
	private float room_move_speed = 21.6f;
	private Transform rooms;
	private Room current_room;
	private readonly Room[] next_rooms = new Room[Dungeon.Max];

	private TouchInput touch_input;
	private Vector3 touch_point = Vector3.zero;

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

	/*
    public AudioSource audioWalk;
    public AudioSource audioBG;
    public AudioSource audioMonsterDie;
    
    private Config config;
    private Dungeon.LevelInfo dungeonLevelInfo;
    
    private List<QuestData> completeQuests;
*/
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
		battle.gameObject.SetActive(false);
		box = UIUtil.FindChild<DungeonBox>(transform, "Box");		
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
		};
		touch_input.onTouchUp += (Vector3 position) => 
		{
			float distance = Vector3.Distance(touch_point, position);
			
			if (0.02f > distance)
			{
				Debug.Log("not enough drag distance(" + distance + ")");
				return;
			}
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

		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Start, () => { touch_input.AddBlockCount(); });
		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Finish, () => { touch_input.ReleaseBlockCount(); });

		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Finish, InitRooms);
		Util.EventSystem.Subscribe(EventID.Player_Change_Health, OnChangePlayerHealth);
		Util.EventSystem.Subscribe(EventID.TextBox_Close, DestroyCoins);

		/*
		audioWalk = GameObject.Instantiate<AudioSource>(audioWalk);
		audioBG = GameObject.Instantiate<AudioSource>(audioBG);
		audioMonsterDie = GameObject.Instantiate<AudioSource>(audioMonsterDie);
		*/
		Analytics.CustomEvent("DungeonMain", new Dictionary<string, object> { });
		/*
        completeQuests = new List<QuestData> ();
		StartCoroutine (Init ());
		*/
		
		InitScene();
		
		touch_input.ReleaseBlockCount();
		Debug.Log("init complete dungeon");

		string[] scripts = new string[] {
			"오래전 저 성엔 부와 젊을을 위해 백성들을 악마의 제물로 바치던 피의 여왕이 살았다고 하네.",
			"이젠 전설이 되어 아는 사람이 별로 남지 않았지만 확실한건 저 성엔 여왕이 남긴 엄청난 보물이 있다는 거지.",
			"하지만 아직도 여왕의 유령이 저 성 지하에서 부활할 날 만을 기다리고 있다고 하네.\n많은 모험가들이 저 성으로 들어갔지만 아무도 돌아오지 않았어.\n\n" +
			"행운을 빈다네 모험가"
		};
		touch_input.AddBlockCount();
		yield return StartCoroutine(GameManager.Instance.ui_npc.Talk("", scripts));
		touch_input.ReleaseBlockCount();
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Open);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Close);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Start);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Finish);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Finish, InitRooms);
		Util.EventSystem.Unsubscribe(EventID.Player_Change_Health, OnChangePlayerHealth);
		Util.EventSystem.Unsubscribe(EventID.TextBox_Close, DestroyCoins);
	}

	void InitScene()
	{
		GameManager.Instance.player.level = 1;
		GameManager.Instance.player.exp = 0;
		GameManager.Instance.player.max_health = 100;
		GameManager.Instance.player.cur_health = GameManager.Instance.player.max_health;

		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
		player_exp.max = GameManager.Instance.player.GetMaxExp();
		player_exp.current = GameManager.Instance.player.exp;

		dungeon_level = 1;
		InitDungeon ();
		/*
		yield return StartCoroutine(QuestManager.Instance.Init ());
		QuestManager.Instance.onComplete += (QuestData data) => {
			completeQuests.Add(data);
		};
		QuestManager.Instance.Update (QuestProgress.Type.CrrentLocation, "Dungeon");
		yield return StartCoroutine(CheckCompleteQuest ());
		*/
	}
	void InitDungeon()
	{
		StartCoroutine(GameManager.Instance.CameraFade(1.0f, 0.0f, 1.5f));
		dungeon.Init(dungeon_level);
		mini_map.Init(dungeon);
		InitRooms();
		
		ui_dungeon_level.text = "<size=" + (ui_dungeon_level.fontSize * 0.8f) + ">B</size> " + dungeon_level.ToString();
		/*		
				Analytics.CustomEvent("InitDungeon", new Dictionary<string, object> {
					//{"dungeon_level", level },
					{"player_level", GameManager.Instance.player.level},
					//{"player_exp", GameManager.Instance.player.exp.current },
					{"player_gold", GameManager.Instance.player.inventory.coin }
				});
				*/
	}

	void InitRooms()
	{
		rooms.transform.position = Vector3.zero;
		current_room.Init (dungeon.current_room);
		for(int i=0; i<Dungeon.Max; i++)
		{
			Dungeon.Room room = dungeon.current_room.next [i];
			if (null != room)
			{
				next_rooms [i].Init (room);
			}
		}
		mini_map.CurrentPosition (dungeon.current_room.id);
	}

	IEnumerator Lose()
	{
		/*
		Analytics.CustomEvent("Lose", new Dictionary<string, object>
		{
			{"dungeon_level", level },
			{"player_level", player.level}
		});
		*/
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(
			"You died.\n" +
			"Your body will be carried to village.\n" +
			"See you soon.."
		));
		//yield return StartCoroutine (CameraFadeTo (Color.black, iTween.Hash ("amount", 1.0f, "time", 1.0f), true));
		//SceneManager.LoadScene("Village");
	}
	
	IEnumerator GoDown()
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
	
    void CreateCoins(int amount)
    {
        int total = amount;
        int multiply = 1;
        float scale = 1.0f;
        
		while (0 < total) {
			int countCount = Random.Range (5, 10);
			for (int i = 0; i < countCount; i++) {
				Coin coin = GameObject.Instantiate<Coin> (coin_prefab);
				coin.amount = Mathf.Min(total, multiply);
				coin.transform.SetParent(coin_spot, false);
				coin.transform.localScale = new Vector3(scale, scale, 1.0f);
				coin.transform.localPosition = Vector3.zero;
				coin.gameObject.SetActive(true);
				iTween.MoveBy (coin.gameObject, new Vector3 (Random.Range (-1.5f, 1.5f), Random.Range (0.0f, 0.5f), 0.0f), 0.5f);

				total -= coin.amount;
				if (0 >= total) {
					return;
				}
			}
			multiply *= 10;
			scale += 0.1f;
		}
    }

	void DestroyCoins()
	{
		for (int i = 0; i < coin_spot.childCount; i++)
		{
			Transform child = coin_spot.GetChild(i);
			Coin coin = child.GetComponent<Coin>();
			if (null == coin)
			{
				Debug.LogError("can not find component 'Coin'");
				continue;
			}
			coin.Stop();
		}
	}
	IEnumerator Move(int direction)
	{
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

		while (0 < coin_spot.childCount)
		{
			yield return null;
		}

		Vector3 position = Vector3.zero;
		switch (direction)
		{
			case Dungeon.North:
				position = new Vector3(rooms.position.x, rooms.position.y, rooms.position.z - room_size);
				break;
			case Dungeon.East:
				position = new Vector3(rooms.position.x - room_size, rooms.position.y, rooms.position.z);
				break;
			case Dungeon.South:
				position = new Vector3(rooms.position.x, rooms.position.y, rooms.position.z + room_size);
				break;
			case Dungeon.West:
				position = new Vector3(rooms.position.x + room_size, rooms.position.y, rooms.position.z);
				break;
			default:
				break;
		}

		dungeon.Move(direction);
		//	audioWalk.Play();
		yield return StartCoroutine(MoveTo(rooms.gameObject, iTween.Hash("position", position, "time", room_size / room_move_speed, "easetype", iTween.EaseType.easeInQuad), true));
		//	audioWalk.Stop();

		InitRooms();
		if (100 > Random.Range(0, 100))
		{
			yield return StartCoroutine(box.Show());
		}
		else if (30 > Random.Range(0, 100) && Dungeon.Room.Type.Normal == dungeon.current_room.type && 0 < dungeon.monster_count)
		{
			string monsterID = dungeon.monster_ids[Random.Range(0, dungeon.monster_ids.Count - 1)];
			Monster.Meta meta = MonsterManager.Instance.FindMeta(monsterID);

			yield return StartCoroutine(mini_map.Hide(0.5f));
			main_buttons.Hide(0.1f);
			yield return StartCoroutine(battle.BattleStart(meta));
			main_buttons.Show(0.5f);
			yield return StartCoroutine(mini_map.Show(0.5f));
			if (true == battle.battle_result)
			{
				yield return StartCoroutine(Win(meta));
			}
			else
			{
				yield return StartCoroutine(Lose());
			}
			dungeon.monster_count--;
		}
		


		if (Dungeon.Room.Type.Exit == dungeon.current_room.type)
		{
			bool goDown = false;
			GameManager.Instance.ui_dialogbox.onSubmit += () => {
				goDown = true;
			};
			yield return StartCoroutine(GameManager.Instance.ui_dialogbox.Write("Do you want to go down the stair?"));
			if (true == goDown)
			{
				yield return StartCoroutine(GoDown());
				InitDungeon();
				yield return new WaitForSeconds(1.0f);
			}
		}
		Util.EventSystem.Publish(EventID.Dungeon_Move_Finish);
	}

	IEnumerator Win(Monster.Meta meta)
	{
		int prevPlayerLevel = GameManager.Instance.player.level;
		Stat stat = GameManager.Instance.player.stats;
		int rewardCoin = meta.reward.coin + (int)Random.Range(-meta.reward.coin * 0.1f, meta.reward.coin * 0.1f);
		int bonusCoin = (int)(rewardCoin * stat.GetStat(StatType.CoinBonus) / 100.0f);
		int rewardExp = meta.reward.exp + (int)Random.Range(-meta.reward.exp * 0.1f, meta.reward.exp * 0.1f);
		int bonusExp = (int)(rewardExp * stat.GetStat(StatType.ExpBonus) / 100.0f);
		GameManager.Instance.player.inventory.coin += rewardCoin + bonusCoin;
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

		string text = "";
		text += "You defeated \'" + meta.name + "\'\n";
		text += "Coins : +" + rewardCoin + (0 < bonusCoin ? "(" + bonusCoin + " bonus)" : "") + "\n";
		text += "Exp : +" + rewardExp + (0 < bonusExp ? "(" + bonusExp + " bonus)" : "") + "\n";
		text += "Level : " + prevPlayerLevel + " -> " + GameManager.Instance.player.level + "\n";

		/*
		if (dungeonLevelInfo.items.chance >= Random.Range(0.0f, 1.0f))
		{
			Item item = ItemManager.Instance.CreateRandomItem(this.level);
			inventory.Put(item);
			text += "You got a " + item.name + "\n";
		}
		*/

		/*
		if (10 >= Random.Range(0, 100)) {
			Item item = ItemManager.Instance.CreateItem("ITEM_POTION_HEALING");
			GameManager.Instance.player.inventory.Add(item);
			text += "You got a " + item.meta.name;
		}
		*/

		/*
		QuestManager.Instance.Update("KillMonster", info.id);
		yield return StartCoroutine(CheckCompleteQuest());
		*/

		Transform playerParent = ui_player_transform.parent;
		ui_player_transform.SetParent(GameManager.Instance.ui_textbox.transform);
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(text));
		ui_player_transform.SetParent(playerParent);
		Analytics.CustomEvent("Win", new Dictionary<string, object>
		{
			{"dungeon_level", dungeon_level},
			{"monster_id", meta.id },
			{"player_level", GameManager.Instance.player.level}
		});
	}

	void OnChangePlayerHealth()
	{
		player_health.max = GameManager.Instance.player.max_health;
		player_health.current = GameManager.Instance.player.cur_health;
	}
}
