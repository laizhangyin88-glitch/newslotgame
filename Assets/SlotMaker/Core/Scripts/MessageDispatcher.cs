using ParadoxNotion;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMaker
{
    public class MessageDispatcher : Singleton<MessageDispatcher>
    {
        public delegate void EventDelegate(EventData eventData);

        private Dictionary<string, EventDelegate> delegates = new Dictionary<string, EventDelegate>();

        public static void Register(string eventName, EventDelegate del)
        {
            EventDelegate tempDel;
            if (Instance.delegates.TryGetValue(eventName, out tempDel))
                Instance.delegates[eventName] = tempDel += del;
            else
                Instance.delegates[eventName] = del;
        }

        public static void UnRegister(string eventName, EventDelegate del)
        {
            EventDelegate tempDel;
            if (Instance.delegates.TryGetValue(eventName, out tempDel))
            {
                tempDel -= del;
                if (tempDel == null)
                    Instance.delegates.Remove(eventName);
                else
                    Instance.delegates[eventName] = tempDel;
            }
        }

        public static void Dispatch(string eventName, EventData eventData)
        {
#if UNITY_EDITOR
            if (eventData != null)
            {
                Debug.Log($"【 MessageDispatcher 发送消息】：eventName = {eventName} ， name = {eventData.name}  value = {eventData.value}");
            }
#endif
            EventDelegate del;
            if (Instance.delegates.TryGetValue(eventName, out del))
                del.Invoke(eventData);
        }
    }
}
