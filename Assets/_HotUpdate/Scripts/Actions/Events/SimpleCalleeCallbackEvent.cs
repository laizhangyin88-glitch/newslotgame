using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using SlotMaker;

namespace BagelCode.Task.Condition
{
    [Category("★ BagelCode/Events")]
    public class SimpleCalleeCallbackEvent : ActionTask<Blackboard>
    {
        [RequiredField]
        public BBParameter<string> eventName = "OnCalleeCallback";

        protected override string info
        {
            get
            {
                return "★ Send Event " + eventName.value + " if caller exist";
            }
        }

        protected override void OnUpdate()
        {
            var caller = BlackboardUtils.FindVariable<GameObject>(agent, "caller");

            if (caller != null && caller.value != null)
                EventSender.SendEvent(caller.value, MessageRouter.ON_CUSTOM_EVENT, new EventData(eventName.value));

            EndAction();
        }
    }
}
