using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode/Utils")]
    public class SendEventOnlyOnce : ActionTask<Blackboard>
    {
        public static List<string> recordEvent = new List<string>();

        public static bool IsSended(string userAccount, string eventType, string eventName)
        {
            return recordEvent.Contains($"{userAccount}+{eventType}+{eventName}");
        }
        public static void Record(string userAccount, string eventType, string eventName)
        {
            string key = $"{userAccount}+{eventType}+{eventName}";
            if (recordEvent.Contains(key))
                return;

            recordEvent.Add(key);
        }

        public BBParameter<string> eventType;
        public BBParameter<string> eventName;
        public BBParameter<bool> sendGlobal;
        public BBParameter<float> delay;

        protected override string info
        {
            get { return "程序生命周期内每个账号只会发送一次 " + (sendGlobal.value ? "Global " : "") + "Event Meta [" + eventType.value + "." + eventName.value + "]"; }
        }

        protected override void OnExecute()
        {
            if (IsSended(NetData_Login.Instance.UserAccount, eventType.value, eventName.value))
            {
                EndAction();
                return;
            }

            Record(NetData_Login.Instance.UserAccount, eventType.value, eventName.value);

            if (delay.value > 0f)
            {
                StartCoroutine(SendEventWithDelayCoroutine());
            }
            else
            {
                SendEvent();
            }

            EndAction();
        }

        private IEnumerator SendEventWithDelayCoroutine()
        {
            yield return new WaitForSeconds(delay.value);

            SendEvent();
        }

        private void SendEvent()
        {
            if (sendGlobal.value)
            {
                EventSender.SendGlobalEvent(eventType.value, new EventData(eventName.value));
            }
            else
            {
                if (agent != null && agent.gameObject != null)
                {
                    EventSender.SendEvent(agent.gameObject, eventType.value, new EventData(eventName.value));
                }
#if DEV
				else if (ApplicationSettings.LogTest())
				{
					Debug.Log(string.Format("SendEventMeta failure: {0}.{1}", eventType, eventName));
				}
#endif
            }

            EndAction();
        }
    }

    [Category("★ BagelCode/Utils")]
    public class SendEventOnlyOnce<T> : ActionTask<Blackboard>
    {
        public BBParameter<string> eventType;
        public BBParameter<T> eventValue;
        public BBParameter<string> eventName;
        public BBParameter<bool> sendGlobal;
        public BBParameter<float> delay;

        protected override string info
        {
            get { return "程序生命周期内每个账号只会发送一次 " + (sendGlobal.value ? "Global " : "") + "Event Meta [" + eventType.value + "." + eventName.value + "]"; }
        }

        protected override void OnExecute()
        {
            if (SendEventOnlyOnce.IsSended(NetData_Login.Instance.UserAccount, eventType.value, eventName.value))
            {
                EndAction();
                return;
            }

            SendEventOnlyOnce.Record(NetData_Login.Instance.UserAccount, eventType.value, eventName.value);

            if (delay.value > 0f)
            {
                StartCoroutine(SendEventWithDelayCoroutine());
            }
            else
            {
                SendEvent();
            }

            EndAction();
        }

        private IEnumerator SendEventWithDelayCoroutine()
        {
            yield return new WaitForSeconds(delay.value);

            SendEvent();
        }

        private void SendEvent()
        {
            if (sendGlobal.value)
            {
                var eventData = new EventData<T>(eventName.value, eventValue.value);
                EventSender.SendGlobalEvent(eventType.value, eventData);
            }
            else
            {
                if (agent != null && agent.gameObject != null)
                {
                    var eventData = new EventData<T>(eventName.value, eventValue.value);
                    EventSender.SendEvent(agent.gameObject, eventType.value, eventData);
                }
#if DEV
                else if (ApplicationSettings.LogTest())
                {
                    Debug.Log(string.Format("SendEventMeta failure: {0}.{1}", eventType, eventName));
                }
#endif
            }
        }
    }
}
