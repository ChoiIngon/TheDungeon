using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
	public const float DISTANCE = 7.2f; // 이지 사이즈가 1130 * 640 이라서...
	public const float MOVETIME = 0.3f;

	public bool enableInput {
		set {
			input.enabled = value;
			for (int i = 0; i < mainButtons.Length; i++) {
				if (null == mainButtons [i].button) {
					continue;
				}
				mainButtons [i].button.enabled = true;
			}
		}
	}

	[System.Serializable]
	public class UIMainButton
	{
		public string name;
		public Sprite sprite;
		public GameObject target;
		public Button button;

		public void Init()
		{
			if (null == button) {
				return;
			}
			button.transform.FindChild ("Text").GetComponent<Text> ().text = name;
			button.GetComponent<Image> ().sprite = sprite;
			button.onClick.AddListener (OnClick);
			if (null == target) {
				button.gameObject.SetActive (false);
			}
		}
		public void OnClick() {
			DungeonMain.Instance.enableInput = false;
			if(null == target)
			{
				return;
			}
			target.SetActive(true);
		}
	}

	public UIMainButton[] 	mainButtons;
	public UITextBox 		textBox;
	public UIDialogBox		dialogBox;
	public UICoin 			coin;
	public UIMiniMap		miniMap;

	public Monster monster;

	private Color cameraFadeColor;
	public Transform coins;
	private Transform rooms;
	private Room current;
	private Room[] next = new Room[Dungeon.Max];

	private TouchInput input;
	private Vector3 touchPoint = Vector3.zero;
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
		for (int i = 0; i < mainButtons.Length; i++) {
			UIMainButton mainButton = mainButtons [i];
			mainButton.Init ();
		}
		RectTransform uiMain = transform.FindChild ("UI/Main").GetComponent<RectTransform>();
		uiMain.position = Camera.main.WorldToScreenPoint(monster.transform.position);

		ResourceManager.Instance.Init ();
		ItemManager.Instance.Init ();
		QuestManager.Instance.Init ();
		MonsterManager.Instance.Init ();
		Player.Instance.Init ();
		Dungeon.Instance.Init ();
		iTween.CameraFadeAdd ();

		SetCameraFadeColor (Color.white);

		input = GetComponent<TouchInput> ();
		input.onTouchDown += (Vector3 position) => {
			touchPoint = position;
		};
		input.onTouchUp += (Vector3 position) => {
			float distance = Vector3.Distance(touchPoint, position);
			if(0.01f > distance)
			{
				return;
			}
			Vector3 delta = position - touchPoint;

			if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				if(0.0f == delta.x)
				{
					return;
				}
				if(0.0f > delta.x)
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
				if(0.0f == delta.y)
				{
					return;
				}

				if(0.0f > delta.y)
				{
					StartCoroutine(Move(Dungeon.North));
				}
				else
				{
					StartCoroutine(Move(Dungeon.South));
				}
			}

			touchPoint = Vector3.zero;
		};
		QuestManager.Instance.onComplete += (QuestData quest) =>
		{
			StartCoroutine(textBox.Write(
				quest.name + " is completed!!\n" +
				"strangth : +1"
			));
		};

		enableInput = true;

		InitRooms ();
		miniMap.Init ();
		miniMap.CurrentPosition (Dungeon.Instance.current.id);
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
	}
	IEnumerator Move(int direction)
	{
		enableInput = false;
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
			enableInput = true;
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
		miniMap.CurrentPosition (Dungeon.Instance.current.id);
		if (null != Dungeon.Instance.current.monster) {
			monster.Init (Dungeon.Instance.current.monster);
			yield return StartCoroutine (Battle ());
		} 

		if (Dungeon.Room.Type.Exit == Dungeon.Instance.current.type) {
			bool goDown = false;
			dialogBox.onSubmit += () =>  {
				goDown = true;
			};
			yield return StartCoroutine(dialogBox.Write("Do you want to go down the stair?"));
			if (true == goDown) {
				yield return StartCoroutine (GoDown ());
				Dungeon.Instance.Init ();
				InitRooms ();
				miniMap.Init ();
				miniMap.CurrentPosition (Dungeon.Instance.current.id);
				iTween.CameraFadeTo(0.0f, 1.0f);
				yield return new WaitForSeconds (1.0f);
				SetCameraFadeColor (Color.white);
			}
		}
		enableInput = true;
	}
	IEnumerator Battle()
	{
        yield return StartCoroutine(miniMap.Hide(1.0f));
		// attack per second
		float playerAPS = Player.Instance.stats.speed/monster.info.speed; 
		float monsterAPS = 1.0f;
		float playerTurn = playerAPS;
		float monsterTurn = monsterAPS;
		while (0.0f < monster.health.current && 0.0f < Player.Instance.health.current) {
			float waitTime = 0.0f;

			if (monsterTurn + Random.Range(0, monsterAPS * 0.3f) < playerTurn + Random.Range(0, playerAPS * 0.3f) ) {
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
				monsterTurn += monsterAPS;
			}
			else 
			{
				monster.Attack (Player.Instance);
				playerTurn += playerAPS;
			}
			yield return new WaitForSeconds (0.7f);
		}

		GameObject.Instantiate<GameObject> (monster.dieEffectPrefab);
		monster.gameObject.SetActive (false);
		
		yield return StartCoroutine (Win (monster.info));
        yield return StartCoroutine(miniMap.Show(0.5f));
		Dungeon.Instance.current.monster = null;
    }
	IEnumerator Win(Monster.Info info)
	{
		Player.Instance.AddCoin (info.reward.coin + (int)Random.Range(0, info.reward.coin * 0.1f));
		int level = Player.Instance.level;
		yield return StartCoroutine(Player.Instance.AddExp(info.reward.exp + (int)Random.Range(0, info.reward.exp * 0.1f)));
		yield return new WaitForSeconds (0.5f);
		yield return StartCoroutine(textBox.Write("You defeated \'" + info.name +"\'\n" +
			"Coins : +" + monster.info.reward.coin + "\n" +
			"Exp : +" + monster.info.reward.exp + "\n" +
			"Level : " + level + " -> " + Player.Instance.level + "\n" +
			"-str : +1\n" +
			"-spd : +1\n" +
			"-cri : +0.03\n"
		));
		QuestManager.Instance.Update ("KillMonster", info.id);
        Analytics.CustomEvent("KillMonster", new Dictionary<string, object>
        {
            {"monster_id", info.id }
        });
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
		// set the pixel values
		colorTexture.SetPixel(0, 0, color);
		// Apply all SetPixel calls
		colorTexture.Apply();
		iTween.CameraFadeSwap (colorTexture);
	}
}
