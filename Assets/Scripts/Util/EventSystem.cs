using UnityEngine;
using System.Collections.Generic;

namespace Util
{
    public class EventSystem : Singleton<EventSystem>
    {
        public static void Publish(string eventID, object eventParam = null)
        {
            EventSystem.Instance._Publish(eventID, eventParam);
        }

        public static void Subscribe<T>(string eventID, System.Action<T> handler) where T : new()
        {
            EventSystem.Instance._Subscribe<T>(eventID, handler);
        }

        public static void Subscribe(string eventID, System.Action handler) 
        {
            EventSystem.Instance._Subscribe(eventID, handler);
        }

        public static void Unsubscribe<T>(string eventID, System.Action<T> handler = null) where T : new()
        {
            EventSystem.Instance._Unsubscribe<T>(eventID, handler);
        }

        public static void Unsubscribe(string eventID, System.Action handler = null)
        {
            EventSystem.Instance._Unsubscribe(eventID, handler);
        }

        public abstract class IEventHandler
        {
            public abstract void OnEvent(object eventParam);
        }

        public class EventHandler<T> : IEventHandler where T : new()
        {
            public System.Action<T> onEvent;

            public override void OnEvent(object eventParam)
            {
                if (null == onEvent)
                {
                    throw new System.Exception("no event handler");
                }

                T param = (T)eventParam;
                onEvent(param);
            }
        }

        public class EventHandler : IEventHandler 
        {
            public System.Action onEvent;

            public override void OnEvent(object eventParam)
            {
                if (null == onEvent)
                {
                    throw new System.Exception("no event handler");
                }

                onEvent();
            }
        }

        private Dictionary<string, IEventHandler> event_handlers = new Dictionary<string, IEventHandler>();

        private void _Subscribe<T>(string eventID, System.Action<T> handler) where T : new()
        {
            EventHandler<T> eventHandler = null;
            if (true == event_handlers.ContainsKey(eventID))
            {
                try
                {
                    eventHandler = (EventHandler<T>)event_handlers[eventID];
                }
                catch(System.InvalidCastException)
                {
                    throw new System.Exception("try to subscribe same event(event_id:" + eventID + ") but different callback function proto type(System.Action<" + typeof(T).FullName + ">)");
                }
            }
            else
            {
                eventHandler = new EventHandler<T>();
                event_handlers.Add(eventID, eventHandler);
            }
            eventHandler.onEvent += handler;
        }

        private void _Subscribe(string eventID, System.Action handler)
        {
            EventHandler eventHandler = null;
            if (true == event_handlers.ContainsKey(eventID))
            {
                try
                { 
                    eventHandler = (EventHandler)event_handlers[eventID];
                }
                catch (System.InvalidCastException)
                {
                    throw new System.Exception("try to subscribe same event(event_id:" + eventID + ") but different callback function proto type(System.Action)");
                }
            }
            else
            {
                eventHandler = new EventHandler();
                event_handlers.Add(eventID, eventHandler);
            }
            eventHandler.onEvent += handler;
        }

        private void _Unsubscribe<T>(string eventID, System.Action<T> handler) where T : new()
        {
            if (false == event_handlers.ContainsKey(eventID))
            {
                Debug.LogWarning("can not find event key(event_id:" + eventID + ")");
                return;
            }

            EventHandler<T> eventHandler = (EventHandler<T>)event_handlers[eventID];
            if (null != handler)
            {
                eventHandler.onEvent -= handler;
            }
            else
            {
                eventHandler.onEvent = null;
            }

            if (null == eventHandler.onEvent)
            {
                event_handlers.Remove(eventID);
                Debug.Log("clear event handler container(event_id:" + eventID + ")");
            }
        }

        private void _Unsubscribe(string eventID, System.Action handler) 
        {
            if (false == event_handlers.ContainsKey(eventID))
            {
                Debug.LogWarning("can not find event key(event_id:" + eventID + ")");
                return;
            }

            EventHandler eventHandler = (EventHandler)event_handlers[eventID];
            if (null != handler)
            {
                eventHandler.onEvent -= handler;
            }
            else
            {
                eventHandler.onEvent = null;
            }

            if (null == eventHandler.onEvent)
            {
                event_handlers.Remove(eventID);
                Debug.Log("clear event handler container(event_id:" + eventID + ")");
            }
        }

        private void _Publish(string eventID, object eventParam)
        {
            if (false == event_handlers.ContainsKey(eventID))
            {
                Debug.LogWarning("can not find event key(event_id:" + eventID + ")");
                return;
            }

            event_handlers[eventID].OnEvent(eventParam);
        }
    }
}