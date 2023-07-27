using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_GetPickCount : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        public BBParameter<int> saveAs;

        protected override string info { get { return $"{saveAs} = Keno.PickCount"; } }

        protected override void OnExecute()
        {
            int pickCount = 0;
            for (int i = 0; i < mediator.value.row * mediator.value.column; ++i)
            {
                if (mediator.value.GetSpot(i).markState == MarkState.Mark)
                    pickCount++;
            }

            saveAs.value = pickCount;
            EndAction();
        }
    }
}