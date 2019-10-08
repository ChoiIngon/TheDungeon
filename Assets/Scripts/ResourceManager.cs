using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResourceManager : Util.Singleton<ResourceManager>
{
    Dictionary<string, Object> resource;
	public delegate void OnLoadProgress(string bundleName, string assetName);
	public OnLoadProgress onLoadProgress;
	
    public void Init()
	{
		resource = new Dictionary<string, Object> ();	
    }

    public T Load<T>(string path) where T : Object
	{
		if (true == resource.ContainsKey (path))
		{
			return resource [path] as T;
		}

		T asset = Resources.Load<T>(path);
		resource[path] = asset ?? throw new System.Exception("can not find asset(path:" + path + ")");
		return asset;
	}
}
