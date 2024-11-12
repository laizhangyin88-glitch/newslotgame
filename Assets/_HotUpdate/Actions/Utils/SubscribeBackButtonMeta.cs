using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions
{
    [Category("✫ BagelCode/Utils")]
    public class SubscribeBackButtonMeta : ActionTask
    {
        public BBParameter<string> eventType;
        public BBParameter<string> eventName;
        public bool sendGlobal = false;

        protected override string info
        {
            get
            {
                return string.Format("Subscribe {0}{1}{2} Event to BackButton Event", (sendGlobal ? "(Global)" : ""), eventType, eventName);
            }
        }

        protected override void OnExecute()
        {
            if (sendGlobal)
            {
                MetaSystem.SubscribeBackButton(agent.gameObject.GetHashCode(),
                    () =>
                    {
                        EventSender.SendGlobalEvent(eventType.value, eventName.value);
                    });
            }
            else
            {
                MetaSystem.SubscribeBackButton(agent.gameObject.GetHashCode(),
                    () =>
                    {
                        EventSender.SendEvent(agent.gameObject, eventType.value, eventName.value);
                    });
            }

            EndAction();
        }
    }
}
