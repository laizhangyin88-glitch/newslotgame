using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion.Services;
using ParadoxNotion;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/IAM")]
    public class TriggerIAM : ActionTask
    {
        public BBParameter<BagelCode.ClientModels.InAppMessageTriggerType> type;
        public BBParameter<int> iamId;
        public BBParameter<bool> isTriggered;
        public BBParameter<bool> setCallback;
        public BBParameter<string> contextID;

        private const string eventName = "OnIAMCallback";

        protected override string info
        {
            get { return "Trigger " + type; }
        }

        protected override void OnExecute()
        {
            bool triggered;
            //Trigger IAM
            if (iamId.value != 0)
            {
                triggered = BagelCode.IAMRouter.Instance.TriggerIAMByID(type.value, iamId.value, setCallback.value ? agent.gameObject : null, contextID.value);
            }
            else
            {
                triggered = BagelCode.IAMRouter.Instance.TriggerIAM(type.value, setCallback.value ? agent.gameObject : null, contextID.value);
            }

            if (isTriggered != null) { isTriggered.value = triggered; }

            //Cancel Callback
            if (!triggered && setCallback.value)
                EventSender.SendEvent(agent.gameObject, MessageRouter.ON_CUSTOM_EVENT, new EventData(eventName));

            EndAction();
        }
    }
}
