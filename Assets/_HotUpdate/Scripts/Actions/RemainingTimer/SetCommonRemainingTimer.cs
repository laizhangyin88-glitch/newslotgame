using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils/Timer")]
    public class SetCommonRemainingTimer : ActionTask<Blackboard>
    {
        public BBParameter<ContextElement> timerElement;

        public BBParameter<long>    targetTime;
        public BBParameter<long>    warningTime;
        
        public BBParameter<string>  timeFormatKey;
        public BBParameter<string>  outputFormatKey;
        public BBParameter<string>  warningFormatKey;

        public BBParameter<string>  expireText;

        public BBParameter<bool>    useCommonTimer;

        protected override string info
        {
            get { return "Set Common Remaining Timer"; }
        }

        protected override void OnExecute()
        {
            MetaContextElementUtils.SetCommonRemainingTimer(    timerElement.value,
                                                                targetTime.value,
                                                                warningTime.value,
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
