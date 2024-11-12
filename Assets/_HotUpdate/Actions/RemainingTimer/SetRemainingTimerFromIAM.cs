using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils/Timer")]
    public class SetRemainingTimerFromIAM : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> iamInfoBB;
        public BBParameter<ContextElement> timerElement;
        
        public BBParameter<string>  timeFormatKey;
        public BBParameter<string>  outputFormatKey;

        public BBParameter<bool>    useWarningTime;
        public BBParameter<long>    warningGapMS;
        public BBParameter<string>  warningFormatKey;

        public BBParameter<string>  expireText;

        public BBParameter<bool>    useCommonTimer;

        protected override string info
        {
            get { return "Set Remaining Timer from IAM"; }
        }

        protected override void OnExecute()
        {
            var targetTime = iamInfoBB.value.GetValue<long>("endTimestamp");

            MetaContextElementUtils.SetCommonRemainingTimer(    timerElement.value,
                                                                targetTime,
                                                                useWarningTime.value ? targetTime - warningGapMS.value : 0L,
                                                                timeFormatKey.value,
                                                                outputFormatKey.value,
                                                                warningFormatKey.value,
                                                                expireText.value,
                                                                useCommonTimer.value,
                                                                null
                                                            );

            EndAction();
        }
    }
}
