using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace NodeCanvas.Tasks.Actions{

    [Category("★ BagelCode/SlotMachine")]
    public class SkippableWait : ActionTask {

        public BBParameter<float> waitTime = new BBParameter<float>{value = 1};
        public CompactStatus finishStatus  = CompactStatus.Success;

        public BBParameter<bool> skip;

        protected override string info{
            get {return "(Skippable)Wait " + waitTime + " sec.";}
        }

        protected override void OnUpdate() 
        {
            if (skip.value == true)
            {
                EndAction(!Condition());
            }
            else if (elapsedTime >= waitTime.value) 
            {
                EndAction(Condition());
            }
        }

        private bool Condition()
        {
            return finishStatus == CompactStatus.Success? true : false;
        }
    }
}
