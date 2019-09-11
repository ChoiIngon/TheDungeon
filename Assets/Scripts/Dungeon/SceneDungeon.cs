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
	public UIButtonGroup battle_buttons;
    public UIMiniMap mini_map;
	
	private float room_size = 7.2f; // walkDistance;
	private float room_move_speed = 21.6f;
	private Transform rooms;
	private Room current_room;
	private readonly Room[] next_rooms = new Room[Dungeon.Max];

	private TouchInput touch_input;
	private Vector3 touch_point = Vector3.zero;

	public DungeonMonster monster;
	public Text ui_monster_name;
	public UIGaugeBar ui_monster_health;

	private Dungeon dungeon;
	public int dungeon_level = 1;
    public Text ui_dungeon_level;

    public Transform coin_spot;
    public Coin coin_prefab;

	public Transform player_damage_effect_spot;
	public Effect_PlayerDamage player_damage_effect_prefab;

	public Transform monster_damage_effect_spot;
	public Effect_MonsterDamage monster_damage_effect_prefab;

	private float battle_speed = 2.5f;
	/*
    public AudioSource audioWalk;
    public AudioSource audioBG;
    public AudioSource audioMonsterDie;
    
    private Config config;
    private Dungeon.LevelInfo dungeonLevelInfo;
    
    private List<QuestData> completeQuests;
    
	[System.Serializable]
	public class Config
	{
		public Dungeon.LevelInfo[] level_infos;
	}
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
		monster = UIUtil.FindChild<DungeonMonster>(transform, "Monster");
		player_damage_effect_spot = UIUtil.FindChild<Transform>(transform, "UI/PlayerDamage");
		player_damage_effect_prefab = UIUtil.FindChild<Effect_PlayerDamage>(player_damage_effect_spot, "Effect_PlayerDamage");
		monster_damage_effect_spot = UIUtil.FindChild<Transform>(transform, "UI/PlayerDamage");
		monster_damage_effect_prefab = UIUtil.FindChild<Effect_MonsterDamage>(monster_damage_effect_spot, "Effect_MonsterDamage");

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

		main_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "UI/Main/MainButtonGroup");
		main_buttons.Init();
		main_buttons.actions[0] += () => {
			GameManager.Instance.ui_inventory.SetActive(true);
		};
		main_buttons.actions[1] += () => {
			CreateCoins(500);
		};
		main_buttons.buttons[2].gameObject.SetActive(false);
		main_buttons.buttons[3].gameObject.SetActive(false);
		
		battle_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "UI/Main/BattleButtonGroup");
		battle_buttons.Init();

		mini_map = UIUtil.FindChild<UIMiniMap>(transform, "UI/UIMiniMap");
		ui_dungeon_level = UIUtil.FindChild<Text>(transform, "UI/DungeonLevel");

        coin_spot = UIUtil.FindChild<Transform>(transform, "CoinSpot");
        coin_prefab = UIUtil.FindChild<Coin>(transform, "CoinSpot/Coin");

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

		Analytics.CustomEvent("DungeonMain", new Dictionary<string, object> { });
		/*
        completeQuests = new List<QuestData> ();
		StartCoroutine (Init ());
		*/
		Init();

		Util.EventSystem.Subscribe(EventID.Inventory_Open, touch_input.AddBlockCount);
		Util.EventSystem.Subscribe(EventID.Inventory_Close, touch_input.ReleaseBlockCount);

		Util.EventSystem.Subscribe(EventID.Dialog_Open, touch_input.AddBlockCount);
		Util.EventSystem.Subscribe(EventID.Dialog_Close, touch_input.ReleaseBlockCount);

		Util.EventSystem.Subscribe(EventID.Dungeon_Battle_Start, touch_input.AddBlockCount);
		Util.EventSystem.Subscribe(EventID.Dungeon_Battle_Finish, touch_input.ReleaseBlockCount);
		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Start, touch_input.AddBlockCount);
		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Finish, touch_input.ReleaseBlockCount);

		Util.EventSystem.Subscribe(EventID.Dungeon_Move_Finish, InitRooms);

		touch_input.ReleaseBlockCount();
		Debug.Log("init complete dungeon");
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open, touch_input.AddBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close, touch_input.ReleaseBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Open, touch_input.AddBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Close, touch_input.ReleaseBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Battle_Start, touch_input.AddBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Battle_Finish, touch_input.ReleaseBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Start, touch_input.AddBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Finish, touch_input.ReleaseBlockCount);
		Util.EventSystem.Unsubscribe(EventID.Dungeon_Move_Finish, InitRooms);
	}
	void Init()
	{
		dungeon_level = 1;
		/*
	  
	  
	  yield return StartCoroutine(QuestManager.Instance.Init ());
	  QuestManager.Instance.onComplete += (QuestData data) => {
		  completeQuests.Add(data);
	  };
	  yield return NetworkManager.Instance.HttpRequest ("info_dungeon.php", (string json) => {
		  config = JsonUtility.FromJson<Config>(json);
	  });

	  #if UNITY_EDITOR
	  Assert.AreNotEqual(0, config.level_infos.Length);
	  #endif
	  audioWalk = GameObject.Instantiate<AudioSource>(audioWalk);
	  audioBG = GameObject.Instantiate<AudioSource>(audioBG);
	  audioMonsterDie = GameObject.Instantiate<AudioSource>(audioMonsterDie);

	  QuestManager.Instance.Update (QuestProgress.Type.CrrentLocation, "Dungeon");
	  yield return StartCoroutine(CheckCompleteQuest ());
	  */
	  InitDungeon ();
	}
	void InitDungeon()
	{
		dungeon.Init(dungeon_level);
		mini_map.Init(dungeon);
		InitRooms();
		rooms.gameObject.SetActive(true);

		ui_dungeon_level.text = "<size=" + (ui_dungeon_level.fontSize * 0.8f) + ">B</size> " + dungeon_level.ToString();

		StartCoroutine(CameraFadeTo(Color.black, iTween.Hash("amount", 0.0f, "time", 1.0f)));
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

	IEnumerator Battle()
	{
		Util.EventSystem.Publish(EventID.Dungeon_Battle_Start);
		//battle_buttons.names [0].text = "Heal(" + GamePlayer.Instance.inventory.GetItems<HealingPotionItem> ().Count.ToString() + ")";
		main_buttons.gameObject.SetActive(false);
		battle_buttons.gameObject.SetActive(true);

		yield return StartCoroutine(mini_map.Hide(0.5f));
		yield return StartCoroutine(monster.Show(0.5f));

		// attack per second
		float playerAPS = GameManager.Instance.player.speed/monster.unit.speed; 
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;

		while (0.0f < monster.unit.cur_health && 0.0f < GameManager.Instance.player.cur_health)
		{
			float waitTime = 0.0f;

			if (monsterTurn < playerTurn)
			{
				int attackCount = 1;

				if (0 == Random.Range (0, 5))
				{
					attackCount = 2;
					waitTime = 1.0f / battle_speed / 2;
				}

				if (0 == Random.Range (0, 10))
				{
					attackCount = 3;
					waitTime = 1.0f / battle_speed / 3;
                }

				for (int i = 0; i < attackCount; i++)
				{
					int attack = (int)Mathf.Max(1, GameManager.Instance.player.attack + Random.Range(-GameManager.Instance.player.attack * 0.1f, GameManager.Instance.player.attack * 0.1f) - monster.unit.defense);
					if (GameManager.Instance.player.critcal >= Random.Range(0.0f, 100.0f))
					{
						attack *= 3;
						// critical effect
					}
					Effect_MonsterDamage effect = GameObject.Instantiate<Effect_MonsterDamage>(monster_damage_effect_prefab);
					effect.damage = attack;
					effect.gameObject.SetActive(true);
					monster.unit.cur_health -= attack;
					monster.health.current -= attack;
				}
				monsterTurn += monsterAPS + Random.Range(0, monsterAPS * 0.1f);
			}
			else 
			{
				monster.Attack (GameManager.Instance.player);
				monster.animator.SetTrigger("Attack");
				int attack = (int)Mathf.Max(1, monster.unit.attack + Random.Range(0, monster.unit.attack * 0.1f) - GameManager.Instance.player.defense);

				//StartCoroutine(CameraFadeFrom(Color.white, iTween.Hash("amount", 0.1f, "time", 0.1f)));
				iTween.ShakePosition(Camera.main.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);

                Effect_PlayerDamage bloodMark = GameObject.Instantiate<Effect_PlayerDamage>(player_damage_effect_prefab);
				bloodMark.gameObject.SetActive(true);
                bloodMark.transform.SetParent(player_damage_effect_spot, false);
                bloodMark.transform.position = new Vector3(
                    Random.Range(Screen.width / 2 - Screen.width / 2 * 0.85f, Screen.width / 2 + Screen.width / 2 * 0.9f),
                    Random.Range(Screen.height / 2 - Screen.height / 2 * 0.85f, Screen.height / 2 + Screen.height / 2 * 0.9f),
                    0.0f
                );
                playerTurn += playerAPS + Random.Range(0, playerAPS * 0.1f);
			}
			yield return new WaitForSeconds (1.0f/battle_speed);
		}
		
		monster.gameObject.SetActive (false);
		if (0.0f < monster.health.current)
		{
			yield return StartCoroutine (Lose ());
		}
		else
		{
			GameObject.Instantiate<GameObject> (monster.dieEffectPrefab);
			monster.gameObject.SetActive (false);
			monster.ui_monster_info.gameObject.SetActive(false);
			yield return StartCoroutine (Win (monster.meta));
			yield return StartCoroutine (mini_map.Show (0.5f));
			//dungeon.current_room.monster_meta = null;
		}

		main_buttons.gameObject.SetActive(true);
		battle_buttons.gameObject.SetActive(false);
		Util.EventSystem.Publish(EventID.Dungeon_Battle_Finish);
    }
		
	IEnumerator Win(DungeonMonster.Meta meta)
	{
		// audioMonsterDie.Play();
		string text = "";
		text += "You defeated \'" + meta.name + "\'\n";

		Stat stat = GameManager.Instance.player.stats;
		int rewardCoin = meta.reward.coin + (int)Random.Range(-meta.reward.coin * 0.1f, meta.reward.coin * 0.1f);
		int bonusCoin = (int)(rewardCoin * stat.GetStat(StatType.CoinBonus) / 100.0f);
		int rewardExp = meta.reward.exp + (int)Random.Range(-meta.reward.exp * 0.1f, meta.reward.exp * 0.1f);
		int bonusExp = (int)(rewardExp * stat.GetStat(StatType.ExpBonus) / 100.0f);
		text += "Coins : +" + rewardCoin + (0 < bonusCoin ? "(" + bonusCoin + " bonus)" : "") + "\n";
		text += "Exp : +" + rewardExp + (0 < bonusExp ? "(" + bonusExp + " bonus)" : "") + "\n";

		GameManager.Instance.player.inventory.coin += rewardCoin + bonusCoin;
		GameManager.Instance.player.AddExp(rewardExp + bonusExp);
		CreateCoins(rewardCoin + bonusCoin);
		//StartCoroutine(player.AddExp(gainExp + expBonus));
		int playerLevel = GameManager.Instance.player.level;
		text += "Level : " + playerLevel + " -> " + GameManager.Instance.player.level + "\n";

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
		yield return StartCoroutine(GameManager.Instance.ui_textbox.Write(text));
		/*
		QuestManager.Instance.Update("KillMonster", info.id);
		yield return StartCoroutine(CheckCompleteQuest());

		Analytics.CustomEvent("Win", new Dictionary<string, object>
		{
			{"dungeon_level", level},
			{"monster_id", info.id },
			{"player_level", playerLevel}
		});
		*/
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
		yield return StartCoroutine (CameraFadeTo (Color.black, iTween.Hash ("amount", 1.0f, "time", 1.0f), true));
		//SceneManager.LoadScene("Village");
	}
	
	IEnumerator GoDown()
	{
		Vector3 position = Camera.main.transform.position;
		StartCoroutine(CameraFadeTo(Color.black, iTween.Hash("amount", 1.0f, "time", 1.0f)));
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
		if (33 > Random.Range(0, 100) && Dungeon.Room.Type.Normal == dungeon.current_room.type && 0 < dungeon.monster_count)
		{
			string monsterID = dungeon.monster_ids[Random.Range(0, dungeon.monster_ids.Count - 1)];
			DungeonMonster.Meta meta = MonsterManager.Instance.FindMeta(monsterID);
			monster.Init(meta);
			dungeon.monster_count--;
			StartCoroutine(Battle());
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
}
