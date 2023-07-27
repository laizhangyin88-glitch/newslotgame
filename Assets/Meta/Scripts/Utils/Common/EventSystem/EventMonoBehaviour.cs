using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;
using SlotMaker;
using UnityEngine;

using EventNameType = System.Tuple<string, string>;

namespace BagelCode
{
    // Wrapping class of MonoBehaviour that can Register/Dispatch custom events
    public abstract partial class EventMonoBehaviour : MonoBehaviour
    {
        public const string ON_CUSTOM_EVENT = "OnCustomEvent";

        public delegate void EventDelegate(EventData eventData);

        public static Dictionary<string, List<EventMonoBehaviour>> listenerListDict = new Dictionary<string, List<EventMonoBehaviour>>();

        // Handling events that calls from MessageRouter,  MessageDispatcher.
        [SerializeField]
        private List<string> handleEventTypes;

        private MessageRouter router;
        public MessageRouter Router
        {
            get => InitRouter();
        }

        public bool isGraphOwner = false;

        private Dictionary<EventNameType, EventDelegate> events = new Dictionary<EventNameType, EventDelegate>();
        private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();

        //

        public void InitHandleEventType(bool forceReset = false)
        {
            if (forceReset || handleEventTypes == null)
                handleEventTypes = new List<string>() { ON_CUSTOM_EVENT };
            else if (!handleEventTypes.Contains(ON_CUSTOM_EVENT))
                handleEventTypes.Add(ON_CUSTOM_EVENT);
        }

        public void RegisterHandleEventType(string eventType)
        {
            if (!handleEventTypes.Contains(eventType))
            {
                Validate(eventType);

                handleEventTypes.Add(eventType);
                AssignHandleEvent(eventType);
            }
        }

        public void Dispatch(string eventType, EventData eventData)
        {
            var key = ValidateKey(eventType, eventData.name);
            if (key != null)
            {
                if (ApplicationSettings.LogTest())
                    Debug.Log("Invoke: " + string.Join(".", this.name, eventType, eventData.name, eventData.value));

                events[key]?.Invoke(eventData);
            }

            if (eventData.name != MetaEventDefine.ANY_EVENT_NAME)
            {
                var key2 = ValidateKey(eventType, MetaEventDefine.ANY_EVENT_NAME);
                if (key2 != null)
                {
                    if (ApplicationSettings.LogTest())
                        Debug.Log("Invoke: " + string.Join(".", this.name, eventType, eventData.name, eventData.value));

                    events[key2]?.Invoke(eventData);
                }
            }
        }

        public void Dispatch(string eventType, string eventName)
        {
            Dispatch(eventType, new EventData(eventName));
        }

        public void Dispatch(EventData eventData)
        {
            Dispatch(ON_CUSTOM_EVENT, eventData);
        }

        public void Dispatch(string eventName)
        {
            Dispatch(ON_CUSTOM_EVENT, eventName);
        }

        // If eventName is "ANY_EVENT_NAME", it can be dispatched from any event that matches the 'type'.
        public void Register(string eventType, string eventName, System.Action<EventData> action)
        {
            if (!handleEventTypes.Contains(eventType))
            {
                if (ApplicationSettings.LogTest())
                    Debug.LogWarning(string.Format("EventMonoBehaviour.Register warning. {0} is not in handleEventTypes.", eventType));
            }

            var key = new EventNameType(eventType, eventName);
            var del = new EventDelegate(action);

            if (events.ContainsKey(key)) events[key] += del;
            else events[key] = del;
        }

        public void Register(string eventType, string eventName, System.Action action)
        {
            Register(eventType, eventName, (EventData eventData) => action?.Invoke());
        }

        public void Register(string eventName, System.Action<EventData> action)
        {
            Register(ON_CUSTOM_EVENT, eventName, action);
        }

        public void Register(string eventName, System.Action action)
        {
            Register(ON_CUSTOM_EVENT, eventName, action);
        }

        public bool UnRegister(string eventType, string eventName)
        {
            return events.Remove(new EventNameType(eventType, eventName));
        }

        public bool UnRegister(string eventName)
        {
            return UnRegister(ON_CUSTOM_EVENT, eventName);
        }

        public void UnRegisterAll()
        {
            events.Clear();
        }

        //

        protected virtual void Awake()
        {
            InitRouter();
            InitHandleEventType();

            var graph = GetComponent<GraphOwner>();
            if (graph == null)
            {
                isGraphOwner = false;
            }
            else
            {
                isGraphOwner = graph.graph != null;
            }
        }

        protected virtual void OnEnable()
        {
            AssignHandleEvents();

            foreach (var handler in eventHandlerDict.Values)
                handler.EnableHandler();

            EnableEventScheduler();
        }

        protected virtual void OnDisable()
        {
            ClearHandleEvents();

            foreach (var handler in eventHandlerDict.Values)
                handler.DisableHandler();

            DisableEventScheduler();
        }

        protected MessageRouter InitRouter()
        {
            if (router is null) router = GetComponent<MessageRouter>();
            if (router is null) router = gameObject.AddComponent<MessageRouter>();
            return router;
        }

        //

        private MonoBehaviour GetEventDispatcherComponent(string type)
        {
            switch (type)
            {
                case MetaEventDefine.ON_META_UI_EVENT:
                    return GetComponent<MetaUIEventDispatcher>();
                case MetaEventDefine.ON_CREDIT_EVENT:
                    return GetComponent<CreditEventDispatcher>();
                case MetaEventDefine.ON_SYSTEM_EVENT:
                    return GetComponent<SystemEventDispatcher>();
                case MetaEventDefine.ON_LONG_POLL_EVENT:
                    return GetComponent<LongPollEventDispatcher>();
                case MetaEventDefine.ON_PASSIVE_EVENT:
                    return GetComponent<PassiveEventDispatcher>();
            }

            Debug.LogWarning(string.Format("EventMonoBehaviour.EventTypeToMessageDispatcherType failure. {0} is undefined Event Type.", type));
            return null;
        }

        private void Validate(string eventType)
        {
            var component = GetEventDispatcherComponent(eventType);
            if (component != null)
            {
                // EventMonoBehaviour 에 의해 처리되고 있는 이벤트를 포함하는 Dispatcher 자동 제거
                Debug.LogWarning(string.Format(
                    "{0} is already dispatched by <EventMonoBehaviour>. " +
                    "Then <{1}> was automatically destroyed.", eventType, component.GetType()));
                Destroy(component);
            }
        }

        private void AssignHandleEvents()
        {
            handleEventTypes.ForEach(e => AssignHandleEvent(e));
        }

        private void AssignHandleEvent(string eventType)
        {
            // Replace Message Dispatcher
            if (!delegates.ContainsKey(eventType))
            {
                var del = new MessageDispatcher.EventDelegate((EventData eventData) =>
                {
                    Dispatch(eventType, eventData); // to Script
                    Router.Dispatch(eventType, eventData); // to Graph
                });
                MessageDispatcher.Register(eventType, del);
                delegates.Add(eventType, del);
            }

            // Event Sender
            if (!listenerListDict.ContainsKey(eventType))
                listenerListDict.Add(eventType, new List<EventMonoBehaviour>());
            listenerListDict[eventType].Add(this);
        }

        private void ClearHandleEvents()
        {
            delegates.ToList().ForEach(p => MessageDispatcher.UnRegister(p.Key, p.Value));
            delegates.Clear();

            foreach (var handleEvent in handleEventTypes)
            {
                if (listenerListDict.ContainsKey(handleEvent))
                    listenerListDict[handleEvent].Remove(this);
            }
        }

        private EventNameType ValidateKey(string eventType, string eventName)
        {
            var key = new EventNameType(eventType, eventName);
            if (events.ContainsKey(key))
                return key;

            return null;
        }
    }
}
