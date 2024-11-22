using System;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/TimeUtils")]
    public class GetTimeRate : ActionTask<Blackboard>
    {
        public BBParameter<long>    startTime;
        public BBParameter<long>    endTime;
        public BBParameter<bool>    isInvert;

        [BlackboardOnly]        
        public BBParameter<float>    saveAs;

        protected override string info
        {
            get { return string.Format("{0} = Get Time Rate(0f~1f)", saveAs); }
        }

        protected override void OnExecute()
        {
            long totalTime = endTime.value - startTime.value;
            long current = TimeUtils.GetTimeStamp() - startTime.value;

            if(isInvert.value)
                saveAs.value = 1f - (float)((double)current/(double)totalTime);
            else
                saveAs.value = (float)((double)current/(double)totalTime);

            if(saveAs.value < 0f)
                saveAs.value = 0f;
            else if(saveAs.value > 1f)
                saveAs.value = 1f;

            EndAction();
        }
    }
}
