using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_Stop : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;

        protected override string info { get { return $"{mediator}.Stop()"; } }

        protected override void OnExecute()
        {
            mediator.value.Stop();
            EndAction();
        }
    }
}