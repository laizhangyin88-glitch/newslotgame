using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Conditions.Slot
{
    [Category("✶ Slots/Slot")]
    public class CheckSpinStates : ConditionTask
    {
        [BlackboardOnly]
        public BBParameter<List<SlotMediator>> mediators;
        public CompareMethod compareMethod = CompareMethod.EqualTo;
        public BBParameter<SpinState> spinState = SpinState.Stopped;

        protected override string info
        {
            get { return string.Format("{0} {1} {2}", mediators, OperationUtils.GetCompareString(compareMethod), spinState); }
        }

        protected override bool OnCheck()
        {
            foreach (var mediator in mediators.value)
            {
                if (!OperationUtils.Compare((int)mediator.spinState, (int)spinState.value, compareMethod))
                    return false;
            }
            return true;
        }
    }
}