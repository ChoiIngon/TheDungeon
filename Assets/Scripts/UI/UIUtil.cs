using System;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public static void AddPointerUpListener(GameObject obj, Action listener)
    {
        if(null == obj)
        {
            throw new System.Exception("AddPointerUpListener, game object is null");
        }
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if(null == trigger)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerUp;
        entry.callback.AddListener((data) => {
			if (null == listener)
			{
				throw new System.Exception("listener of " + obj.name + " is null");
			}
			listener();
		});
        trigger.triggers.Add(entry);
    }
}
