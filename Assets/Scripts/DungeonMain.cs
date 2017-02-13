using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DungeonMain : MonoBehaviour {
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
	public const float DISTANCE = 7.2f;
	public const float MOVETIME = 0.4f;

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
				mainButtonGroup.gameObject.SetActive (false);
				battleButtonGroup.gameObject.SetActive (false);
				break;
			default :
				throw new System.Exception ("undefined state : " + value.ToString ());
			}
		}
	}
		
	public UIButtonGroup 	mainButtonGroup;
	public UIButtonGroup	battleButtonGroup;
	public UITextBox 		textBox;
	public UIDialogBox		dialogBox;
	public UICoin 			coin;
	public UIMiniMap		miniMap;
	public Text				dungeonLevel;
	private int level;

	private Color cameraFadeColor;
	public Transform coins;
	private Transform rooms;
	private Room current;
	private Room[] next = new Room[Dungeon.Max];
	public Monster monster;

	private Config config;
	private Dungeon.LevelInfo dungeonLevelInfo;
	private Vector3 touchPoint = Vector3.zero;
	private TouchInput input;

	void Start () {
		Analytics.CustomEvent("DungeonMain", new Dictionary<string, object>{});

		coins = transform.FindChild ("Coins");
		rooms = transform.FindChild ("Rooms");
		current = rooms.FindChild ("Current").GetComponent<Room> ();
		next [Dungeon.North] = rooms.FindChild ("North").GetComponent<Room> ();
		next [Dungeon.North].transform.position = new Vector3 (0.0f, 0.0f, DISTANCE);
		next [Dungeon.East] =  rooms.FindChild ("East").GetComponent<Room> ();
		next [Dungeon.East].transform.position = new Vector3 (DISTANCE, 0.0f, 0.0f);
		next [Dungeon.South] =  rooms.FindChild ("South").GetComponent<Room> ();
		next [Dungeon.South].transform.position = new Vector3 (0.0f, 0.0f, -DISTANCE);
		next [Dungeon.West] =  rooms.FindChild ("West").GetComponent<Room> ();
		next [Dungeon.West].transform.position = new Vector3 (-DISTANCE, 0.0f, 0.0f);
		RectTransform uiMain = transform.FindChild ("UI/Main").GetComponent<RectTransform>();
		uiMain.position = Camera.main.WorldToScreenPoint(monster.transform.position);
     
		mainButtonGroup.Init ();
		mainButtonGroup.gameObject.SetActive (false);
		mainButtonGroup.actions [0] += () => {
			Player.Instance.inventory.ui.gameObject.SetActive(true);
		};
		battleButtonGroup.Init ();
		battleButtonGroup.gameObject.SetActive (false);
		battleButtonGroup.actions [0] += () => {
			for(int i=0; i<Inventory.MAX_SLOT_COUNT; i++)
			{
				HealingPotionItem item = Player.Instance.inventory.GetItem<HealingPotionItem>(i);
				if(null != item)
				{
					item.Use(Player.Instance);
					Player.Instance.inventory.Pull(i);
					battleButtonGroup.names [0].text = "Heal(" + Player.Instance.inventory.GetItems<HealingPotionItem> ().Count.ToString() + ")";
					break;
				}
			}
		};

		iTween.CameraFadeAdd ();
		SetCameraFadeColor (Color.black);
		iTween.CameraFadeTo (1.0f, 0.0f);
		input = GetComponent<TouchInput> ();
		input.onTouchDown += (Vector3 position) => {
			touchPoint = position;
		};
		input.onTouchUp += (Vector3 position) => {
			float distance = Vector3.Distance(touchPoint, position);
			if(0.05f > distance) {
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

		StartCoroutine (Init ());
	}

	IEnumerator Init() {
		state = State.Invalid;
		level = 1;
		dungeonLevel.text = "<size=" + (dungeonLevel.fontSize*0.8f) + ">B</size> " + level.ToString ();
		#if UNITY_EDITOR
		NetworkManager.Instance.Init ();
		ResourceManager.Instance.Init ();
		yield return StartCoroutine(ItemManager.Instance.Init ());
		yield return StartCoroutine(MonsterManager.Instance.Init ());
		#endif
		yield return NetworkManager.Instance.HttpRequest ("info_dungeon.php", (string json) => {
			config = JsonUtility.FromJson<Config>(json);
		});
		#if UNITY_EDITOR
		Assert.AreNotEqual(0, config.level_infos.Length);
		#endif

		QuestManager.Instance.Init ();
		QuestManager.Instance.onComplete += (QuestData quest) => {
			StartCoroutine(textBox.Write(
				quest.name + " is completed!!"
			));
		};
		Player.Instance.Init ();

		InitDungeon ();
		state = State.Idle;
		yield break;
	}
	void InitDungeon() {
		dungeonLevelInfo = config.level_infos [(level - 1) % config.level_infos.Length];
		ItemManager.Instance.InitDungeonLevel (dungeonLevelInfo);
		Dungeon.Instance.Init (dungeonLevelInfo);
		miniMap.Init ();
		InitRooms ();
		dungeonLevel.text = "<size=" + (dungeonLevel.fontSize * 0.8f) + ">B</size> " + level.ToString ();
		iTween.CameraFadeTo(0.0f, 1.0f);
		Analytics.CustomEvent("InitDungeon", new Dictionary<string, object>	{
			{"dungeon_level", level },
			{"player_level", Player.Instance.level},
			{"player_exp", Player.Instance.exp.current },
			{"player_gold", Player.Instance.coin.count }
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
			while (0 < coins.childCount) {
				Coin coin = coins.GetChild (0).GetComponent<Coin>();
				coin.transform.SetParent (null);
				coin.Stop ();
			}
			yield return new WaitForSeconds (0.1f);
		}

		float movement = 0.0f;
		float speed = DISTANCE / MOVETIME;
		while (DISTANCE > movement) {
			movement += Time.deltaTime * speed;
			Vector3 position = rooms.position;
			switch (direction) {
			case Dungeon.North :
				position = new Vector3 (position.x, position.y, position.z - Time.deltaTime * speed);
				break;
			case Dungeon.East :
				position = new Vector3 (position.x - Time.deltaTime * speed, position.y, position.z);
				break;
			case Dungeon.South :
				position = new Vector3 (position.x, position.y, position.z + Time.deltaTime * speed);
				break;
			case Dungeon.West:
				position = new Vector3 (position.x + Time.deltaTime * speed, position.y, position.z);
				break;
			default :
				break;
			}
			rooms.position = position;
			yield return null;
		}
		Dungeon.Instance.Move (direction);

		InitRooms ();

		if (null != Dungeon.Instance.current.monster) {
			monster.Init (Dungeon.Instance.current.monster);
			yield return StartCoroutine (Battle ());
		} 
		else if (Dungeon.Room.Type.Exit == Dungeon.Instance.current.type) {
			bool goDown = false;
			dialogBox.onSubmit += () =>  {
				goDown = true;
			};
			yield return StartCoroutine(dialogBox.Write("Do you want to go down the stair?"));
			if (true == goDown) {
				yield return StartCoroutine (GoDown ());
				InitDungeon ();
				yield return new WaitForSeconds (1.0f);
				SetCameraFadeColor (Color.white);
			}
		}
		state = State.Idle;
	}
	IEnumerator Battle()
	{
		SetCameraFadeColor (Color.white);
		state = State.Battle;
		battleButtonGroup.names [0].text = "Heal(" + Player.Instance.inventory.GetItems<HealingPotionItem> ().Count.ToString() + ")";
		yield return StartCoroutine(miniMap.Hide(1.0f));

		// attack per second
		float playerAPS = Player.Instance.GetStat().speed/monster.info.speed; 
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;
		while (0.0f < monster.health.current && 0.0f < Player.Instance.health.current) {
			float waitTime = 0.0f;

			if (monsterTurn < playerTurn) {
				int attackCount = 1;

				if (0 == Random.Range (0, 3)) {
					attackCount += 1;
					waitTime = 0.5f;
				}
				if (0 == Random.Range (0, 5)) {
					attackCount += 1;
					waitTime = 0.2f;
				}

				for (int i = 0; i < attackCount; i++) {
					Player.Instance.Attack(monster);
					yield return new WaitForSeconds (waitTime);
				}
				monsterTurn += monsterAPS + Random.Range(0, monsterAPS * 0.1f);
			}
			else 
			{
				monster.Attack (Player.Instance);
				playerTurn += playerAPS + Random.Range(0, playerAPS * 0.1f);
			}
			yield return new WaitForSeconds (0.7f);
		}
		state = State.Invalid;
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
		state = State.Idle;
    }
	IEnumerator Win(Monster.Info info)
	{
		if(dungeonLevelInfo.items.chance >= Random.Range(0.0f, 1.0f)) 
		{
			Item item = ItemManager.Instance.CreateRandomItem (this.level);
			item.Pickup ();
		}

		if(10 >= Random.Range(0, 100)) {
			Item item = ItemManager.Instance.CreateItem ("ITEM_POTION_HEALING");
			item.Pickup ();
		}

		int gainCoins = info.reward.coin + (int)Random.Range (-info.reward.coin * 0.1f, info.reward.coin * 0.1f);
		int gainExp = info.reward.exp + (int)Random.Range (-info.reward.exp * 0.1f, info.reward.exp * 0.1f);
		int level = Player.Instance.level;

		Player.Instance.AddCoin (gainCoins);
		yield return StartCoroutine(Player.Instance.AddExp(gainExp));
		yield return new WaitForSeconds (0.5f);
		yield return StartCoroutine(textBox.Write(
			"You defeated \'" + info.name +"\'\n" +
			"Coins : +" + gainCoins + "\n" +
			"Exp : +" + gainExp + "\n" +
			"Level : " + level + " -> " + Player.Instance.level + "\n"
		));
		QuestManager.Instance.Update ("KillMonster", info.id);
        Analytics.CustomEvent("Win", new Dictionary<string, object>
        {
            {"monster_id", info.id }
        });
    }
	IEnumerator Lose()
	{
		Analytics.CustomEvent("Lose", new Dictionary<string, object>
		{
			{"dungeon_leve", level },
			{"player_level", Player.Instance.level}
		});
		yield return StartCoroutine(textBox.Write("You died.\n" +
			"Your body will be carried to village.\n" +
			"See you soon.."
		));
		SceneManager.LoadScene("Village");
	}
	bool isComplete = false;
	IEnumerator GoDown()
	{
		SetCameraFadeColor (Color.black);
		Vector3 position = Camera.main.transform.position;
		iTween.CameraFadeTo(1.0f, 1.0f);
		iTween.MoveTo (Camera.main.gameObject, iTween.Hash(
			"x", current.stair.transform.position.x,
			"y", current.stair.transform.position.y,
			"z", current.stair.transform.position.z-0.5f,
			"time", 1.0f,
			"oncomplete", "OnComplete",
			"oncompletetarget", gameObject
		));
		yield return new WaitForSeconds (1.0f);
		while (false == isComplete) {
			yield return null;
		}
		level += 1;

		isComplete = false;
		Camera.main.transform.position = position;
	}
	void OnComplete()
	{
		isComplete = true;	
	}
	void SetCameraFadeColor(Color color)
	{
		Texture2D colorTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		colorTexture.SetPixel(0, 0, color);
		colorTexture.Apply();
		iTween.CameraFadeSwap (colorTexture);
	}
}
