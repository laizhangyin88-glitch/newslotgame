using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Conditions
{
    [Category("✶ Kenos/Keno")]
    public class Keno_CheckState : ConditionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public CompareMethod compareMethod = CompareMethod.EqualTo;
        public BBParameter<KenoState> spinState = KenoState.Ready;

        protected override string info { get { return $"{mediator}.kenoState {OperationUtils.GetCompareString(compareMethod)} {spinState}"; } }
        
        protected override bool OnCheck()
        {
            return OperationUtils.Compare((int)mediator.value.kenoState, (int)spinState.value, compareMethod);
        }
    }
}