using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_SetSpotWin : ActionTask<Blackboard>
    {
        public BBParameter<KenoWin> kenoWin;

        // Save As
        [BlackboardOnly]
        public BBParameter<long> earnCredit;
        [BlackboardOnly]
        public BBParameter<long> multiplier;
        [BlackboardOnly]
        public BBParameter<List<SpotInstance>> spots;
        [BlackboardOnly]
        public BBParameter<int>  hitCount;

        protected override void OnExecute()
        {
            earnCredit.value  = kenoWin.value.earnCredit;
            multiplier.value  = kenoWin.value.multiplier;
            spots.value       = kenoWin.value.spots;
            hitCount.value    = kenoWin.value.spots.Count;

            EndAction();
        }
    }
}
