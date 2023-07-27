using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.ScatterExpectation
{
    [Category("✶ Slots/Expectation/Scatter")]
    /// This action solve to use row base. Volumn base solving will be more complex.
    public class CalcPossibleMaximumScatterCountsFromReel : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public bool canAppearMultipleSymbolsInOneReel;
        public bool allowOnlyFullyIn;
        public bool inverse;

        protected override void OnExecute()
        {
            SpinOutputScatterExpectation scatterExpectation = target.value as SpinOutputScatterExpectation;

            var results = new List<int>();
            var visibleAreas = scatterExpectation.source.visibleAreas;
            int count = visibleAreas.Count;
            int accumulatedPossibility = 0;
            for (int x = count - 1; x >= 0; --x)
            {
                int index = inverse ? (count - 1) - x : x;
                if (scatterExpectation.possibleReels[index])
                {
                    if (canAppearMultipleSymbolsInOneReel)
                    {
                        var visibleArea = visibleAreas[index];
                        int visibleVolumn = visibleArea.rowCount;
                        if (!allowOnlyFullyIn)
                            visibleVolumn += (scatterExpectation.scatter.rowCount - 1) * 2;
                        int possibility = visibleVolumn / scatterExpectation.scatter.rowCount;
                        accumulatedPossibility += possibility;
                    }
                    else
                    {
                        ++accumulatedPossibility;
                    }
                }
                results.Add(accumulatedPossibility);
            }

            results.Reverse();
            scatterExpectation.possibleMaximumScatterCountsFromReel = results;

            EndAction();
        }
    }
}