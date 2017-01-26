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
                    if(null == buttons[i].ui)
                    {
                        continue;
                    }
					buttons [i].ui.enabled = true;
				}
			} else {
				input.enabled = false;
                

                for (int i = 0; i < buttons.Length; i++) {
                    if (null == buttons[i].ui)
                    {
                        continue;
                    }
                    buttons [i].ui.enabled = false;
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
		public Button ui;

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
	public UIInventory inventory;
	

	public GameObject rooms;
	public Monster monster;
	public Texture2D fadeout;

	private SpriteRenderer currentRenderer;
	private SpriteRenderer[] doorRenderer = new SpriteRenderer[Dungeon.Max];
	private TouchInput input;
    public UIButton[] buttons;
    private Vector3 touchPoint = Vector3.zero;

	// Use this for initialization
	void Start () {
		ResourceManager.Instance.Init ();
		ItemManager.Instance.Init ();
		Dungeon.Instance.Init ();
		Player.Instance.Init ();
		Monster.Init ();
		iTween.CameraFadeAdd (fadeout);

		for (int i = 0; i < buttons.Length; i++) {
			UIButton button = buttons [i];
            if(null == button.ui)
            {
                continue;
            }
			button.ui.transform.FindChild ("Text").GetComponent<Text>().text = button.name;
			button.ui.GetComponent<Image>().sprite = button.sprite;
            if(null == button.target)
            {
                button.ui.gameObject.SetActive(false);
            }
			button.ui.onClick.AddListener (button.OnClick);
		}

		currentRenderer = rooms.transform.FindChild ("Current").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.North] =  rooms.transform.FindChild ("North").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.East] =  rooms.transform.FindChild ("East").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.South] =  rooms.transform.FindChild ("South").GetComponent<SpriteRenderer> ();
		doorRenderer[Dungeon.West] =  rooms.transform.FindChild ("West").GetComponent<SpriteRenderer> ();

		monster.gameObject.SetActive (false);

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
		enableInput = true;
	}

	// Update is called once per frame
	public IEnumerator Move(int direction)
	{
		enableInput = false;
		Dungeon.Room room = Dungeon.Instance.current;
		Dungeon.Room next = room.GetNext (direction);
		if (null == next) {
            switch (direction)
            {
                case Dungeon.North:
                    iTween.PunchPosition(rooms.gameObject, new Vector3(0.0f, 0.0f, -1.0f), 0.5f);
                    break;
                case Dungeon.East:
                    iTween.PunchPosition(rooms.gameObject, new Vector3(-1.0f, 0.0f, 0.0f), 0.5f);
                    break;
                case Dungeon.South:
                    iTween.PunchPosition(rooms.gameObject, new Vector3(0.0f, 0.0f, 1.0f), 0.5f);
                    break;
                case Dungeon.West:
                    iTween.PunchPosition(rooms.gameObject, new Vector3(1.0f, 0.0f, 0.0f), 0.5f);
                    break;
                default:
                    break;
            }
            enableInput = true;
            yield break;
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
		
		if (null != Dungeon.Instance.current.monster) {
			monster.Init (Dungeon.Instance.current.monster);
		} else {
			enableInput = true;
		}
	}

}
