using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DungeonMain : SceneMain {
    public UIButtonGroup mainButtonGroup;
    public UIButtonGroup battleButtonGroup;
    
    public UIMiniMap miniMap;
	public UIInventory inventory;
	public RectTransform uiMain;
    public Text dungeonLevel;
    public float walkDistance;
    public float walkSpeed;
    public float battleSpeed;
    public AudioSource audioWalk;
    public AudioSource audioBG;
    public AudioSource audioMonsterDie;

    public Transform 	bloodMarkPanel;
    public BloodMark    bloodMarkPrefab;
    public Coin         coinPrefab;
    public Transform    coins;
    public UIDungeonPlayer player;
    private int level;
    
    private Transform rooms;
    private Room current;
    private Room[] next = new Room[Dungeon.Max];
    public Monster monster;

    private Config config;
    private Dungeon.LevelInfo dungeonLevelInfo;
    private Vector3 touchPoint = Vector3.zero;
    private TouchInput input;
	private List<QuestData> completeQuests;
    private static DungeonMain _instance;  
	public static DungeonMain Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (DungeonMain)GameObject.FindObjectOfType(typeof(DungeonMain));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "DungeonMain";  
					_instance = container.AddComponent<DungeonMain>();  
				}  
			}  
			return _instance;  
		}  
	}

	[System.Serializable]
	public class Config
	{
		public Dungeon.LevelInfo[] level_infos;
	}

	public enum State
	{
		Invalid, Idle, Move, Battle, Popup, Max
	}

	public State state {
		set {
			if (true == Application.isPlaying) {
				return;
			}
			switch (value) {
			case State.Invalid:
				input.enabled = false;
				mainButtonGroup.gameObject.SetActive (false);
				battleButtonGroup.gameObject.SetActive (false);
				break;
			case State.Idle:
				input.enabled = true;
				mainButtonGroup.gameObject.SetActive (true);
				battleButtonGroup.gameObject.SetActive (false);
				break;
			case State.Move:
				input.enabled = false;
				mainButtonGroup.gameObject.SetActive (true);
				mainButtonGroup.Enable (false);
				battleButtonGroup.gameObject.SetActive (false);
				break;
			case State.Battle:
				input.enabled = false;
				mainButtonGroup.gameObject.SetActive (false);
				battleButtonGroup.gameObject.SetActive (true);
				break;
			case State.Popup:
				input.enabled = false;
                mainButtonGroup.enabled = false; 
                battleButtonGroup.enabled = false;
                break;
			default :
				throw new System.Exception ("undefined state : " + value.ToString ());
			}
		}
	}

	public override IEnumerator Run () {
		Analytics.CustomEvent("DungeonMain", new Dictionary<string, object>{});
		GameObject commonUI = GameObject.Find ("CommonUI");
		inventory.transform.SetParent (commonUI.transform);
		rooms = transform.FindChild ("Rooms");
		current = rooms.FindChild ("Current").GetComponent<Room> ();
		next [Dungeon.North] = rooms.FindChild ("North").GetComponent<Room> ();
		next [Dungeon.North].transform.position = new Vector3 (0.0f, 0.0f, walkDistance);
		next [Dungeon.East] =  rooms.FindChild ("East").GetComponent<Room> ();
		next [Dungeon.East].transform.position = new Vector3 (walkDistance, 0.0f, 0.0f);
		next [Dungeon.South] =  rooms.FindChild ("South").GetComponent<Room> ();
		next [Dungeon.South].transform.position = new Vector3 (0.0f, 0.0f, -walkDistance);
		next [Dungeon.West] =  rooms.FindChild ("West").GetComponent<Room> ();
		next [Dungeon.West].transform.position = new Vector3 (-walkDistance, 0.0f, 0.0f);
		uiMain.position = Camera.main.WorldToScreenPoint(monster.transform.position);
     
		mainButtonGroup.Init ();
		mainButtonGroup.gameObject.SetActive (false);
		mainButtonGroup.actions [0] += () => {
			player.inventory.gameObject.SetActive(true);
		};
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

        input = GetComponent<TouchInput> ();
		input.onTouchDown += (Vector3 position) => {
			touchPoint = position;
		};
		input.onTouchUp += (Vector3 position) => {
			float distance = Vector3.Distance(touchPoint, position);
			if(0.02f > distance) {
				Debug.Log("not enough drag distance(" + distance + ")");
				return;
			}
			Vector3 delta = position - touchPoint;

			if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))	{
				if(0.0f > delta.x) {
					StartCoroutine(Move(Dungeon.East));
				}
				else {
					StartCoroutine(Move(Dungeon.West));
				}
			}
			else {
				if(0.0f > delta.y) {
					StartCoroutine(Move(Dungeon.North));
				}
				else {
					StartCoroutine(Move(Dungeon.South));
				}
			}
			touchPoint = Vector3.zero;
		};
		completeQuests = new List<QuestData> ();
		StartCoroutine (Init ());
        yield break;
	}

	IEnumerator Init() {
		rooms.gameObject.SetActive (false);
		state = State.Invalid;
		level = 1;
		#if UNITY_EDITOR
		NetworkManager.Instance.Init ();
		yield return StartCoroutine(ResourceManager.Instance.Init ());
		yield return StartCoroutine(ItemManager.Instance.Init ());
		yield return StartCoroutine(MonsterManager.Instance.Init ());
		#endif

		yield return StartCoroutine(QuestManager.Instance.Init ());
		QuestManager.Instance.onComplete += (QuestData data) => {
			completeQuests.Add(data);
		};
		#if UNITY_EDITOR
		Player.Instance.Init();
        #endif
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
        player.Init();
		InitDungeon ();
		rooms.gameObject.SetActive (true);
		state = State.Idle;
	}
	void InitDungeon() {
		dungeonLevelInfo = config.level_infos [(level - 1) % config.level_infos.Length];
		ItemManager.Instance.InitDungeonLevel (dungeonLevelInfo);
		Dungeon.Instance.Init (dungeonLevelInfo);
		miniMap.Init ();
		InitRooms ();
		dungeonLevel.text = "<size=" + (dungeonLevel.fontSize * 0.8f) + ">B</size> " + level.ToString ();
		StartCoroutine(CameraFadeTo(Color.black, iTween.Hash("amount", 0.0f, "time", 1.0f)));

		Analytics.CustomEvent("InitDungeon", new Dictionary<string, object>	{
			{"dungeon_level", level },
			{"player_level", player.level},
			{"player_exp", player.exp.current },
			{"player_gold", Player.Instance.coins }
		});
	}

	void InitRooms()
	{
		rooms.transform.position = Vector3.zero;
		current.Init (Dungeon.Instance.current);
		for(int i=0; i<Dungeon.Max; i++) {
			Dungeon.Room room = Dungeon.Instance.current.next [i];
			if (null != room) {
				next [i].Init (room);
			}
		}
		miniMap.CurrentPosition (Dungeon.Instance.current.id);
	}
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
	IEnumerator Battle()
	{
		battleButtonGroup.names [0].text = "Heal(" + Player.Instance.inventory.GetItems<HealingPotionItem> ().Count.ToString() + ")";
		StartCoroutine(miniMap.Hide(1.0f));
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
    }
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

	IEnumerator GoDown()
	{
		Vector3 position = Camera.main.transform.position;
		StartCoroutine(CameraFadeTo(Color.black, iTween.Hash("amount", 1.0f, "time", 1.0f)));
		yield return StartCoroutine(MoveTo (Camera.main.gameObject, iTween.Hash(
			"x", current.stair.transform.position.x,
			"y", current.stair.transform.position.y,
			"z", current.stair.transform.position.z-0.5f,
			"time", 1.0f
		), true));
		level += 1;

		Camera.main.transform.position = position;
	}
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
}
