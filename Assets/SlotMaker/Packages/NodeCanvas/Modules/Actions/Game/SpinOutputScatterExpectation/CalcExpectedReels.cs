using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.ScatterExpectation
{
    [Category("✶ Slots/Expectation/Scatter")]
    public class CalcExpectedReels : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public bool inverse;

        protected override void OnExecute()
        {
            SpinOutputScatterExpectation scatterExpectation = target.value as SpinOutputScatterExpectation;

            int count = scatterExpectation.foundScatterCounts.Count;
            scatterExpectation.expectedReels.Clear();
            for (int i = 0; i < count; ++i)
                scatterExpectation.expectedReels.Add(false);

            int accumulatedScatterCount = 0;
            for (int x = 0; x < count; ++x)
            {
                int index = inverse ? (count - 1) - x : x;
                if (index != 0 && scatterExpectation.possibleReels[index] && 
                    ((accumulatedScatterCount + scatterExpectation.possibleMaximumScatterCountsFromReel[index]) >= scatterExpectation.winningCount) &&
                    (accumulatedScatterCount >= scatterExpectation.minimumCount))
                {
                    scatterExpectation.expectedReels[index] = true;
                }
                
                int scatterCount = scatterExpectation.foundScatterCounts[index];
                if (scatterCount == 0 && scatterExpectation.continuous)
                    break;
                    
                accumulatedScatterCount += scatterCount;
            }

            EndAction();
        }
    }
}