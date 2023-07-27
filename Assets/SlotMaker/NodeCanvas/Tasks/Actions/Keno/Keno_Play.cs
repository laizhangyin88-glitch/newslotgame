using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_Play : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<List<int>> drawResult;

        protected override string info { get { return $"{mediator}.Play"; } }

        protected override void OnExecute()
        {
            mediator.value.Play(drawResult.value);
            EndAction();
        }
    }
}