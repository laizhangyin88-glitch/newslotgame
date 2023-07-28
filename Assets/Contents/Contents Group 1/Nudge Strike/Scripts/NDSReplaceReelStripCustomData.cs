using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using SlotMaker;

namespace GameStudio.Slot.NDS
{
    public class NDSReplaceReelStripCustomData : FeatureController
    {
        protected override string ON_FEATURE_BEGIN_EVENT { get => "ReplaceSymbolCustomData"; }
        protected override string ON_FEATURE_END_EVENT { get => "EndReplaceSymbolCustomData"; }

        protected override IEnumerator OnPlayCoroutine()
        {
            List<Blackboard> reelOutputTable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./spin/response/reelOutputTable").value;
            List<Blackboard> multiplierOutputTable = BlackboardUtils.FindVariable<List<Blackboard>>(null, "./spin/response/multiplierOutputTable").value;
            for (int slotIndex = 0; slotIndex < 8; slotIndex++)
            {
                List<int> reelOutputList = BlackboardUtils.FindVariable<List<int>>(reelOutputTable[slotIndex], "value").value;
                List<Blackboard> multiplierOutputList = BlackboardUtils.FindVariable<List<Blackboard>>(multiplierOutputTable[slotIndex], "value").value;
                Deck deck = ContentCustomData.GetSlotData(slotIndex).deck;
                for (int rowIndex = 0; rowIndex < 7; rowIndex++)
                {
                    List<int> multiplierOutput = BlackboardUtils.FindVariable<List<int>>(multiplierOutputList[rowIndex], "value").value;
                    for (int colIndex = 0; colIndex < 3; colIndex++)
                    {
                        int multiplier = multiplierOutput[colIndex];

                        SymbolInfo symbol = deck.GetOriginalSymbol(colIndex, rowIndex);
                        symbol.multiplier = multiplier;

                        var newList = new List<SymbolInfo>();
                        int beginIndex = reelOutputList[colIndex] + rowIndex;
                        ReelStrips reelStrips = GlobalReelStrips.Instance.GetReelStrips();
                        var reelStrip = reelStrips.GetReelStrip(colIndex);
                        var newSymbolInfo = (SymbolInfo)reelStrip.GetSymbol(reelStrip.CalcIndex(beginIndex)).Clone();
                        if (newSymbolInfo.customData == null)
                            newSymbolInfo.customData = new Dictionary<string, object>();
                        newSymbolInfo.customData[slotIndex.ToString()] = multiplier;
                        newList.Add(newSymbolInfo);
                        reelStrip.ReplaceRange(beginIndex % reelStrip.stripCount, newList);
                    }
                }
            }

            yield break;
        }
    }
}
