﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Monster : MonoBehaviour {

	[System.Serializable]
	public class Info
	{
		[System.Serializable]
		public class Reward
		{
			public int gold;
			public int exp;
		}
		public string id;
		public string name;
		public int level;
		public int health;
		public float defense;
		public float attack;
		public float speed;
		public Sprite sprite;
		public Reward reward;

		public Info()
		{
			reward = new Reward();
		}
	}
	
	public Info info;
	public GameObject ui;

	public TrailRenderer trailPrefab;
	public GameObject damagePrefab;
	public Coin coinPrefab;
	public BloodMark bloodMarkPrefab;

	private Animator animator;
	private new SpriteRenderer renderer;
	private new Text name;
	public GaugeBar health;

	void Start () {
		animator = transform.FindChild ("Sprite").GetComponent<Animator> ();
		renderer = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ();
		name = ui.transform.FindChild ("Name").GetComponent<Text> ();
		health = ui.transform.FindChild ("Health").GetComponent<GaugeBar> ();
		gameObject.SetActive (false);
		/*
		{
			RectTransform rt = ui.transform.GetComponent<RectTransform> ();
			rt.position = Camera.main.WorldToScreenPoint (new Vector3 (transform.position.x, transform.position.y - 0.5f, 0.0f));
		}
		{
			RectTransform rt = name.gameObject.GetComponent<RectTransform> ();
			name.fontSize = (int)(rt.rect.height - name.lineSpacing);
		}
		*/
		ui.gameObject.SetActive (false);
	}

	public void Init(Info info_)
	{
		gameObject.SetActive (true);
		ui.gameObject.SetActive (true);

		info = info_;
		name.text = info.name;
		renderer.sprite = info.sprite;

		health.max = info.health;
		health.current = health.max;
	}

	public void OnDisable()
	{
		ui.gameObject.SetActive (false);
	}
	 
	public void Attack()
	{
		iTween.CameraFadeFrom (0.1f, 0.1f);
		iTween.ShakePosition (Camera.main.gameObject, new Vector3 (0.3f, 0.3f, 0.0f), 0.2f);
		animator.SetTrigger ("Attack");
		BloodMark bloodMark = GameObject.Instantiate<BloodMark> (bloodMarkPrefab);
		bloodMark.transform.SetParent (Player.Instance.damage.transform, false);
		bloodMark.transform.position = new Vector3(
			Random.Range(Screen.width/2 - Screen.width /2 * 0.85f, Screen.width/2 + Screen.width/2 * 0.9f), 
			Random.Range(Screen.height/2 - Screen.height /2 * 0.85f, Screen.height/2 + Screen.height/2 * 0.9f),
			0.0f
		);
	}

	public void Damage(int damage)
	{
		StartCoroutine(health.DeferredChange(-damage, 0.2f));

        TrailRenderer trail = GameObject.Instantiate<TrailRenderer> (trailPrefab);
		trail.sortingLayerName = "Effect";
		trail.sortingOrder = 1;

		const float length = 4.0f;
		Vector3 direction = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), 0.0f);
		direction = length * direction.normalized;

		Vector3 start = -1.0f * direction;
		start.y += 0.5f;
		direction.y += 0.5f;
		trail.transform.localPosition = start;
		GameObject effect = GameObject.Instantiate<GameObject> (damagePrefab);
		effect.transform.position = Vector3.Lerp(start, direction, Random.Range(0.4f, 0.6f));
		MeshRenderer renderer = effect.transform.Find ("Text").GetComponent<MeshRenderer> ();
		renderer.sortingLayerName = "Effect";

		TextMesh text = effect.transform.FindChild ("Text").GetComponent<TextMesh> ();
		text.text = "-" + damage.ToString ();
		iTween.MoveTo(trail.gameObject, direction, 0.3f);
		iTween.ShakePosition (gameObject, new Vector3 (0.3f, 0.3f, 0.0f), 0.1f);
	}

	public void Dead()
	{
		int coinCount = info.reward.gold;
		int amount = 1;
		float scale = 1.0f;
		while (0 < coinCount) {
			int count = Random.Range (1, 10);
			for (int i = 0; i < count; i++) {
				Coin coin = GameObject.Instantiate<Coin> (coinPrefab);
				coin.amount = Mathf.Min(amount, coinCount);
				coin.transform.SetParent (DungeonMain.Instance.coins);
				coin.transform.localScale = new Vector3 (scale, scale, 1.0f);
				coin.transform.position = transform.position;
				iTween.MoveBy (coin.gameObject, new Vector3 (Random.Range (-1.5f, 1.5f), Random.Range (0.0f, 0.5f), 0.0f), 0.5f);
				coinCount -= amount;
				if (0 >= coinCount) {
					return;
				}
			}
			amount *= 10;
			scale += 0.1f;
		}
	}
}
