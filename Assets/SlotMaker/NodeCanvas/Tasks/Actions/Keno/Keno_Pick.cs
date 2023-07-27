using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_Pick : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<int> index;

        protected override string info { get { return $"Keno Pick({index})"; } }

        protected override void OnExecute()
        {
            mediator.value.GetSpot(index.value).Pick();
            EndAction();
        }
    }
}