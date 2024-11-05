using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils/Timer")]
    public class SetEventTagTimer : ActionTask<Blackboard>
    {
        public BBParameter<ContextElement> tagElement;

        public BBParameter<long>    targetTime;
        public BBParameter<long>    warningTime;
        
        public BBParameter<string>  timeFormatKey;
        public BBParameter<string>  outputFormatKey;
        public BBParameter<string>  warningFormatKey;

        public BBParameter<string>  expireText;
        public BBParameter<bool>    useCommonTimer;

        public BBParameter<bool>    useCallback;
        public BBParameter<string>  eventText;

        protected override string info
        {
            get { return "Set Event Tag Timer"; }
        }

        protected override void OnExecute()
        {
            if(tagElement.value != null)
            {
                EventTagController eventController = tagElement.value.gameObject.GetComponent<EventTagController>();
                eventController.Initialize( targetTime.value,
                                            warningTime.value,
                                            timeFormatKey.value,
                                            outputFormatKey.value,
                                            warningFormatKey.value,
                                            expireText.value,
                                            useCommonTimer.value,
                                            useCallback.value ? agent.gameObject : null,
                                            null,
                                            0f,
                                            0f,
                                            eventText.value
                                        );
            }
            

            EndAction();
        }
    }
}
