using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;

namespace BagelCode.Task.Actions
{
    // todo shk agent blackboard 대신 gameobject 사용하도록 수정
    [Category("★ BagelCode/Utils")]
    public class SendEventMeta : ActionTask<Blackboard>
	{
		public BBParameter<string> eventType;
		public BBParameter<string> eventName;
		public BBParameter<bool> sendGlobal;
        public BBParameter<float> delay;

        protected override string info
		{
			get { return "Send " + (sendGlobal.value ? "Global " : "") + "Event Meta [" + eventType.value + "." + eventName.value + "]"; }
        }

        protected override void OnExecute()
        {
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
    public class SendEventMeta<T> : ActionTask<Blackboard>
	{
		public BBParameter<string> eventType;
		public BBParameter<T> eventValue;
		public BBParameter<string> eventName;
		public BBParameter<bool> sendGlobal;
        public BBParameter<float> delay;

		protected override string info
		{
			get { return "Send " + (sendGlobal.value ? "Global " : "") + "Event Meta [" + eventType.value + "." + eventName.value + "]"; }
		}

		protected override void OnExecute()
		{
            if(delay.value > 0f)
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
