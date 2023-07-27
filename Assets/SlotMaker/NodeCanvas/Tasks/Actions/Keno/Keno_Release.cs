using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_Release : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;

        protected override string info { get { return $"{mediator}.Release()"; } }

        protected override void OnExecute()
        {
            for (int i = 0; i < mediator.value.row * mediator.value.column; ++i)
                mediator.value.GetSpot(i).Release();

            EndAction();
        }
    }
}