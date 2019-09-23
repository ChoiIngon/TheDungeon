using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : Util.MonoSingleton<GameManager>
{
    public const string VERSION = "1.0";
	public Player player;
	public UIInventory ui_inventory;
	public UICoin ui_coin;
	public UINpc ui_npc;
	public UIDialogBox ui_dialogbox;
	public UITextBox ui_textbox;
	private Image camera_fade;
	private IEnumerator Start()
	{
		if("Common" == SceneManager.GetActiveScene().name)
		{
			yield return StartCoroutine(Init());
			AsyncOperation operation = SceneManager.LoadSceneAsync("Dungeon", LoadSceneMode.Additive);
			while (false == operation.isDone)
			{
				// loading progress
				yield return null;
			}
		}
	}
	
	public IEnumerator Init()
	{
		yield return ResourceManager.Instance.Init();
		DontDestroyOnLoad(ResourceManager.Instance.gameObject);
		DontDestroyOnLoad(gameObject);

		if (Application.platform != RuntimePlatform.Android)
		{
			Database.Connect(Database.Type.MetaData, Application.dataPath + "/meta_data.db");
		}
		else
		{
			string dbName = "meta_data.db";
			string sourceDBPath = Path.Combine(Application.streamingAssetsPath, dbName);
			string targetDBPath = Path.Combine(Application.persistentDataPath, dbName);

			Debug.Log("src:" + sourceDBPath + "(modify:" + File.GetLastWriteTime(sourceDBPath) + "), target:" + targetDBPath +")" + "(modify:" + File.GetLastWriteTime(targetDBPath) + ")");
			if (false == File.Exists(targetDBPath) || File.GetLastWriteTime(sourceDBPath) != File.GetLastWriteTime(targetDBPath))
			{
				UnityWebRequest request = new UnityWebRequest(Application.streamingAssetsPath + "/" + dbName);
				request.downloadHandler = new DownloadHandlerBuffer();
				yield return request.SendWebRequest();
				File.WriteAllBytes(targetDBPath, request.downloadHandler.data);
				Debug.Log("new version meta file(target path:" + targetDBPath + ")");
			}

			Database.Connect(Database.Type.MetaData, targetDBPath);
		}
		Database.Connect(Database.Type.UserData, Application.persistentDataPath + "/user_data.db");
		ItemManager.Instance.Init();
		MonsterManager.Instance.Init();
		AchieveManager.Instance.Init();

		player = new Player();
		player.Init();

		ui_dialogbox = UIUtil.FindChild<UIDialogBox>(transform, "UI/UIDialogBox");
		ui_dialogbox.Init();
		ui_inventory = UIUtil.FindChild<UIInventory>(transform, "UI/UIInventory");
		ui_inventory.Init();
		ui_coin = UIUtil.FindChild<UICoin>(transform, "UI/UICoin");
		ui_coin.Init();
		ui_npc = UIUtil.FindChild<UINpc>(transform, "UI/UINpc");
		ui_npc.Init();
		ui_textbox = UIUtil.FindChild<UITextBox>(transform, "UI/UITextBox");
		ui_textbox.Init();

		camera_fade = UIUtil.FindChild<Image>(transform, "UI/CameraFade");
		camera_fade.color = Color.black;

		for (int i = 0; i < 10; i++)
		{
			EquipItem item = ItemManager.Instance.CreateRandomEquipItem(150);
			player.inventory.Add(item);
		}

		PotionItem.Meta itemMeta = ItemManager.Instance.FindMeta<PotionItem.Meta>("ITEM_POTION_HEALING");
		player.inventory.Add(itemMeta.CreateInstance());
	}

	/*
		Instantly changes the amount(transparency) of a camera fade and then returns it back over time.
	*/
	public IEnumerator CameraFade(float from, float to, float time)
	{
		float amount = to - from;
		Color color = camera_fade.color;
		camera_fade.color = new Color(color.r, color.g, color.b, from);
		camera_fade.gameObject.SetActive(true);

		float delta = 0.0f;
		while (Mathf.Abs(delta) < Mathf.Abs(amount))
		{
			delta += amount * (Time.deltaTime / time);
			camera_fade.color = new Color(color.r, color.g, color.b, from + delta);
			yield return null;
		}
		camera_fade.color = new Color(color.r, color.g, color.b, to);
		camera_fade.gameObject.SetActive(false);
	}

	public IEnumerator CameraFade(Color from, Color to, float time)
	{
		Color amount = Color.white;
		amount.r = to.r - from.r;
		amount.g = to.g - from.g;
		amount.b = to.b - from.b;
		amount.a = to.a - from.a;

		Color color = camera_fade.color;
		camera_fade.color = from;
		camera_fade.gameObject.SetActive(true);

		Color delta = new Color(0.0f, 0.0f, 0.0f, 0.0f);
		while (Mathf.Abs(delta.r) < Mathf.Abs(amount.r) || Mathf.Abs(delta.g) < Mathf.Abs(amount.g) || Mathf.Abs(delta.b) < Mathf.Abs(amount.b) || Mathf.Abs(delta.a) < Mathf.Abs(amount.a))
		{
			delta.r += amount.r * (Time.deltaTime / time);
			delta.g += amount.g * (Time.deltaTime / time);
			delta.b += amount.b * (Time.deltaTime / time);
			delta.a += amount.a * (Time.deltaTime / time);
			camera_fade.color = new Color(from.r + delta.r, from.g + delta.g, from.b + delta.b, from.a + delta.a);
			yield return null;
		}
		camera_fade.color = to;
		camera_fade.gameObject.SetActive(false);
	}
}
 