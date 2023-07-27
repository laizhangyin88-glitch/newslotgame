using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Conditions
{
    [Category("✶ Kenos/Keno")]
    public class Keno_CheckDrawnAll : ConditionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;

        protected override string info { get { return $"{mediator}.Instance.IsDrawnAll"; } }
        
        protected override bool OnCheck()
        {
            return mediator.value.KenoInstance.IsDrawnAll == true;
        }
    }
}