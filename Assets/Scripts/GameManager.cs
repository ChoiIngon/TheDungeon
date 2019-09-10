using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Data;
using Mono.Data.Sqlite;

public class GameManager : Util.MonoSingleton<GameManager>
{
    public const string VERSION = "1.0";
	public Player player;
	public UIInventory ui_inventory;
	public UICoin ui_coin;
	public UINpc ui_npc;
	public UIDialogBox ui_dialogbox;
	public UITextBox ui_textbox;

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

		Database.Connect(Database.Type.MetaData, Application.dataPath + "/meta_data.db");
		Database.Connect(Database.Type.UserData, Application.persistentDataPath + "/user_data.db");
		ItemManager.Instance.Init();
		MonsterManager.Instance.Init();

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
	
		for (int i = 0; i < 10; i++)
		{
			EquipItem item = ItemManager.Instance.CreateRandomEquipItem(150);
			player.inventory.Add(item);
		}
	}

	void Update()
    {
    }

    [System.Serializable]
    public class SaveData
    {
        public string version;
    }

	public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerdata.dat");

        SaveData data = new SaveData();
        data.version = VERSION;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        Debug.Log("persistent data path:" + Application.persistentDataPath);
        if (false == File.Exists(Application.persistentDataPath + "/playerdata.dat"))
        {
            return;
        }

        FileStream file = File.Open(Application.persistentDataPath + "/playerdata.dat", FileMode.Open);
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            SaveData data = (SaveData)bf.Deserialize(file);
            Debug.Log("game version:" + data.version);
        }
        catch (System.InvalidCastException)
        {
            Debug.Log("invalid save data format");
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
        file.Close();
    }

    public void Quit()
    {
    }
}