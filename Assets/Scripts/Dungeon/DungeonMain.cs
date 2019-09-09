using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DungeonMain : SceneMain
{
	public UIButtonGroup main_buttons;
	public UIButtonGroup battle_buttons;
    public UIMiniMap mini_map;
	public Transform coins;

	private float room_size = 7.2f; // walkDistance;
	private float room_move_speed = 21.6f;
	private Transform rooms;
	private Room current_room;
	private readonly Room[] next_rooms = new Room[Dungeon.Max];

	private TouchInput touch_input;
	private Vector3 touch_point = Vector3.zero;

	public Monster monster;
	public Text ui_monster_name;
	public UIGaugeBar ui_monster_health;

	private Dungeon dungeon;
	public int dungeon_level = 1;
    public Text ui_dungeon_level;
	public UIInventory ui_inventory;
	public UICoin ui_coin;
	

	/*
    public float battleSpeed;
    public AudioSource audioWalk;
    public AudioSource audioBG;
    public AudioSource audioMonsterDie;

    public Transform 	bloodMarkPanel;
    public BloodMark    bloodMarkPrefab;
    public Coin         coinPrefab;
    
    public UIDungeonPlayer player;
    
    private Config config;
    private Dungeon.LevelInfo dungeonLevelInfo;
    
    private List<QuestData> completeQuests;
    
	[System.Serializable]
	public class Config
	{
		public Dungeon.LevelInfo[] level_infos;
	}
*/
	public override IEnumerator Run ()
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

		ui_inventory = UIUtil.FindChild<UIInventory>(transform, "UI/UIInventory");
		ui_inventory.Init();
		
		main_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "UI/Main/MainButtonGroup");
		main_buttons.Init();
		main_buttons.actions[0] += () => {
			ui_inventory.SetActive(true);
		};
		main_buttons.actions[1] += () => {
			Debug.Log("touch ui");
		};
		main_buttons.buttons[2].gameObject.SetActive(false);
		main_buttons.buttons[3].gameObject.SetActive(false);
		
		battle_buttons = UIUtil.FindChild<UIButtonGroup>(transform, "UI/Main/BattleButtonGroup");
		battle_buttons.Init();

		monster = UIUtil.FindChild<Monster>(transform, "Monster");
		mini_map = UIUtil.FindChild<UIMiniMap>(transform, "UI/UIMiniMap");
		ui_dungeon_level = UIUtil.FindChild<Text>(transform, "UI/DungeonLevel");

		touch_input = GetComponent<TouchInput>();
		if (null == touch_input)
		{
			throw new System.Exception("can not find component 'TouchInput'");
		}
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

		dungeon = new Dungeon();
		
		Analytics.CustomEvent("DungeonMain", new Dictionary<string, object> { });
		/*
		
		GameObject commonUI = GameObject.Find ("CommonUI");
		inventory.transform.SetParent (commonUI.transform);
															   
		
		uiMain.position = Camera.main.WorldToScreenPoint(monster.transform.position);
     
		
		battleButtonGroup.Init ();
		battleButtonGroup.gameObject.SetActive (false);
		battleButtonGroup.actions [0] += () => {
			for(int i=0; i<Inventory.MAX_SLOT_COUNT; i++)
			{
				HealingPotionItem item = Player.Instance.inventory.GetItem<HealingPotionItem>(i);
				if(null != item)
				{
					item.Use(player);
					inventory.Pull(i);
					battleButtonGroup.names [0].text = "Heal(" + Player.Instance.inventory.GetItems<HealingPotionItem> ().Count.ToString() + ")";
					break;
				}
			}
		};

        completeQuests = new List<QuestData> ();
		StartCoroutine (Init ());
		*/
		Init();

		Util.EventSystem.Subscribe(EventID.Inventory_Open, OnTouchInputBlock);
		Util.EventSystem.Subscribe(EventID.Inventory_Close, OnTouchInputRelease);
		Util.EventSystem.Subscribe(EventID.Dialog_Open, OnTouchInputBlock);
		Util.EventSystem.Subscribe(EventID.Dialog_Close, OnTouchInputRelease);
		Debug.Log("init complete dungeon");
	}

	private void OnDestroy()
	{
		Util.EventSystem.Unsubscribe(EventID.Inventory_Open, OnTouchInputBlock);
		Util.EventSystem.Unsubscribe(EventID.Inventory_Close, OnTouchInputRelease);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Open, OnTouchInputBlock);
		Util.EventSystem.Unsubscribe(EventID.Dialog_Close, OnTouchInputRelease);
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

	  UICoin.Instance.Init ();
	  
	  */
	  InitDungeon ();
		//state = State.Idle;
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

	/*
	IEnumerator Move(int direction)
	{
		state = State.Move;
		Dungeon.Room room = Dungeon.Instance.current;
		Dungeon.Room next = room.GetNext (direction);
		if (null == next) {
			switch (direction) {
			case Dungeon.North:
				iTween.PunchPosition (rooms.gameObject, new Vector3 (0.0f, 0.0f, 1.0f), 0.5f);
				break;
			case Dungeon.East:
				iTween.PunchPosition (rooms.gameObject, new Vector3 (1.0f, 0.0f, 0.0f), 0.5f);
				break;
			case Dungeon.South:
				iTween.PunchPosition (rooms.gameObject, new Vector3 (0.0f, 0.0f, -1.0f), 0.5f);
				break;
			case Dungeon.West:
				iTween.PunchPosition (rooms.gameObject, new Vector3 (-1.0f, 0.0f, 0.5f), 0.5f);
				break;
			default :
				break;
			}
			state = State.Idle;
			yield break;
		} 

		if(0 < coins.childCount) {
            for(int i=0; i<coins.childCount; i++)
            {
                Coin coin = coins.GetChild(i).GetComponent<Coin>();
                coin.Stop();
            }
			while (0 < coins.childCount) {
                yield return null;
			}
		}
			
		Vector3 position = Vector3.zero;
		switch (direction) {
		case Dungeon.North:
			position = new Vector3 (rooms.position.x, rooms.position.y, rooms.position.z - walkDistance);
			break;
		case Dungeon.East :
			position = new Vector3 (rooms.position.x - walkDistance, rooms.position.y, rooms.position.z);
			break;
		case Dungeon.South :
			position = new Vector3 (rooms.position.x, rooms.position.y, rooms.position.z + walkDistance);
			break;
		case Dungeon.West:
			position = new Vector3 (rooms.position.x + walkDistance, rooms.position.y, rooms.position.z);
			break;
		default :
			break;
		}
        audioWalk.Play();
		yield return StartCoroutine(MoveTo(rooms.gameObject, iTween.Hash ("position", position, "time", walkDistance/walkSpeed, "easetype", iTween.EaseType.easeInQuad), true));
		Dungeon.Instance.Move (direction);
		InitRooms ();
        audioWalk.Stop();

		if (null != Dungeon.Instance.current.monster) {
            state = State.Battle;
            monster.Init (Dungeon.Instance.current.monster);
			yield return StartCoroutine (Battle ());
		} 
	
		if (Dungeon.Room.Type.Exit == Dungeon.Instance.current.type) {
            state = State.Popup;
            bool goDown = false;
			UIDialogBox.Instance.onSubmit += () =>  {
				goDown = true;
			};
			yield return StartCoroutine(UIDialogBox.Instance.Write("Do you want to go down the stair?"));
			if (true == goDown) {
				yield return StartCoroutine (GoDown ());
				InitDungeon ();
				yield return new WaitForSeconds (1.0f);
			}
		}

		state = State.Idle;
	}
	*/
	IEnumerator Battle()
	{
		//battle_buttons.names [0].text = "Heal(" + GamePlayer.Instance.inventory.GetItems<HealingPotionItem> ().Count.ToString() + ")";
		yield return StartCoroutine(mini_map.Hide(1.0f));
		/*
		yield return StartCoroutine(monster.Show(1.0f));

		// attack per second
		float playerAPS = Player.Instance.GetStat().speed/monster.info.speed; 
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;
		
		while (0.0f < monster.health.current && 0.0f < player.health.current) {
			float waitTime = 0.0f;

			if (monsterTurn < playerTurn) {
				int attackCount = 1;

				if (0 == Random.Range (0, 5)) {
					attackCount += 1;
					waitTime = 1.0f / battleSpeed / 2;
				}
				if (0 == Random.Range (0, 10)) {
					attackCount += 1;
					waitTime = 1.0f / battleSpeed / 3;
                }

				for (int i = 0; i < attackCount; i++) {
					player.Attack(monster);
					yield return new WaitForSeconds (waitTime);
				}
				monsterTurn += monsterAPS + Random.Range(0, monsterAPS * 0.1f);
			}
			else 
			{
				monster.Attack (player);
                StartCoroutine(CameraFadeFrom(Color.white, iTween.Hash("amount", 0.1f, "time", 0.1f)));
                iTween.ShakePosition(Camera.main.gameObject, new Vector3(0.3f, 0.3f, 0.0f), 0.2f);

                BloodMark bloodMark = GameObject.Instantiate<BloodMark>(bloodMarkPrefab);
                bloodMark.transform.SetParent(bloodMarkPanel.transform, false);
                bloodMark.transform.position = new Vector3(
                    Random.Range(Screen.width / 2 - Screen.width / 2 * 0.85f, Screen.width / 2 + Screen.width / 2 * 0.9f),
                    Random.Range(Screen.height / 2 - Screen.height / 2 * 0.85f, Screen.height / 2 + Screen.height / 2 * 0.9f),
                    0.0f
                );
                playerTurn += playerAPS + Random.Range(0, playerAPS * 0.1f);
			}
			yield return new WaitForSeconds (1.0f/battleSpeed);
		}
		
		monster.ui.gameObject.SetActive (false);
		if (0.0f < monster.health.current) {
			yield return StartCoroutine (Lose ());
		}
		else {
			GameObject.Instantiate<GameObject> (monster.dieEffectPrefab);
			monster.gameObject.SetActive (false);
			yield return StartCoroutine (Win (monster.info));
			yield return StartCoroutine (miniMap.Show (0.5f));
			Dungeon.Instance.current.monster = null;
		}
		*/
    }
	/*
	IEnumerator Win(Monster.Info info)
	{
        audioMonsterDie.Play();

        string text = "";
		text += "You defeated \'" + info.name + "\'\n";

		Unit.Stat stat = player.GetStat ();
		int gainCoins = info.reward.coin + (int)Random.Range (-info.reward.coin * 0.1f, info.reward.coin * 0.1f);
		int coinBonus = (int)(gainCoins * stat.coinBonus/100.0f);
		int gainExp = info.reward.exp + (int)Random.Range (-info.reward.exp * 0.1f, info.reward.exp * 0.1f);
		int expBonus = (int)(gainExp * stat.expBonus/100.0f);
		text += "Coins : +" + gainCoins + (0 < coinBonus ? "(" + coinBonus + " bonus)" : "") + "\n";
		text += "Exp : +" + gainExp + (0 < expBonus ? "(" + expBonus + " bonus)" : "") + "\n";

        CreateCoins(gainCoins + coinBonus);

        int playerLevel = player.level;
        yield return StartCoroutine(player.AddExp(gainExp + expBonus));
		text += "Level : " + playerLevel + " -> " + player.level + "\n";

		if(dungeonLevelInfo.items.chance >= Random.Range(0.0f, 1.0f)) 
		{
			Item item = ItemManager.Instance.CreateRandomItem (this.level);
			inventory.Put (item);
			text += "You got a " + item.name + "\n";
		}

		if(10 >= Random.Range(0, 100)) {
			Item item = ItemManager.Instance.CreateItem ("ITEM_POTION_HEALING");
			inventory.Put (item);
			text += "You got a " + item.name;
		}
		yield return StartCoroutine(UITextBox.Instance.Write(text));

		QuestManager.Instance.Update ("KillMonster", info.id);
		yield return StartCoroutine(CheckCompleteQuest ());

        Analytics.CustomEvent("Win", new Dictionary<string, object>
        {
			{"dungeon_level", level},
            {"monster_id", info.id },
			{"player_level", playerLevel}
        });
    }
	IEnumerator Lose()
	{
		Analytics.CustomEvent("Lose", new Dictionary<string, object>
		{
			{"dungeon_level", level },
			{"player_level", player.level}
		});
		
		yield return StartCoroutine(UITextBox.Instance.Write(
			"You died.\n" +
			"Your body will be carried to village.\n" +
			"See you soon.."
		));
		yield return StartCoroutine (CameraFadeTo (Color.black, iTween.Hash ("amount", 1.0f, "time", 1.0f), true));
		SceneManager.LoadScene("Village");
	}
	*/
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

    void CreateCoins(int amount)
    {
        int total = amount;
        int multiply = 1;
        float scale = 1.0f;
        
		while (0 < total) {
			int countCount = Random.Range (1, 10);
			for (int i = 0; i < countCount; i++) {
				Coin coin = GameObject.Instantiate<Coin> (coinPrefab);
				coin.amount = Mathf.Min(total, multiply);
				coin.transform.SetParent (coins, false);
				coin.transform.localScale = new Vector3 (scale, scale, 1.0f);
				coin.transform.localPosition = Vector3.zero;
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
	*/

	IEnumerator Move(int direction)
	{
		touch_input.touchBlockCount++;
		Dungeon.Room current_room = dungeon.current_room;
		Dungeon.Room next_room = current_room.GetNext(direction);

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
			touch_input.touchBlockCount--;
			yield break;
		}

		Util.EventSystem.Publish(EventID.Dungeon_Move_Start);
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
		yield return StartCoroutine(MoveTo(rooms.gameObject, iTween.Hash("position", position, "time", room_size / room_move_speed, "easetype", iTween.EaseType.easeInQuad), true));
		InitRooms();
		Util.EventSystem.Publish(EventID.Dungeon_Move_Finish);
		/*
		if (0 < coins.childCount)
		{
			for (int i = 0; i < coins.childCount; i++)
			{
				Coin coin = coins.GetChild(i).GetComponent<Coin>();
				coin.Stop();
			}
			while (0 < coins.childCount)
			{
				yield return null;
			}
		}

		
		audioWalk.Play();
		
		InitRooms();
		audioWalk.Stop();
		*/
		/*
		if (null != dungeon.current_room.monster_meta)
		{
			monster.Init(dungeon.current_room.monster_meta);
			yield return StartCoroutine(Battle());
		}
		*/
		
		if (Dungeon.Room.Type.Exit == dungeon.current_room.type)
		{
			bool goDown = false;
			UIDialogBox.Instance.onSubmit += () => {
				goDown = true;
			};
			yield return StartCoroutine(UIDialogBox.Instance.Write("Do you want to go down the stair?"));
			if (true == goDown)
			{
				yield return StartCoroutine(GoDown());
				InitDungeon();
				yield return new WaitForSeconds(1.0f);
			}
		}
		touch_input.touchBlockCount--;
	}


	private void OnTouchInputBlock()
	{
		touch_input.touchBlockCount++;
		if (0 < touch_input.touchBlockCount)
		{
			mini_map.Hide(1.0f);
		}
	}

	private void OnTouchInputRelease()
	{
		touch_input.touchBlockCount--;
		if (0 == touch_input.touchBlockCount)
		{
			mini_map.Show(1.0f);
		}
	}
	
}
