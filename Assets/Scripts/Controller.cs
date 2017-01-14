﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace TheDungeon {
	public class Controller : MonoBehaviour {
		public const float DISTANCE = 6.4f; // 이지 사이즈가 1130 * 640 이라서...
		public const float speed = 12.8f;

		public UITextBox textBox;
		public GameObject rooms;
		public Monster monster;
		public Texture2D fadeout;
		private SpriteRenderer currentRenderer;
		private SpriteRenderer[] doorRenderer = new SpriteRenderer[Room.Max];
		Vector3 touchPoint = Vector3.zero;

		// Use this for initialization
		void Start () {
			iTween.CameraFadeAdd (fadeout);

			currentRenderer = rooms.transform.FindChild ("Current").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.North] =  rooms.transform.FindChild ("North").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.East] =  rooms.transform.FindChild ("East").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.South] =  rooms.transform.FindChild ("South").GetComponent<SpriteRenderer> ();
			doorRenderer[Room.West] =  rooms.transform.FindChild ("West").GetComponent<SpriteRenderer> ();

			ResourceManager.Instance.Init ();
			Monster.Init ();
			monster.gameObject.SetActive (false);
			TheDungeon.TouchPad touchPad = GetComponent<TheDungeon.TouchPad> ();

			touchPad.onTouchDown += (Vector3 position) => {
				touchPoint = position;
			};

			touchPad.onTouchUp += (Vector3 position) => {
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

			Map.Instance.Init ();
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

			Room current = Map.Instance.Move (direction);

			currentRenderer.sprite = Map.Instance.current.sprite;
			for(int i=0; i<Room.Max; i++) {
				Room door = Map.Instance.current.doors [i];
				if (null != door) {
					doorRenderer [i].sprite = door.sprite;
				}
			}

			if (30 > Random.Range (0, 100)) {
				Monster.Info info = Monster.FindInfo ("DAEMON_001");
				monster.Init (info);
			}
		}
	}
}