using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.ScatterExpectation
{
    [Category("✶ Slots/Expectation/Scatter")]
    public class ExcludeScatter : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;

        public BBParameter<int> referenceLayer;

        protected override void OnExecute()
        {
            SpinOutputScatterExpectation scatterExpectation = target.value as SpinOutputScatterExpectation;

            var spots = scatterExpectation.foundSpots;
            var counts = scatterExpectation.foundScatterCounts;
            for (int i = 0, count = spots.Count; i < count; ++i)
            {
                for (int j = 0; j < spots[i].Count;)
                {
                    var cell = spots[i][j];
                    var refSymbol = scatterExpectation.source.GetSymbol(cell.x, cell.y, referenceLayer.value);
                    if (refSymbol.isNull) ++j;
                    else spots[i].RemoveAt(j);
                }
                counts[i] = spots[i].Count;
            }

            EndAction();
        }
    }
}