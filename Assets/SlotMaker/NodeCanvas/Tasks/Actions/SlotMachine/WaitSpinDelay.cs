using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Actions
{
    [Category("★ SlotMaker/SlotMachine")]
    public class WaitSpinDelay : ActionTask
    {
        public BBParameter<int> movementType;
        public BBParameter<float> spinTime;
        public BBParameter<float> spinDelay;
        public BBParameter<float> boostSpinDelay;

        private float waitTime;

        protected override void OnExecute()
        {
            waitTime = (movementType.value == 0) ? spinDelay.value : boostSpinDelay.value;
        }

        protected override void OnUpdate()
        {
            if (Time.time > (spinTime.value + waitTime))
                EndAction();
        }
    }
}
