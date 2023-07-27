using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/NativeHelper")]
    public class SubscribeBackButtonEvent : ActionTask
    {
        public BBParameter<string> eventName;
        public bool sendGlobal = false;

        protected override string info
        {
            get 
            {
                return string.Format("Subscribe {0}{1} Event to BackButton Event", (sendGlobal ? "(Global)" : ""), eventName);
            }
        }

        protected override void OnExecute()
        {
            GraphOwner owner = agent.GetComponent<GraphOwner>();

            if (sendGlobal)
            {
                MetaSystem.SubscribeBackButton(owner.GetHashCode(), () => { GraphOwner.SendGlobalEvent(eventName.value); });
            }
            else
            {
                MetaSystem.SubscribeBackButton(owner.GetHashCode(), () => { owner.SendEvent(eventName.value); });
            }

            EndAction();
        }
    }
}
