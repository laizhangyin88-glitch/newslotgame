using NodeCanvas.Framework;
using ParadoxNotion;
using SlotMaker;
using UnityEngine;
using ParadoxNotion.Design;

namespace BagelCode.Task.Actions
{
    [Category("★ BagelCode/Utils")]
    public class SendCalleeCallbackMeta : ActionTask<Blackboard>
	{
		public BBParameter<string> eventType;
		public BBParameter<string> eventName;
		public BBParameter<bool> sendGlobal;

		protected override string info
		{
			get { return "Send " + (sendGlobal.value ? "Global " : "") + "Event Meta [" + eventType.value + "." + eventName.value + "]"; }
		}

		protected override void OnExecute()
		{
			if (sendGlobal.value)
			{
				EventSender.SendGlobalEvent(eventType.value, new EventData(eventName.value));
			}
			else
			{
				var caller = BlackboardUtils.FindVariable<GameObject>(agent, "caller");
				if (caller != null && caller.value != null)
				{
					EventSender.SendEvent(caller.value, eventType.value, new EventData(eventName.value));
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
    public class SendCalleeCallbackMeta<T> : ActionTask<Blackboard>
    {
        public BBParameter<string> eventType;
        public BBParameter<T> eventValue;
        public BBParameter<string> eventName;
        public BBParameter<bool> sendGlobal;

        protected override string info
        {
            get { return "Send " + (sendGlobal.value ? "Global " : "") + "Event Meta [" + eventType.value + "." + eventName.value + "]"; }
        }

        protected override void OnExecute()
        {
            var e = new EventData<T>(eventName.value, eventValue.value);

            if (sendGlobal.value)
            {
                EventSender.SendGlobalEvent(eventType.value, e);
            }
            else
            {
                var caller = BlackboardUtils.FindVariable<GameObject>(agent, "caller");
                if (caller != null && caller.value != null)
                {
                    EventSender.SendEvent(caller.value, eventType.value, e);
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
}
