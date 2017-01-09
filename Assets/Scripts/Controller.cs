using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TheDungeon {
	public class Controller : MonoBehaviour {
		public const float DISTANCE = 6.4f; // 이지 사이즈가 1130 * 640 이라서...
		public const float speed = 12.8f;

		public Text text;
		public GameObject background;
		private Monster monster;
		private SpriteRenderer currentRenderer;
		private SpriteRenderer[] doorRenderer = new SpriteRenderer[Room.Max];
		Vector3 delta = Vector3.zero;

		// Use this for initialization
		void Start () {
			monster = background.transform.FindChild ("Current/Monster").GetComponent<Monster> ();
			currentRenderer = background.transform.FindChild ("Current").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.North] =  background.transform.FindChild ("North").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.East] =  background.transform.FindChild ("East").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.South] =  background.transform.FindChild ("South").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.West] =  background.transform.FindChild ("West").GetComponent<SpriteRenderer> ();

			ResourceManager.Instance.Init ();
			Monster.Init ();
			monster.gameObject.SetActive (false);
			TheDungeon.TouchPad touchPad = GetComponent<TheDungeon.TouchPad> ();

			touchPad.onTouchDrag += ((Vector3 delta) => {
				this.delta += delta;
			});

			touchPad.onTouchUp += (() => {
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

				delta = Vector3.zero;
				Debug.Log("touch up");
			}); 

			Map.Instance.Init ();
			MiniMap.Instance.Init ();

			currentRenderer.sprite = Map.Instance.current.sprite;
			for(int i=0; i<Room.Max; i++) {
				Room door = Map.Instance.current.doors [i];
				if (null != door) {
					doorRenderer [i].sprite = door.sprite;
				}
			}
		}
	
		// Update is called once per frame
		public IEnumerator Move(int direction)
		{
			Room room = Map.Instance.current;
			Room next = room.GetNext (direction);
			if (null == next) {
				yield break;
			}
			float movement = 0.0f;

			while (DISTANCE > movement) {
				movement += Time.deltaTime * speed;
				Vector3 position = background.transform.position;
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
				background.transform.position = position;
				yield return null;
			}

			background.transform.position = new Vector3 (0.0f, 0.0f, 0.0f);

			Room current = Map.Instance.Move (direction);
			MiniMap.Instance.CurrentPosition (current.id);

			currentRenderer.sprite = Map.Instance.current.sprite;
			for(int i=0; i<Room.Max; i++) {
				Room door = Map.Instance.current.doors [i];
				if (null != door) {
					doorRenderer [i].sprite = door.sprite;
				}
			
			}

			Monster.Info info = Monster.FindInfo ("DAEMON_001");
			monster.Init (info);
		}
	}
}