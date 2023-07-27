using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_GetSpot: ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<int> index;
        public BBParameter<SpotInstance> saveAs;

        protected override string info { get { return $"{saveAs} = Keno Spot({index})"; } }

        protected override void OnExecute()
        {
            saveAs.value = mediator.value.GetSpot(index.value);
            EndAction();
        }
    }
}