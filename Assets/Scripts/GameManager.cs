using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : Util.MonoSingleton<GameManager>
{
    public const string VERSION = "1.1";
	public Player player;
	public UIInventory ui_inventory;
	public UICoin ui_coin;
	public UINpc ui_npc;
	public UITextBox ui_textbox;
	public UITicker ui_ticker;
	public Advertisement advertisement;
	private Image camera_fade;
	public Canvas canvas;

	private IEnumerator Start()
	{
		if("Common" == SceneManager.GetActiveScene().name)
		{
			yield return StartCoroutine(Init());
			AsyncOperation operation = SceneManager.LoadSceneAsync("Start", LoadSceneMode.Additive);
			while (false == operation.isDone)
			{
				// loading progress
				yield return null;
			}
			SceneManager.SetActiveScene(SceneManager.GetSceneByName("Start"));
		}
	}
	
	public IEnumerator Init()
	{
		DontDestroyOnLoad(gameObject);

		canvas = UIUtil.FindChild<Canvas>(transform, "UI");
		camera_fade = UIUtil.FindChild<Image>(canvas.transform, "CameraFade");
		ui_inventory = UIUtil.FindChild<UIInventory>(canvas.transform, "UIInventory");
		ui_textbox = UIUtil.FindChild<UITextBox>(canvas.transform, "UITextBox");
		ui_coin = UIUtil.FindChild<UICoin>(canvas.transform, "UICoin");
		ui_npc = UIUtil.FindChild<UINpc>(canvas.transform, "UINpc");
		ui_ticker = UIUtil.FindChild<UITicker>(canvas.transform, "UITicker");
		camera_fade.gameObject.SetActive(true);
		camera_fade.color = Color.black;

		advertisement = GetComponent<Advertisement>();
		if (null == advertisement)
		{
			throw new MissingComponentException("Advertisement");
		}

		if (Application.platform != RuntimePlatform.Android)
		{
			Database.Connect(Database.Type.MetaData, Application.streamingAssetsPath + "/meta_data.db");
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
		ResourceManager.Instance.Init();
		ItemManager.Instance.Init();
		MonsterManager.Instance.Init();
		SkillManager.Instance.Init();
		AudioManager.Instance.Init();

		player = new Player();
		player.meta.Init();

		ui_inventory.gameObject.SetActive(true);
		ui_inventory.Init();
		ui_inventory.gameObject.SetActive(false);
		ui_coin.Init();
		ui_npc.Init();
		ui_textbox.Init();
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
 