using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ResourceManager : MonoBehaviour {
    Dictionary<string, Object> resource;

    private static ResourceManager _instance;  
	public static ResourceManager Instance  
	{  
		get  
		{  
			if (!_instance) 
			{  
				_instance = (ResourceManager)GameObject.FindObjectOfType(typeof(ResourceManager));  
				if (!_instance)  
				{  
					GameObject container = new GameObject();  
					container.name = "ResourceManager";  
					_instance = container.AddComponent<ResourceManager>();  
					DontDestroyOnLoad (container);
				}  
			}  

			return _instance;  
		}  
	}
	public IEnumerator Init()
	{
        if(null != resource)
        {
            yield break;
        }

		resource = new Dictionary<string, Object> ();	
        AssetBundles.AssetBundleManager.SetSourceAssetBundleURL(NetworkManager.Instance.host + "/AssetBundles/");
        var request = AssetBundles.AssetBundleManager.Initialize();
        if (request != null)
        {
            yield return StartCoroutine(request);

            string[] assetBundleNames = AssetBundles.AssetBundleManager.AssetBundleManifestObject.GetAllAssetBundles();
            foreach(string assetBundleName in assetBundleNames) {
                var assetLoadOperation = AssetBundles.AssetBundleManager.LoadAssetAsync(assetBundleName, "null", typeof(Object));
                if(null == assetLoadOperation)
                {
                    yield break;
                }

                yield return StartCoroutine(assetLoadOperation);
                string error;

                AssetBundles.LoadedAssetBundle loadedAssetBundle = AssetBundles.AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out error);
                string[] assetNames = loadedAssetBundle.m_AssetBundle.GetAllAssetNames();
                foreach (string assetName in assetNames)
                {
                    AssetBundles.AssetBundleLoadAssetOperation operation = AssetBundles.AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(Object));
                    if (null == operation)
                        yield break;
                    yield return StartCoroutine(operation);
                    Object obj = operation.GetAsset<Object>();
                    if (!resource.ContainsKey(obj.name))
                    {
                        resource.Add(obj.name, obj);
                    }
                }
            }
        }
    }

    public T Load<T>(string name) where T : Object {
		if (true == resource.ContainsKey (name)) {
			return resource [name] as T;
		}
		return null;
	}
}
