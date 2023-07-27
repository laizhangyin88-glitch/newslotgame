using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.ScatterExpectation
{
    [Category("✶ Slots/Expectation/Scatter")]
    public class CalcExpectedSymbols : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public bool inverse;

        protected override void OnExecute()
        {
            SpinOutputScatterExpectation scatterExpectation = target.value as SpinOutputScatterExpectation;

            int count = scatterExpectation.foundScatterCounts.Count;
            scatterExpectation.expectedSpots.Clear();
            scatterExpectation.expectedValues.Clear();
            for (int i = 0; i < count; ++i)
            {
                scatterExpectation.expectedSpots.Add(new List<Cell3>());
                scatterExpectation.expectedValues.Add(0);
            }

            int accumulatedScatterCount = 0;
            for (int x = 0; x < count; ++x)
            {
                int index = inverse ? (count - 1) - x : x;
                if ((accumulatedScatterCount + scatterExpectation.possibleMaximumScatterCountsFromReel[x]) >= scatterExpectation.winningCount)
                {
                    scatterExpectation.expectedSpots[index].AddRange(scatterExpectation.foundSpots[index]);
                }

                int scatterCount = scatterExpectation.foundScatterCounts[index];
                if (scatterCount == 0 && scatterExpectation.continuous)
                    break;
                    
                accumulatedScatterCount += scatterCount;
                scatterExpectation.expectedValues[index] = accumulatedScatterCount;
            }

            EndAction();
        }
    }
}