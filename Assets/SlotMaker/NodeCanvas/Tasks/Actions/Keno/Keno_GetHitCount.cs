using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_GetHitCount : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        [BlackboardOnly]
        public BBParameter<int> saveAs;

        protected override string info { get { return $"{saveAs} = {mediator}.hitCount"; } }

        protected override void OnExecute()
        {
            saveAs.value = mediator.value.hitCount;
            EndAction();
        }
    }
}