using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Conditions.Slot
{
    [Category("✶ Slots/Slot")]
    public class CheckSpinState : ConditionTask
    {
        [BlackboardOnly]
        public BBParameter<SlotMediator> mediator;
        public CompareMethod compareMethod = CompareMethod.EqualTo;
        public BBParameter<SpinState> spinState = SpinState.Stopped;

        protected override string info 
        {
            get { return string.Format("{0} {1} {2}", mediator, OperationUtils.GetCompareString(compareMethod), spinState); }
        }
        
        protected override bool OnCheck()
        {
            return OperationUtils.Compare((int)mediator.value.spinState, (int)spinState.value, compareMethod);
        }
    }
}