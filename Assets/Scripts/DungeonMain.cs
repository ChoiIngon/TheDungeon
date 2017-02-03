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
			if (true == value) {
				input.enabled = true;
				for (int i = 0; i < buttons.Length; i++) {
					if (null == buttons [i].button) {
						continue;
					}
					buttons [i].button.enabled = true;
				}
			} else {
				input.enabled = false;
				for (int i = 0; i < buttons.Length; i++) {
					if (null == buttons [i].button) {
						continue;
					}
					buttons [i].button.enabled = false;
				}
			}
		}
	}

	[System.Serializable]
	public class UIButton
	{
		public string name;
		public Sprite sprite;

		public GameObject target;
		public Button button;

		public void OnClick() {
			DungeonMain.Instance.enableInput = false;
			if(null == target)
			{
				return;
			}
			target.SetActive(true);
		}
	}

	public UIButton[] 	buttons;
	public UITextBox 	textBox;
	public UIDialogBox	dialogBox;
	public UICoin 		coin;
	public UIMiniMap	miniMap;

	public Monster monster;
	public Texture2D fadeout;

	public GameObject rooms;

	private Room current;
	private Room[] next = new Room[Dungeon.Max];

	private GameObject reward;

	private TouchInput input;
	private Vector3 touchPoint = Vector3.zero;

	void Start () {
        Analytics.CustomEvent("DungeonMain", new Dictionary<string, object>
        {

        });
		ResourceManager.Instance.Init ();
		ItemManager.Instance.Init ();
		QuestManager.Instance.Init ();
		Monster.Init ();
		Dungeon.Instance.Init ();

		Player.Instance.Init ();

		iTween.CameraFadeAdd (fadeout);

		for (int i = 0; i < buttons.Length; i++) {
			UIButton button = buttons [i];
			if (null == button.button) {
				continue;
			}
			button.button.transform.FindChild ("Text").GetComponent<Text> ().text = button.name;
			button.button.GetComponent<Image> ().sprite = button.sprite;
			button.button.onClick.AddListener (button.OnClick);
			if (null == button.target) {
				button.button.gameObject.SetActive (false);
			}
		}

		current = rooms.transform.FindChild ("Current").GetComponent<Room> ();
		next [Dungeon.North] = rooms.transform.FindChild ("North").GetComponent<Room> ();
		next [Dungeon.North].transform.position = new Vector3 (0.0f, 0.0f, DISTANCE);
		next [Dungeon.East] =  rooms.transform.FindChild ("East").GetComponent<Room> ();
		next [Dungeon.East].transform.position = new Vector3 (DISTANCE, 0.0f, 0.0f);
		next [Dungeon.South] =  rooms.transform.FindChild ("South").GetComponent<Room> ();
		next [Dungeon.South].transform.position = new Vector3 (0.0f, 0.0f, -DISTANCE);
		next [Dungeon.West] =  rooms.transform.FindChild ("West").GetComponent<Room> ();
		next [Dungeon.West].transform.position = new Vector3 (-DISTANCE, 0.0f, 0.0f);

		RectTransform uiMain = transform.FindChild ("UI/Main").GetComponent<RectTransform>();
		uiMain.position = Camera.main.WorldToScreenPoint(monster.transform.position);
		input = GetComponent<TouchInput> ();
		input.onTouchDown += (Vector3 position) => {
			touchPoint = position;
		};
		input.onTouchUp += (Vector3 position) => {
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
		reward = new GameObject ();
		reward.transform.SetParent (transform);
		reward.name = "Reward";
		enableInput = true;

		InitRooms ();
		miniMap.Init ();
		miniMap.CurrentPosition (Dungeon.Instance.current.id);
	}
	private void InitRooms()
	{
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

		if(0 < reward.transform.childCount) {
			while (0 < reward.transform.childCount) {
				Coin coin = reward.transform.GetChild (0).GetComponent<Coin>();
				coin.transform.SetParent (null);
				coin.Stop ();
			}
			yield return new WaitForSeconds (0.1f);
		}
		float movement = 0.0f;

		float speed = DISTANCE / MOVETIME;
		while (DISTANCE > movement) {
			movement += Time.deltaTime * speed;
			Vector3 position = rooms.transform.position;
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
			rooms.transform.position = position;
			yield return null;
		}

		rooms.transform.position = Vector3.zero;
		
		Dungeon.Instance.Move (direction);

		InitRooms ();
		miniMap.CurrentPosition (Dungeon.Instance.current.id);
		if (null != Dungeon.Instance.current.monster) {
			monster.Init (Dungeon.Instance.current.monster);
			yield return StartCoroutine (Battle ());
		} 

		if (Dungeon.Room.Type.Exit == Dungeon.Instance.current.type) {
			dialogBox.onSubmit += () =>  {
				Dungeon.Instance.Init ();
				InitRooms ();
				miniMap.Init ();
				miniMap.CurrentPosition (Dungeon.Instance.current.id);
				Debug.Log("on submit");
			};
			yield return StartCoroutine(dialogBox.Write("Do you want go down the stair?"));
		}
		enableInput = true;
	}
	IEnumerator Battle()
	{
		while (0.0f < monster.health.current) {
			float waitTime = 0.0f;
			if (0 == Random.Range (0, 2)) {
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
					monster.Damage (30);
					yield return new WaitForSeconds (waitTime);
				}
			}
			else 
			{
				monster.Attack ();
			}
			yield return new WaitForSeconds (50.0f / monster.info.speed - waitTime);
		}

		Dungeon.Instance.current.monster = null;
		GameObject.Instantiate<GameObject> (monster.damagePrefab);
		monster.gameObject.SetActive (false);
		yield return StartCoroutine (Win (monster.info));
	}
	IEnumerator Win(Monster.Info info)
	{
		int coinCount = Random.Range (1, 25);
		for (int i = 0; i < coinCount; i++) {
			Coin coin = GameObject.Instantiate<Coin> (monster.coinPrefab);
			float scale = Random.Range (1.0f, 1.5f);
			coin.transform.SetParent (reward.transform);
			coin.transform.localScale = new Vector3 (scale, scale, 1.0f);
			coin.transform.position = monster.transform.position;
			iTween.MoveBy (coin.gameObject, new Vector3 (Random.Range (-1.5f, 1.5f), Random.Range (0.0f, 0.5f), 0.0f), 0.5f);
		}
		Vector3 position = Player.Instance.exp.GetComponent<RectTransform> ().position;
		Player.Instance.exp.GetComponent<RectTransform> ().position = Camera.main.WorldToScreenPoint (monster.transform.position);
		int level = (int)Player.Instance.exp.max;
		int exp = info.reward.exp + (int)Player.Instance.exp.current;
		Player.Instance.exp.current = 0;
		while (Player.Instance.exp.max < exp) {
			yield return StartCoroutine(Player.Instance.exp.DeferredChange(Player.Instance.exp.max, 0.25f));
			exp -= (int)Player.Instance.exp.max;
			Player.Instance.level += 1;
			Player.Instance.exp.max = Player.Instance.level;
			Player.Instance.exp.current = 0.0f;
		}
		yield return StartCoroutine(Player.Instance.exp.DeferredChange (exp, 0.1f));
		yield return StartCoroutine(textBox.Write("You defeated \'daemon\'\n" +
			"Coins : +" + monster.info.reward.gold + "\n" +
			"Exp : +" + monster.info.reward.exp + "\n" +
			"Level : " + level + " -> " + Player.Instance.exp.max
		));
		Player.Instance.exp.GetComponent<RectTransform> ().position = position;
		QuestManager.Instance.Update ("KillMonster", info.id);

        Analytics.CustomEvent("KillMonster", new Dictionary<string, object>
        {
            {"monster_id", info.id }
        });
    }
}
