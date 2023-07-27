using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{
    // EventMonoBehaviourScheduler는 원하는 함수를 일정 시간 이후에 호출하거나, 주기적으로 호출하는 기능을 제공한다.
    public abstract partial class EventMonoBehaviour : MonoBehaviour
    {
        private Dictionary<string, ScheduledEventInfo> scheduledEventDict = new Dictionary<string, ScheduledEventInfo>();

        public void RegisterSchedulingEvent(string key, System.Action action, float delay, bool repeat, bool triggerOnRegister = false)
        {
            UnRegisterSchedulingEvent(key);

            var value = new ScheduledEventInfo(this, action, delay, repeat, triggerOnRegister);
            scheduledEventDict.Add(key, value);
            value.coroutine = StartCoroutine(EventScheduleCoroutine(value));
        }

        public void RegisterSchedulingEvent(string key, System.Func<IEnumerator> action, float delay, bool repeat, bool triggerOnRegister = false)
        {
            UnRegisterSchedulingEvent(key);

            var value = new ScheduledEventInfo(this, action, delay, repeat, triggerOnRegister);
            scheduledEventDict.Add(key, value);
            value.coroutine = StartCoroutine(EventScheduleCoroutine(value));
        }

        public void UnRegisterSchedulingEvent(string key)
        {
            if (scheduledEventDict.ContainsKey(key))
            {
                if (scheduledEventDict[key].coroutine != null)
                    StopCoroutine(scheduledEventDict[key].coroutine);

                scheduledEventDict.Remove(key);
            }
        }

        public void UnRegisterSchedulingEventAll()
        {
            foreach(var key in scheduledEventDict.Keys)
            {
                UnRegisterSchedulingEvent(key);
            }

            scheduledEventDict.Clear();
        }

        public void EnableEventScheduler()
        {
            foreach (var info in scheduledEventDict.Values)
            {
                if (info.coroutine == null)
                {
                    info.coroutine = StartCoroutine(EventScheduleCoroutine(info));
                }
            }
        }

        public void DisableEventScheduler()
        {
            foreach (var info in scheduledEventDict.Values)
            {
                if (info.coroutine != null)
                {
                    StopCoroutine(info.coroutine);
                    info.coroutine = null;
                }
            }
        }

        //

        private IEnumerator EventScheduleCoroutine(ScheduledEventInfo eventInfo)
        {
            while (true)
            {
                if (eventInfo.Update())
                {
                    yield return StartCoroutine(eventInfo.OnTrigger());
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
