using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    [Description("Update expectation in Spot Reel Game using custom order of spin.\nIt does not have to check all Reels. (can check specific reels)")]
    public class UpdateExpectationSpotReelCustomOrder : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> reelIndices;
        public BBParameter<List<int>> visibleCounts;
        public BBParameter<int> symbolCount;
        public BBParameter<int> minimumSymbolCount;
        public BBParameter<bool> continuous;
        public BBParameter<bool> ignoreExpectation;
        public BBParameter<bool> ignoreStopEffect;  // Expectation Spot is used in Stop Effect
        public BBParameter<List<bool>> ignoreCheckSpots;

        public SymbolAttribute symbolMask;

        private int reelCount { get { return reelIndices.value.Count; } }

        protected override string info { get { return string.Format("UpdateExpectationCustomSpotReel({0}, {1})", symbolMask, symbolCount); } }

        private List<int> hiddenSpotCounts = new List<int>();
        private void UpdateHiddenSpotCounts()
        {
            for (int i = 0; i < reelCount; i++)
                hiddenSpotCounts.Add(0);

            int spotCount = 0;
            for (int i = reelCount - 1; i >= 0; --i)
            {
                int reelIndex = reelIndices.value[i];
                if (!IsIgnoreSpot(reelIndex))
                {
                    spotCount += visibleCounts.value[reelIndex];
                }
                hiddenSpotCounts[reelIndex] = spotCount;
            }
        }

        private bool IsIgnoreSpot(int column)
        {
            return !(ignoreCheckSpots.isNull || ignoreCheckSpots.isNone) && ignoreCheckSpots.value[column];
        }

        private bool IsPossibleExpectation(int hiddenSpotCount, int accumulatedSpotCount)
        {
            return (hiddenSpotCount + accumulatedSpotCount) >= symbolCount.value;
        }

        private bool IsExpectation(int hiddenSpotCount, int accumulatedSpotCount)
        {
            return ignoreExpectation.value == false
                && (accumulatedSpotCount >= minimumSymbolCount.value);
        }

        private bool IsStopEffect(SlotData slotData, int columnIndex, int rowIndex)
        {
            var symbolInfo = slotData.deck.GetDeckSymbol(columnIndex, rowIndex);
            return SymbolMask.HasAttribute(symbolInfo, symbolMask);
        }
        protected override void OnExecute()
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            int accumulatedSpotCount = 0;
            UpdateHiddenSpotCounts();

            foreach (int reelIndex in reelIndices.value)
            {
                if (IsIgnoreSpot(reelIndex))
                    continue;

                if (IsPossibleExpectation(hiddenSpotCounts[reelIndex], accumulatedSpotCount) == false)
                    break;

                if (reelIndex != 0 && IsExpectation(hiddenSpotCounts[reelIndex], accumulatedSpotCount))
                    expectation.expectations[reelIndex] = true;

                int rowIndex = reelIndex / column.value;
                int columnIndex = reelIndex % column.value;

                if (IsStopEffect(slotData, columnIndex, rowIndex))
                {
                    ++accumulatedSpotCount;
                    if (ignoreStopEffect.value == false)
                        expectation.expectationSpots[reelIndex].Add(new Cell(columnIndex, rowIndex));
                }
                // continuous is True -> check symbols that are connected ([O O O X X] is 3 but, [O O X O X] is 2)
                // continuous is False -> check all symbols even they are not connected (Both of [O O O X X] and [O O X O X] are 3)
                else if (continuous.value == false)
                    break;
            }
            EndAction();
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
#if UNITY_EDITOR

        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();
        }
#endif
    }
}
