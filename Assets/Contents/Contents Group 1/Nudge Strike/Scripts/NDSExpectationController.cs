using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSExpectationController : FeatureController
    {
        protected override string ON_FEATURE_BEGIN_EVENT { get => "UpdateNDSExpectation"; }
        protected override string ON_FEATURE_END_EVENT { get => "EndUpdateNDSExpectation"; }
        // Start is called before the first frame update
        public SlotMachine slotmachine;
        public List<GameObject> reelBGList;

        protected override IEnumerator OnPlayCoroutine()
        {
            int slotIndex = slotmachine.slotIndex;
            var slotData = ContentCustomData.GetSlotData(slotIndex);
            var deck = slotData.deck;
            var expectation = slotData.expectation;
            bool isNeedExpectation = true;

            for (int reelIndex = 0; reelIndex < 3; reelIndex++)
            {
                GameObject reelBG = reelBGList[reelIndex];
                reelBG.GetComponent<NDSReelBGController>().isNeedWildStop = false;
                for (int rowIndex = 2; rowIndex < 5; rowIndex++)
                {
                    int symbolIndex = deck.GetDeckSymbol(reelIndex, rowIndex).symbol;
                    if (CheckNudge(symbolIndex, rowIndex))
                    {
                        expectation.expectationSpots[reelIndex].Add(new Cell(reelIndex, rowIndex));
                        reelBG.GetComponent<NDSReelBGController>().isNeedWildStop = true;
                    }
                    if (rowIndex == 3 && isNeedExpectation)
                    {
                        if (!IsWild(symbolIndex))
                        {
                            isNeedExpectation = false;
                        }
                    }
                }
            }
            yield break;
        }

        private bool IsWild(int symbolIndex)
        {
            return symbolIndex == 0 || symbolIndex == 1;
        }

        private bool CheckNudge(int symbolIndex, int row)
        {
            if (symbolIndex != 0 && symbolIndex != 1)
            {
                return false;
            }

            if (symbolIndex == 0)
            {
                return row < 4;
            }
            else
            {
                return row > 2;
            }
        }
    }
}
