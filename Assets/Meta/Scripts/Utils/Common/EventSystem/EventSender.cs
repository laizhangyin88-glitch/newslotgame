using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;
using SlotMaker;
using UnityEngine;
using System.Collections;

namespace BagelCode
{
    public static class EventSender
    {
        public const string ON_CUSTOM_EVENT = "OnCustomEvent";
        public const string ON_CALLEE_CALLBACK = "OnCalleeCallback";

        #region Default

        public static void SendEvent(GameObject receiver, string eventType, EventData eventData)
        {
            if (receiver == null)
            {
#if UNITY_EDITOR
                if (ApplicationSettings.LogTest())
                    Debug.LogWarning(string.Format("SendEvent.{0}.{1} failure. The receiver is null.", eventType, eventData.name));
#endif
                return;
            }

#if UNITY_EDITOR
            if (ApplicationSettings.LogTest())
                Debug.Log(string.Format("SendEvent \"{0}.{1}\" to {2}", eventType, eventData.name, receiver.name));
#endif

            // to Script
            var component = receiver.GetComponent<EventMonoBehaviour>();
            if (component != null)
                component.Dispatch(eventType, eventData);

            // to Graph
            var router = Common.GetOrAddMessageRouter(receiver);
            router.Dispatch(eventType, eventData);
        }

        public static void SendEvent(GameObject receiver, string eventType, EventData eventData, float delay)
        {
            if (delay > 0)
            {
#if UNITY_EDITOR
                if (receiver == null && ApplicationSettings.LogTest())
                {
                    Debug.LogWarning(string.Format("SendEvent.{0}.{1} failure. The receiver is null.", eventType, eventData.name));
                    return;
                }
#endif
                receiver.GetComponent<MonoBehaviour>().StartCoroutine(SendEventAfterDelayCoroutine(
                    receiver, eventType, eventData, delay));
            }
            else
            {
                SendEvent(receiver, eventType, eventData);
            }
        }

        public static void SendEvent(GameObject receiver, EventData eventData, float delay = 0f)
        {
            SendEvent(receiver, ON_CUSTOM_EVENT, eventData, delay);
        }

        public static void SendEvent(GameObject receiver, string eventName, float delay = 0f)
        {
            SendEvent(receiver, ON_CUSTOM_EVENT, eventName, delay);
        }

        public static void SendEvent(GameObject receiver, string type, string eventName, float delay = 0f)
        {
            SendEvent(receiver, type, new EventData(eventName), delay);
        }

        public static void SendEvent(GameObject receiver, string eventType, EventData eventData, int skipFrame)
        {
            if (skipFrame > 0)
            {
                receiver.GetComponent<MonoBehaviour>().StartCoroutine(SendEventSkipFrameCoroutine(
                    receiver, eventType, eventData, skipFrame));
            }
            else
            {
                SendEvent(receiver, eventType, eventData);
            }
        }

        public static void SendEvent(GameObject receiver, EventData eventData, int skipFrame)
        {
            SendEvent(receiver, ON_CUSTOM_EVENT, eventData, skipFrame);
        }

        public static void SendEvent(GameObject receiver, string eventName, int skipFrame)
        {
            SendEvent(receiver, ON_CUSTOM_EVENT, eventName, skipFrame);
        }

        public static void SendEvent(GameObject receiver, string type, string eventName, int skipFrame)
        {
            SendEvent(receiver, type, new EventData(eventName), skipFrame);
        }

        #endregion

        #region Callback

        public static void SendCalleeCallback(GameObject callee, string eventName = ON_CALLEE_CALLBACK)
        {
            var eventData = new EventData(eventName);
            SendCalleeCallback(callee, eventData);
        }

        public static void SendCalleeCallback<T>(GameObject callee, T eventDataValue, string eventName = ON_CALLEE_CALLBACK)
        {
            var eventData = new EventData<T>(eventName, eventDataValue);
            SendCalleeCallback(callee, eventData);
        }

        public static void SendCalleeCallback(GameObject callee, EventData eventData)
        {
            if (ValidateCaller(callee, out GameObject caller))
            {
                // to Script
                var component = caller.GetComponent<EventMonoBehaviour>();
                if (component != null)
                    component.Dispatch(ON_CUSTOM_EVENT, eventData);

                // to Graph
                MessageRouter router = Common.GetOrAddMessageRouter(caller);
                router.Dispatch(ON_CUSTOM_EVENT, eventData);
            }
        }

        public static void SendCalleeCallback<T>(GameObject callee, EventData<T> eventData)
        {
            if (ValidateCaller(callee, out GameObject caller))
            {
                // to Script
                var component = caller.GetComponent<EventMonoBehaviour>();
                if (component != null)
                    component.Dispatch(ON_CUSTOM_EVENT, eventData);

                // to Graph
                MessageRouter router = Common.GetOrAddMessageRouter(caller);
                router.Dispatch(ON_CUSTOM_EVENT, eventData);
            }
        }

        private static bool ValidateCaller(GameObject callee, out GameObject caller)
        {
            caller = null;

            var bb = callee.GetComponent<Blackboard>();
            if (bb == null)
            {
                Debug.LogWarning("EventSender.ValidateCaller return false. " + callee.name + " has not <Blackboard>.");
                return false;
            }

            caller = bb.GetVariable<GameObject>("caller")?.value;
            if (caller == null)
            {
                Debug.LogWarning("EventSender.ValidateCaller return false. " + callee.name + " has not caller.");
                return false;
            }

            return true;
        }

        #endregion

        // FSM/BT에서 호출된 Coroutine에서 종료 시점에 이벤트를 호출하는 경우 사용
        #region In Coroutine

        public static void SendEventInCoroutine(GameObject receiver, string eventName)
            => SendEventInCoroutine(receiver, ON_CUSTOM_EVENT, new EventData(eventName));

        public static void SendEventInCoroutine(GameObject receiver, EventData eventData)
            => SendEventInCoroutine(receiver, ON_CUSTOM_EVENT, eventData);

        public static void SendEventInCoroutine(GameObject receiver, string eventType, string eventName)
            => SendEventInCoroutine(receiver, eventType, new EventData(eventName));

        public static void SendEventInCoroutine(GameObject receiver, string eventType, EventData eventData)
        {
            SendEvent(receiver, eventType, eventData, 2);
        }

        #endregion

        #region Target

        public static void SendMetaUIEvent(GameObject receiver, string eventName)
        {
            SendEvent(receiver, MetaEventDefine.ON_META_UI_EVENT, new EventData(eventName));
        }

        public static void SendMetaUIEvent(GameObject receiver, EventData eventData)
        {
            SendEvent(receiver, MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        public static void SendGlobalMetaEvent(string eventName)
        {
            SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventName);
        }

        public static void SendGlobalMetaEvent(EventData eventData)
        {
            SendGlobalEvent(MetaEventDefine.ON_META_UI_EVENT, eventData);
        }

        #endregion

        #region Global

        public static void SendGlobalEvent(string eventType, EventData eventData)
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("EventSender.SendGlobalEvent: " + eventType + "." + eventData.name);

#if UNITY_EDITOR
                Debug.Log($"【 EventSender 发送消息】：eventName = {eventType} ， name = {eventData.name}  value = {eventData.value}");
#endif

            bool isCustomEvent = eventType == ON_CUSTOM_EVENT;
            if (isCustomEvent)
            {
                // To Graph
                Graph.SendGlobalEvent(eventData, null);

                // To EventMonoBehaviour
                if (EventMonoBehaviour.listenerListDict.ContainsKey(eventType))
                {
                    var listeners = EventMonoBehaviour.listenerListDict[eventType];
                    foreach (var listner in listeners)
                    {
                        listner.Dispatch(eventType, eventData);
                    }
                }
            }
            else
            {
                MessageDispatcher.Dispatch(eventType, eventData);
            }
        }

        public static void SendGlobalEvent(string eventType, string eventName)
        {
            SendGlobalEvent(eventType, new EventData(eventName));
        }

        public static void SendGlobalEvent(EventData eventData)
        {
            SendGlobalEvent(ON_CUSTOM_EVENT, eventData);
        }

        public static void SendGlobalEvent(string eventName)
        {
            SendGlobalEvent(ON_CUSTOM_EVENT, new EventData(eventName));
        }

#endregion

        private static IEnumerator SendEventSkipFrameCoroutine(GameObject receiver, string eventType, EventData eventData, int skipFrame)
        {
            for (int i = 0; i < skipFrame; ++i)
                yield return new WaitForEndOfFrame();

            SendEvent(receiver, eventType, eventData);
        }

        private static IEnumerator SendEventAfterDelayCoroutine(GameObject receiver, string eventType, EventData eventData, float delay)
        {
            yield return new WaitForSeconds(delay);

            SendEvent(receiver, eventType, eventData);
        }
    }
}
