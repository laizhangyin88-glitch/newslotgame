using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Keno.Tasks.Actions
{
    [Category("★ SlotMaker/Keno")]
    public class Keno_GetPickResult : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<KenoMediator> mediator;
        [BlackboardOnly]
        public BBParameter<List<int>> saveAs;

        protected override string info { get { return $"{saveAs} = Pick Numbers"; } }

        protected override void OnExecute()
        {
            List<int> pickNumbers = new List<int>();
            for (int i = 0; i < mediator.value.row * mediator.value.column; ++i)
            {
                var spot = mediator.value.GetSpot(i);
                if (spot.markState == MarkState.Mark)
                    pickNumbers.Add(spot.number);
            }

            saveAs.value = pickNumbers;
            EndAction();
        }
    }
}