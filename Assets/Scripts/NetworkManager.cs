	using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Util.Singleton<NetworkManager>
{
    public string host;
    public void Init()
    {
        TextAsset server = Resources.Load<TextAsset>("Config/ServerURL");
        host = server.text;
    }

	public IEnumerator HttpRequest(string path, System.Action<string> callback = null)
	{
		WWW www = new WWW(host + "/" + path);
		yield return www;
		if (null != callback) {
			callback (www.text);
		}
	}
}
