using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
    public string host;
    public void Init()
    {
        TextAsset server = Resources.Load<TextAsset>("Config/ServerURL");
        host = server.text;
    }

    public IEnumerator CreateItem(int level)
    {
        WWW www = new WWW(host + "/equipment_item.php?level=1");
        yield return www;
        Debug.Log(www.text);
    }
}
