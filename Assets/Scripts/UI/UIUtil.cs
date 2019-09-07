using System;
using UnityEngine;

public class UIUtil
{
	public static T FindChild<T>(Transform transform, string childName)
	{
		Transform child = transform.Find(childName);
		if (null == child)
		{
			throw new System.Exception("can not find child(name:" + childName + ")");

		}
		T ret = child.GetComponent<T>();
		if (null == ret)
		{
			throw new System.Exception("can not find component(type:" + typeof(T).Name + ")");
		}
		return ret;
	}
 
}
