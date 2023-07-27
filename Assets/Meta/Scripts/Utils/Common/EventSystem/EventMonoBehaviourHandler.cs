using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{
    // fsm을 코드화하기 위해 추가한 기능
    // 이벤트가 처리 중이면 (ienumerator 에서 빠져나오지 않은 상태이면) 추가로 들어오는 이벤트를 무시한다.
    public abstract partial class EventMonoBehaviour : MonoBehaviour
    {
        private Dictionary<string, PreemptiveEventHandler> eventHandlerDict = new Dictionary<string, PreemptiveEventHandler>();

        public const string DEFAULT_EVENT_HANDLER_NAME = "Main";

        public void RegisterHandlingEvent(Trigger trigger, System.Action action, string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetOrCreateEventHandler(handlerName)?.Register(trigger, action);
        }

        public void RegisterHandlingEvent(Trigger trigger, System.Action<Trigger> action, string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetOrCreateEventHandler(handlerName)?.Register(trigger, action);
        }

        public void RegisterHandlingEvent(Trigger trigger, System.Func<IEnumerator> action, string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetOrCreateEventHandler(handlerName)?.Register(trigger, action);
        }

        public void RegisterHandlingEvent(Trigger trigger, System.Func<Trigger, IEnumerator> action, string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetOrCreateEventHandler(handlerName)?.Register(trigger, action);
        }

        public bool UnRegisterHandlingEvent(Trigger trigger, string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            return GetEventHandler(handlerName)?.UnRegister(trigger) ?? false;
        }

        public void InitEventHandler(string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetEventHandler(handlerName)?.InitHandler();
        }

        public void UnRegisterHandlingEventAll()
        {
            foreach (var handler in eventHandlerDict.Values)
                handler?.ClearEvents();

            eventHandlerDict?.Clear();
        }

        public void ClearEventHandler(string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetEventHandler(handlerName)?.ClearEvents();
        }

        public void EnableHandler(string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetEventHandler(handlerName)?.EnableHandler();
        }

        public void DisableHandler(string handlerName = DEFAULT_EVENT_HANDLER_NAME)
        {
            GetEventHandler(handlerName)?.DisableHandler();
        }

        public void DisableAllHandler()
        {
            foreach (var handler in eventHandlerDict.Values)
                handler.DisableHandler();
        }

        //

        private PreemptiveEventHandler GetEventHandler(string handlerName)
        {
            if (string.IsNullOrEmpty(handlerName))
            {
                Debug.LogWarning("EventMonoBehaviour.GetEventHandler failure. handlerName is null of empty.");
                return null;
            }

            if (eventHandlerDict.ContainsKey(handlerName))
                return eventHandlerDict[handlerName];

            return null;
        }

        private PreemptiveEventHandler GetOrCreateEventHandler(string handlerName)
        {
            var handler = GetEventHandler(handlerName);

            if (handler == null) eventHandlerDict.Add(handlerName, new PreemptiveEventHandler(this));

            return eventHandlerDict[handlerName];
        }
    }
}
