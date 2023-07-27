using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;

namespace SlotMaker.Slots.Tasks.Actions.ScatterExpectation
{
    [Category("✶ Slots/Expectation/Scatter")]
    public class FindScatter : ActionTask
    {
        [BlackboardOnly]
        public BBParameter<SpinOutputSubset> target;
        public BBParameter<int> layer;

        protected override void OnExecute()
        {
            SpinOutputScatterExpectation scatterExpectation = target.value as SpinOutputScatterExpectation;

            scatterExpectation.foundSpots.Clear();
            scatterExpectation.foundScatterCounts.Clear();
            var visibleAreas = scatterExpectation.source.visibleAreas;
            int count = visibleAreas.Count;
            for (int i = 0; i < count; ++i)
            {
                var founds = new List<Cell3>();
                int foundScatterCount = 0;
                var visibleArea = visibleAreas[i];
                int x = visibleArea.xMin;
                for (int y = visibleArea.yMin; y < visibleArea.yMax; ++y)
                {
                    var symbol = scatterExpectation.source.GetSymbol(x, y, layer.value);
                    if (symbol.HasAnyAttribute(scatterExpectation.scatter.attribute))
                    {
                        ++foundScatterCount;
                        founds.Add(new Cell3(x, y, layer.value));
                    }
                }
                scatterExpectation.foundSpots.Add(founds);
                scatterExpectation.foundScatterCounts.Add(foundScatterCount);
            }

            EndAction();
        }
    }
}