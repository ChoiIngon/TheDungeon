using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResourceManager : Util.MonoSingleton<ResourceManager> {
    Dictionary<string, Object> resource;
	public delegate void OnLoadProgress(string bundleName, string assetName);
	public OnLoadProgress onLoadProgress;
	public AssetBundles.AssetBundleManager.OnDownloadProgress onDowonloadProgress {
		set {
			AssetBundles.AssetBundleManager.onDownloadProgress += value;
		}
		get {
			return AssetBundles.AssetBundleManager.onDownloadProgress;
		}
	}

    public IEnumerator Init()
	{
        if(null != resource)
        {
            yield break;
        }

		resource = new Dictionary<string, Object> ();	
		/*
        AssetBundles.AssetBundleManager.SetSourceAssetBundleURL(NetworkManager.Instance.host + "/AssetBundles/");
        var request = AssetBundles.AssetBundleManager.Initialize();
        if (request != null)
        {
            yield return StartCoroutine(request);

            string[] assetBundleNames = AssetBundles.AssetBundleManager.AssetBundleManifestObject.GetAllAssetBundles();
			foreach (string assetBundleName in assetBundleNames) {
				var assetLoadOperation = AssetBundles.AssetBundleManager.LoadAssetAsync (assetBundleName, "null", typeof(Object));
				if (null == assetLoadOperation) {
					yield break;
				}

				yield return StartCoroutine (assetLoadOperation);
			}
        }
		*/
    }

    public T Load<T>(string path) where T : Object
	{
		if (true == resource.ContainsKey (path))
		{
			return resource [path] as T;
		}

		T asset = Resources.Load<T>(path);
		resource[path] = asset ?? throw new System.Exception("can not find asset(path:" + path + ")");
		/*
		#if UNITY_EDITOR
		if (true == AssetBundles.AssetBundleManager.SimulateAssetBundleInEditor) {
			string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
			foreach (string assetBundleName in assetBundleNames) {
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName (assetBundleName, name);
				if (assetPaths.Length == 0) {
					continue;
				}
				asset = (T)AssetDatabase.LoadAssetAtPath (assetPaths [0], typeof(T));
				break;
			}
		} else
		#endif
		{
			string[] assetBundleNames = AssetBundles.AssetBundleManager.AssetBundleManifestObject.GetAllAssetBundles();
			foreach (string assetBundleName in assetBundleNames) {
				string error;
				AssetBundles.LoadedAssetBundle loadedAssetBundle = AssetBundles.AssetBundleManager.GetLoadedAssetBundle (assetBundleName, out error);
				asset = loadedAssetBundle.m_AssetBundle.LoadAsset<T> (name);
				if (null != asset) {
					break;
				}
			}
		}
		if (null != asset) {
			resource.Add (name, asset);
		}
		*/
		return asset;
	}
}
