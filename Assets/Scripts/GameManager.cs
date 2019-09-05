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
	private const string init_scene_name = "Village";
	private SceneMain current_scene;

	IEnumerator Start()
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync("Common", LoadSceneMode.Additive);
		while (false == operation.isDone)
		{
			// loading progress
			yield return null;
		}
		UIDialogBox.Instance.Init();
		UIDialogBox.Instance.Active("text");
        //UITicker.Instance.gameObject.SetActive(false);
        using (Database sqlite = new Database())
        {
            sqlite.Open(Application.dataPath + "/meta_data.db");
            sqlite.Execute("DELETE FROM meta_item_equip");
            sqlite.Execute("INSERT INTO meta_item_equip(item_id, item_name, item_type) VALUES('item_id_1', 'item_name', 1)");
            sqlite.Execute("INSERT INTO meta_item_equip(item_id, item_name, item_type) VALUES('item_id_2', 'item_name', 1)");
            sqlite.Execute("INSERT INTO meta_item_equip(item_id, item_name, item_type) VALUES('item_id_3', 'item_name', 1)");

            //Database.DataReader reader = sqlite.Execute("SELECT item_id, item_name, item_type FROM meta_item_equip");
            Database.DataReader reader = sqlite.Execute("SELECT item_id, item_name, item_type FROM meta_item_equip");
            while (true == reader.Read())
            {
                Debug.Log("item_id:" + reader.GetString("item_id") + ", item_name:" + reader.GetString("item_name") +", item_type:" + reader.GetInt32("item_type"));
            }
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