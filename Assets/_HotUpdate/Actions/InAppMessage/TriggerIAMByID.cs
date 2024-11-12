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
    public class TriggerIAMByID : ActionTask
    {
        public BBParameter<BagelCode.ClientModels.InAppMessageTriggerType> type;
        public BBParameter<int> iamId;
        public BBParameter<bool> isTriggered;
        public BBParameter<bool> usingCoolTime;
        public BBParameter<bool> usingExposureCount;
        public BBParameter<bool> setCallback;
        public BBParameter<string> contextID;
        public BBParameter<bool> refresh = false;

        private const string eventName = "OnIAMCallback";

        protected override string info
        {
            get { return string.Format("Trigger By ID {0}", iamId); }
        }

        protected override void OnExecute()
        {
            isTriggered.value = false;

            //Trigger IAM
            if (iamId.value != 0)
            {
                isTriggered.value = BagelCode.IAMRouter.Instance.TriggerIAMByID(type.value, iamId.value, setCallback.value ? agent.gameObject : null, contextID.value, usingCoolTime.value, usingExposureCount.value, refresh.value);

                //Cancel Callback
                if (!isTriggered.value && setCallback.value)
                    EventSender.SendEvent(agent.gameObject, MessageRouter.ON_CUSTOM_EVENT, new EventData(eventName));
            }

            EndAction();
        }
    }
}
