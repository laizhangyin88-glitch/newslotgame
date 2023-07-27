using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Conditions
{
    [Category("✶ Kenos/Keno")]
    public class Keno_CheckPlayable : ConditionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;

        protected override string info { get { return $"{mediator}.pickCount >= {mediator}.minPickCount"; } }
        
        protected override bool OnCheck()
        {
            return (mediator.value.pickCount >= mediator.value.minPickCount) == true;
        }
    }
}