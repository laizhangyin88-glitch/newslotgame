using System.Collections;
using UnityEngine;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Services;
using System.Collections.Generic;
using SlotMaker;

using static BagelCode.Common;

namespace BagelCode
{
    public class TriggerCondition
    {
        public Trigger[] triggers;
        public ConditionChecker<Trigger> conditionChecker;

        public bool isTrigger = false;

        public TriggerCondition(params Trigger[] _triggers)
            : this(TrueCase.ALL_TRUE, _triggers) { }

        public TriggerCondition(TrueCase trueCase, params Trigger[] _triggers)
        {
            conditionChecker = new ConditionChecker<Trigger>(trueCase);
            triggers = _triggers;
        }

        public void Initialize()
        {
            foreach (var t in triggers)
                t.Reset();
        }
    }

    public class WaitUntilTriggers : IEnumerator
    {
        private TriggerCondition[] conditions;

        public WaitUntilTriggers(params TriggerCondition[] _conditions)
        {
            conditions = _conditions;

            foreach (var c in conditions)
                foreach (var t in c.triggers)
                    c.conditionChecker.SetOrAddState(t, () => t.IsTrigger);
        }

        public object Current
        {
            get
            {
                // Call each frames
                foreach (var c in conditions)
                    foreach (var t in c.triggers)
                        t.Update();

                return null;
            }
        }

        public bool MoveNext()
        {
            // Returning false to exit coroutine
            return !conditions.Any(c => c.conditionChecker.State);
        }

        public void Reset() { }
    }

    public class WaitUntilTrigger : IEnumerator
    {
        private TriggerCondition condition;
        public bool ignoreLog = false;

        public WaitUntilTrigger(params Trigger[] _triggers)
            : this(TrueCase.ANY_TRUE, _triggers) { }

        public WaitUntilTrigger(TrueCase trueCase, params Trigger[] _triggers)
        {
            condition = new TriggerCondition(trueCase, _triggers);

            foreach (var t in condition.triggers)
                condition.conditionChecker.SetOrAddState(t, () => t.IsTrigger);
        }

        public object Current
        {
            get
            {
                // Call each frames
                foreach (var t in condition.triggers)
                    t.Update();

                return null;
            }
        }

        public bool MoveNext()
        {
#if UNITY_EDITOR
            PrintLogA();
            PrintLogB();
#endif
            // Returning false to exit coroutine
            return !condition.conditionChecker.State;
        }

        public void Reset() { }

        // For Debug
#if UNITY_EDITOR
        private bool isPrintA = false;
        private bool isPrintB = false;
        private static int colorIdx;
        private EColor color;
        private void PrintLogA()
        {
            if (ApplicationSettings.LogTest() && !isPrintA && !ignoreLog)
            {
                color = DEBUG_LOG_COLOR_SET.CircularIndexing(colorIdx++);
                string log = string.Join(", ", condition.triggers.Select(t => t.Log()));
                if (log != string.Empty)
                    Debug.Log(TextDecoUtils.GetColorFormatText("<WaitUntil> " + log, color));
                isPrintA = true;
            }
        }
        private void PrintLogB()
        {
            if (ApplicationSettings.LogTest() && condition.conditionChecker.State && !isPrintB && !ignoreLog)
            {
                string log = string.Join(", ", condition.triggers.Where(t => t.IsTrigger).Select(t => t.Log()));
                if (log != string.Empty)
                    Debug.Log(TextDecoUtils.GetColorFormatText("<ExitWith> " + log, color));
                isPrintB = true;
            }
        }

        ~WaitUntilTrigger()
        {
            if(ApplicationSettings.LogTest() && !isPrintB && !ignoreLog)
            {
                string log = "<ExitWith> unknown cause.";
                Debug.LogWarning(TextDecoUtils.GetColorFormatText(log, color));
            }
        }
#endif
    }

    public abstract class Trigger
    {
        public bool IsTrigger => isTrigger;
        protected bool isTrigger = false;

        public virtual void Update() { }

        public virtual void Reset() { }

        public virtual string Log() => string.Empty;
    }

    //

    public class EventsTrigger : Trigger
    {
        public string triggerEvent;

        private List<EventTrigger> triggerList = new List<EventTrigger>();
        private List<string> eventNames;

        public EventsTrigger(MonoBehaviour _listener, List<string> _eventNames)
            : this(GetOrAddMessageRouter(_listener), EventMonoBehaviour.ON_CUSTOM_EVENT, _eventNames) { }

        public EventsTrigger(MonoBehaviour _listener, string _type, List<string> _eventNames)
            : this(GetOrAddMessageRouter(_listener), _type, _eventNames) { }

        public EventsTrigger(GameObject _obj, string _type, List<string> _eventNames)
            : this(GetOrAddMessageRouter(_obj), _type, _eventNames) { }

        public EventsTrigger(GameObject _obj, List<string> _eventNames)
            : this(GetOrAddMessageRouter(_obj), MessageRouter.ON_CUSTOM_EVENT, _eventNames) { }

        public EventsTrigger(MessageRouter _listener, List<string> _eventNames)
            : this(_listener, MessageRouter.ON_CUSTOM_EVENT, _eventNames) { }

        private EventsTrigger(MessageRouter _listener, string _type, List<string> _eventNames)
        {
            eventNames = _eventNames;
            eventNames.ForEach(e =>
            {
                triggerList.Add(new EventTrigger(_listener, _type, e));
            });
        }

        ~EventsTrigger()
        {
            triggerList.Clear();
        }

        public override void Update()
        {
            triggerEvent = triggerList.FirstOrDefault(t => t.IsTrigger)?.EventData.name;
            isTrigger = triggerEvent != null;
        }

        public override void Reset()
        {
            isTrigger = false;
            foreach (var t in triggerList)
                t.Reset();
        }

        public override string Log() => triggerEvent ?? string.Join(", ", eventNames);
    }

    public class EventTrigger : Trigger
    {
        public EventData EventData { get; private set; } = null;

        private MessageDispatcher.EventDelegate eventDelegate;
        private string type;
        private string eventName;

        public EventTrigger(MonoBehaviour listener, string _type, string _eventName)
            : this(GetOrAddMessageRouter(listener), _type, _eventName) { }

        public EventTrigger(MonoBehaviour listener, string _eventName)
            : this(GetOrAddMessageRouter(listener), MessageRouter.ON_CUSTOM_EVENT, _eventName) { }

        public EventTrigger(GameObject listener, string _type, string _eventName)
            : this(GetOrAddMessageRouter(listener), _type, _eventName) { }

        public EventTrigger(GameObject listener, string _eventName)
            : this(GetOrAddMessageRouter(listener), MessageRouter.ON_CUSTOM_EVENT, _eventName) { }

        public EventTrigger(MessageRouter listener, string _eventName)
            : this(listener, EventMonoBehaviour.ON_CUSTOM_EVENT, _eventName) { }

        public EventTrigger(MessageRouter listener, string _type, string _eventName)
        {
            if(listener is null)
            {
                Debug.LogError("EventTrigger initializing failure. listener is null.");
                return;
            }

            type = _type;
            eventName = _eventName;

            eventDelegate = new MessageDispatcher.EventDelegate(TriggerEvent);
            MessageDispatcher.Register(type, eventDelegate);

            listener.RegisterCallback(type, (EventData eventData) => TriggerEvent(eventData));
        }

        ~EventTrigger()
        {
            MessageDispatcher.UnRegister(type, eventDelegate);
        }

        private void TriggerEvent(EventData eventData)
        {
            if (eventName == MetaEventDefine.ANY_EVENT_NAME || eventData.name == eventName)
            {
                isTrigger = true;
                EventData = eventData;
            }
        }

        public override void Reset()
        {
            isTrigger = false;
            EventData = null;
        }

        public override string Log() => eventName;
    }

    public class WaitUntilConditionTrigger : Trigger
    {
        private System.Func<bool> predicate;

        public WaitUntilConditionTrigger(System.Func<bool> _predicate)
        {
            predicate = _predicate;
        }

        public override void Update() => isTrigger = predicate.Invoke();

        public override void Reset() => isTrigger = false;

        public override string Log() => "Executed_Predicate";
    }

    public class WaitWhileConditionTrigger : Trigger
    {
        private System.Func<bool> predicate;

        public WaitWhileConditionTrigger(System.Func<bool> _predicate)
        {
            predicate = _predicate;
        }

        public override void Update() => isTrigger = !predicate.Invoke();

        public override void Reset() => isTrigger = false;

        public override string Log() => "Stoped_Predicate";
    }

    public class TimerTrigger : Trigger
    {
        private float startTime;
        private float time;

        public TimerTrigger(float _time)
        {
            time = _time;
            startTime = Time.time;
        }

        public override void Update()
        {
            float passedTime = Time.time - startTime;
            isTrigger = passedTime >= time;
        }

        public void Reset(float newTime)
        {
            time = newTime;
            isTrigger = false;
            startTime = Time.time;
        }

        public override void Reset()
        {
            isTrigger = false;
            startTime = Time.time;
        }

        public override string Log() => "Timeout";
    }
}
