using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {
	public const float Size = 0.32f;
	public enum Type {
		Floor,
		Wall
	};
	public int index;
	public Type type;
	public Sprite sprite {
		set {
			SpriteRenderer renderer = GetComponent<SpriteRenderer>();
			renderer.sprite = value;
		}
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
