using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	public const float DISTANCE = 6.4f; // 이지 사이즈가 1130 * 640 이라서...
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

	public UITextBox textBox;
	//public UIInventory inventory;
	public UIButton[] buttons;

	public GameObject rooms;
	public GameObject coins;
	public Monster monster;
	public Texture2D fadeout;

	private SpriteRenderer currentRenderer;
	private SpriteRenderer[] doorRenderer = new SpriteRenderer[Dungeon.Max];
	public TouchInput input;
	private Vector3 touchPoint = Vector3.zero;

	// Use this for initialization
	void Start () {
		ResourceManager.Instance.Init ();
		ItemManager.Instance.Init ();
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

		currentRenderer = rooms.transform.FindChild ("Current").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.North] =  rooms.transform.FindChild ("North").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.East] =  rooms.transform.FindChild ("East").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.South] =  rooms.transform.FindChild ("South").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.West] =  rooms.transform.FindChild ("West").GetComponent<SpriteRenderer> ();

		/*
		currentRenderer.sprite = Dungeon.Instance.current.sprite;
		for(int i=0; i<Room.Max; i++) {
			Dungeon.Room door = Dungeon.Instance.current.next [i];
			if (null != door) {
				doorRenderer [i].sprite = door.sprite;
			}
		}
		*/
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
		coins = new GameObject ();
		enableInput = true;
	}

	// Update is called once per frame
	public IEnumerator Move(int direction)
	{
		Dungeon.Room room = Dungeon.Instance.current;
		Dungeon.Room next = room.GetNext (direction);
		if (null == next) {
			enableInput = true;
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
			yield break;
		} 
		/*
		else {
			enableInput = false;
			switch (direction) {
			case Dungeon.North:
				iTween.MoveTo (rooms.gameObject, new Vector3 (0.0f, 0.0f, -DISTANCE), 0.5f);
				break;
			case Dungeon.East:
				iTween.MoveTo (rooms.gameObject, new Vector3 (-DISTANCE, 0.0f, 0.0f), 0.5f);
				break;
			case Dungeon.South:
				iTween.MoveTo (rooms.gameObject, new Vector3 (0.0f, 0.0f, DISTANCE), 0.5f);
				break;
			case Dungeon.West:
				iTween.MoveTo (rooms.gameObject, new Vector3 (DISTANCE, 0.0f, 0.5f), 0.5f);
				break;
			default :
				break;
			}
			yield return new WaitForSeconds (0.5f);
			enableInput = true;
		}
		*/
		if(0 < coins.transform.childCount) {
			while (0 < coins.transform.childCount) {
				Coin coin = coins.transform.GetChild (0).GetComponent<Coin>();
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

		rooms.transform.localPosition = Vector3.zero;
		
		Dungeon.Instance.Move (direction);
		/*
		currentRenderer.sprite = Map.Instance.current.sprite;
		for(int i=0; i<Room.Max; i++) {
			Room door = Map.Instance.current.next [i];
			if (null != door) {
				doorRenderer [i].sprite = door.sprite;
			}
		}
*/
		if (null != Dungeon.Instance.current.monster) {
			monster.Init (Dungeon.Instance.current.monster);
			StartCoroutine (Battle ());
		} 
	}

	public IEnumerator Battle()
	{
		DungeonMain.Instance.enableInput = false;
		yield return new WaitForSeconds (0.5f);
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

		int coinCount = Random.Range (1, 25);
		for (int i = 0; i < coinCount; i++) {
			Coin coin = GameObject.Instantiate<Coin> (monster.coinPrefab);
			float scale = Random.Range (1.0f, 1.5f);
			coin.transform.SetParent (coins.transform);
			coin.transform.localScale = new Vector3 (scale, scale, 1.0f);
			coin.transform.position = monster.transform.position;
			iTween.MoveBy (coin.gameObject, new Vector3 (Random.Range (-1.5f, 1.5f), Random.Range (0.0f, 0.5f), 0.0f), 0.5f);
		}
		monster.gameObject.SetActive (false);
		DungeonMain.Instance.enableInput = true;

		UITextBox.Instance.text = "test test test test test test test test";
	}
}
