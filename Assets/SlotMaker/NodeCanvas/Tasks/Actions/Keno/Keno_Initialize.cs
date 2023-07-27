using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_Initialize : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;

        protected override string info { get { return $"{mediator}.Initialize()"; } }

        protected override void OnExecute()
        {
            mediator.value.Initialize();
            EndAction();
        }
    }
}