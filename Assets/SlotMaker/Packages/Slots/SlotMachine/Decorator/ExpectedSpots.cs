using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SlotMaker.Slots
{
    public class ExpectedSpots : MonoBehaviour
    {
        public SlotMediator slot;
        public List<SpinOutputExpectationSubset> outputs;

        public void Expectation(ReelInstance reelInstance)
        {
            int index = slot.GetReelIndex(reelInstance);
            if (index >= 0)
            {
                foreach (var output in outputs)
                {
                    var expectedSpots = output.GetExpectedSpots();
                    if (index > (expectedSpots.Count - 1))
                        continue;

                    var spots = expectedSpots[index];
                    foreach (var spot in spots)
                    {
                        var symbolInstance = reelInstance.GetPatchingSymbol(spot.x, spot.y);
                        if (symbolInstance != null)
                            symbolInstance.SendEvent("Expectation");
                    }
                }
            }
        }
    }
}