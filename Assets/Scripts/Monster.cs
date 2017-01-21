using UnityEngine;
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
	[System.Serializable]
	public class Data
	{
		public float health;
	}

	public Info info;
	public Data data;
	public GameObject ui;

	public TrailRenderer trailPrefab;
	public GameObject damagePrefab;
	public Coin coinPrefab;
	public BloodMark bloodMarkPrefab;

	private Animator animator;
	private new SpriteRenderer renderer;
	private new Text name;
	private GaugeBar health;

	void Start () {
		animator = transform.FindChild ("Sprite").GetComponent<Animator> ();
		renderer = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ();
		name = ui.transform.FindChild ("Name").GetComponent<Text> ();
		health = ui.transform.FindChild ("Health").GetComponent<GaugeBar> ();
		gameObject.SetActive (false);
		{
			RectTransform rt = ui.transform.GetComponent<RectTransform> ();
			rt.position = Camera.main.WorldToScreenPoint (new Vector3 (transform.position.x, transform.position.y - 0.5f, 0.0f));
		}
		{
			RectTransform rt = name.gameObject.GetComponent<RectTransform> ();
			name.fontSize = (int)(rt.rect.height - name.lineSpacing);
		}
		ui.gameObject.SetActive (false);
	}

	public void Init(Info info_)
	{
		info = info_;
		name.text = info.name;
		renderer.sprite = info.sprite;

		data = new Data ();
		data.health = info.health;
		health.max = info.health;
		health.current = data.health;

		gameObject.SetActive (true);
		ui.gameObject.SetActive (true);
		StartCoroutine (Battle ());
	}

	public IEnumerator Battle()
	{
		yield return new WaitForSeconds (0.5f);
		while (0.0f < data.health) {
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
					Damage (10);
					yield return new WaitForSeconds (waitTime);
				}
			}
			else 
			{
				Attack ();
			}
			yield return new WaitForSeconds (80.0f / info.speed - waitTime);

		}

		ui.gameObject.SetActive (false);
		GameObject.Instantiate<GameObject> (damagePrefab);

		int coinCount = Random.Range (1, 25);
		for (int i = 0; i < coinCount; i++) {
			Coin coin = GameObject.Instantiate<Coin> (coinPrefab);
			float scale = Random.Range (1.0f, 1.5f);
			coin.transform.localScale = new Vector3 (scale, scale, 1.0f);
			coin.transform.position = transform.position;
			iTween.MoveBy (coin.gameObject, new Vector3 (Random.Range (-1.5f, 1.5f), Random.Range (0.0f, 0.5f), 0.0f), 0.5f);
		}
		gameObject.SetActive (false);
		UITextBox.Instance.text = "test test test test test test test test";
		TheDungeon.Controller.Instance.SetState (TheDungeon.Controller.State.Idle);
	}

	public void Attack()
	{
		iTween.CameraFadeFrom (0.1f, 0.1f);
		iTween.ShakePosition (Camera.main.gameObject, new Vector3 (0.3f, 0.3f, 0.0f), 0.1f);
		animator.SetTrigger ("Attack");
		BloodMark bloodMark = GameObject.Instantiate<BloodMark> (bloodMarkPrefab);
		bloodMark.transform.SetParent (Player.Instance.damage.transform);
		bloodMark.transform.position = new Vector3(
			Random.Range(Screen.width/2 - Screen.width /2 * 0.85f, Screen.width/2 + Screen.width/2 * 0.9f), 
			Random.Range(Screen.height/2 - Screen.height /2 * 0.85f, Screen.height/2 + Screen.height/2 * 0.9f),
			0.0f
		);
	}

	public void Damage(int damage)
	{
		TrailRenderer trail = GameObject.Instantiate<TrailRenderer> (trailPrefab);
		trail.sortingLayerName = "Effect";
		trail.sortingOrder = 1;

		data.health -= damage;
		if (0 > data.health) {
			data.health = 0;
		}
		health.current = data.health;

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

	public static Dictionary<string, Info> infos = new Dictionary<string, Info> ();
	public static void Init()
	{
		CSVReader reader = new CSVReader ("Config/info_monster");

		foreach (CSVReader.Row row in reader) {
			Info info = new Info ();
			info.id = row ["MONSTER_ID"];
			info.name = row ["MONSTER_NAME"];
			info.level = int.Parse (row ["MONSTER_LEVEL"]);
			info.health = int.Parse (row ["HEALTH"]);
			info.defense = float.Parse (row ["DEFENSE"]);
			info.speed = float.Parse (row ["SPEED"]);
			info.sprite = ResourceManager.Instance.Load<Sprite>(row ["SPRITE_PATH"]);
			info.reward.gold = int.Parse (row ["REWARD_GOLD"]);
			info.reward.exp = int.Parse (row ["REWARD_EXP"]);

			infos.Add (info.id, info);
		}
	}

	public static Info FindInfo(string id)
	{
		if (false == infos.ContainsKey (id)) {
			return null;
		}

		return infos[id];
	}
}
