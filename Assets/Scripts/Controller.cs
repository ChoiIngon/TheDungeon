using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
	public enum State
	{
		Idle,
		Popup,
		Battle,
		Move
	}
	[System.Serializable]
	public class PlayerButton
	{
		public string name;
		public Button button;
		public Sprite sprite;
		public GameObject ui;
	}
	private static Controller _instance;  
	public static Controller Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (Controller)GameObject.FindObjectOfType(typeof(Controller));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "Controller";  
					_instance = container.AddComponent<Controller>();  
				}  
			}  
			return _instance;  
		}  
	}

	public const float DISTANCE = 6.4f; // 이지 사이즈가 1130 * 640 이라서...
	public const float speed = 12.8f;

	public UITextBox textBox;
	public PlayerButton[] buttons;

	public GameObject rooms;
	public Monster monster;
	public Texture2D fadeout;
	public UIInventory inventory;
	private SpriteRenderer currentRenderer;
	private SpriteRenderer[] doorRenderer = new SpriteRenderer[Room.Max];
	public TouchInput input;
	private Vector3 touchPoint = Vector3.zero;
	private State state;
	// Use this for initialization
	void Start () {
		ResourceManager.Instance.Init ();
		ItemManager.Instance.Init ();
		Map.Instance.Init ();
		Player.Instance.Init ();
		Monster.Init ();
		iTween.CameraFadeAdd (fadeout);

		for (int i = 0; i < buttons.Length; i++) {
			int index = i;
			PlayerButton button = buttons [index];
			Text name = button.button.transform.FindChild ("Text").GetComponent<Text>();
			name.text = button.name;
			Image image = button.button.GetComponent<Image>();
			image.sprite = button.sprite;
			button.button.onClick.AddListener (() => {
				if(null == buttons[index].ui)
				{
					return;
				}
				buttons[index].ui.SetActive(true);
			});
		}
		currentRenderer = rooms.transform.FindChild ("Current").GetComponent<SpriteRenderer> ();
		doorRenderer[Room.North] =  rooms.transform.FindChild ("North").GetComponent<SpriteRenderer> ();
		doorRenderer[Room.East] =  rooms.transform.FindChild ("East").GetComponent<SpriteRenderer> ();
		doorRenderer[Room.South] =  rooms.transform.FindChild ("South").GetComponent<SpriteRenderer> ();
		doorRenderer[Room.West] =  rooms.transform.FindChild ("West").GetComponent<SpriteRenderer> ();

		monster.gameObject.SetActive (false);

		currentRenderer.sprite = Map.Instance.current.sprite;
		for(int i=0; i<Room.Max; i++) {
			Room door = Map.Instance.current.next [i];
			if (null != door) {
				doorRenderer [i].sprite = door.sprite;
			}
		}

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
					StartCoroutine(Move(Room.East));
				}
				else
				{
					StartCoroutine(Move(Room.West));
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
					StartCoroutine(Move(Room.North));
				}
				else
				{
					StartCoroutine(Move(Room.South));
				}
			}

			touchPoint = Vector3.zero;
		};
		SetState (State.Idle);
	}

	// Update is called once per frame
	public IEnumerator Move(int direction)
	{
		SetState (State.Move);
		Room room = Map.Instance.current;
		Room next = room.GetNext (direction);
		if (null == next) {
			SetState (State.Idle);
			yield break;
		}
		float movement = 0.0f;

		while (DISTANCE > movement) {
			movement += Time.deltaTime * speed;
			Vector3 position = rooms.transform.position;
			switch (direction) {
			case Room.North :
				position = new Vector3 (position.x, position.y, position.z - Time.deltaTime * speed);
				break;
			case Room.East :
				position = new Vector3 (position.x - Time.deltaTime * speed, position.y, position.z);
				break;
			case Room.South :
				position = new Vector3 (position.x, position.y, position.z + Time.deltaTime * speed);
				break;
			case Room.West:
				position = new Vector3 (position.x + Time.deltaTime * speed, position.y, position.z);
				break;
			default :
				break;
			}
			rooms.transform.position = position;
			yield return null;
		}

		rooms.transform.localPosition = Vector3.zero;

		Map.Instance.Move (direction);

		currentRenderer.sprite = Map.Instance.current.sprite;
		for(int i=0; i<Room.Max; i++) {
			Room door = Map.Instance.current.next [i];
			if (null != door) {
				doorRenderer [i].sprite = door.sprite;
			}
		}

		if (100 > Random.Range (0, 100)) {
			Monster.Info info = Monster.FindInfo ("DAEMON_001");
			monster.Init (info);
		} else {
			SetState (State.Idle);
		}
	}
	public void SetState(State state)
	{
		if (this.state == state) {
			return;
		}
		switch (state) {
		case State.Idle:
			input.enabled = true;
			for (int i = 0; i < buttons.Length; i++) {
				buttons [i].button.enabled = true;
			}
			break;
		case State.Battle:
		case State.Popup:
		case State.Move :
			input.enabled = false;
			for (int i = 0; i < buttons.Length; i++) {
				buttons [i].button.enabled = false;
			}
			break;
		}
		this.state = state;
	}
}
