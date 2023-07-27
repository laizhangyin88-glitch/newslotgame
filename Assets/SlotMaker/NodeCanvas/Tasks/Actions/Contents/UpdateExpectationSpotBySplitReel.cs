using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationSpotBySplitReel : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> splitColumn;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> visibleCounts;
        public SymbolAttribute symbolMask;
        public int symbolCount;
        public int minimumSymbolCount;
        public bool multipleSpot = true;

        [SerializeField] protected bool checkAll;
        [SerializeField] protected bool continuous;
        [SerializeField] protected bool ignoreExpectation;
        [SerializeField] protected List<bool> masks;
        [SerializeField] protected bool splitCheckAll;
        [SerializeField] protected List<bool> splitMasks;

        protected override string info { get { return string.Format("Update Expectation Spot By Split({0}, {1})", symbolMask, symbolCount); } }

        private List<int> hiddenSpotCounts = new List<int>();
        private void UpdateHiddenSpotCounts()
        {
            int spotCount = 0;
            int columnIndex = 0;
            int relativeSpotIndex = 0;
            for (int i = splitColumn.value - 1; i >= 0; --i)
            {
                //Need && IsSplitColumn(i) ?
                if (!IsIgnoreColumn(columnIndex))
                {
                    spotCount += multipleSpot ? visibleCounts.value[i] : 1;
                }

                hiddenSpotCounts.Add(spotCount);
                relativeSpotIndex += visibleCounts.value[i];
                if ((relativeSpotIndex - 1) % row.value == 0)
                {
                    columnIndex++;
                }
            }
            hiddenSpotCounts.Reverse();
        }

        private bool IsIgnoreColumn(int col)
        {
            if (checkAll)
                return false;
            else
                return !masks[col];
        }

        private bool IsSplitColumn(int col)
        {
            if (splitCheckAll)
                return true;
            else
                return splitMasks[col];
        }

        private bool NeedExpectation(int hiddenSpotCount, int accumulatedSpotCount)
        {
            return !ignoreExpectation &&
                ((hiddenSpotCount + accumulatedSpotCount) >= symbolCount) &&
                (accumulatedSpotCount >= minimumSymbolCount);
        }

        private bool NeedExpectationSpot(int hiddenSpotCount, int accumulatedSpotCount)
        {
            return (hiddenSpotCount + accumulatedSpotCount) >= symbolCount;
        }

    	protected override void OnExecute()
    	{
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            int columnIndex = 0;
            int accumulatedSpotCount = 0;
            int accumulatedNonSplitReelCount = 0;
            UpdateHiddenSpotCounts();

            for (int i = 0; i < splitColumn.value; ++i)
            {
                if (IsIgnoreColumn(columnIndex))
                {
                    columnIndex++;
                    accumulatedNonSplitReelCount++;
                    continue;
                }

                if (i != 0 && NeedExpectation(hiddenSpotCounts[i], accumulatedSpotCount))
                    expectation.expectations[i] = true;

                if (!NeedExpectationSpot(hiddenSpotCounts[i], accumulatedSpotCount))
                    break;

                bool found = false;
                if (IsSplitColumn(i))
                {
                    var rowIndex = (i - accumulatedNonSplitReelCount) % row.value;
                    var symbolInfo = deck.GetDeckSymbol(columnIndex, rowIndex);
                    if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
                    {
                        ++accumulatedSpotCount;
                        expectation.expectationSpots[i].Add(new Cell(i, rowIndex));
                        found = true;
                    }
                    if (rowIndex == row.value - 1) columnIndex++;
                }
                else
                {
                    for (int j = 0; j < row.value; ++j)
                    {
                        var symbolInfo = deck.GetDeckSymbol(columnIndex, j);
                        if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
                        {
                            ++accumulatedSpotCount;
                            expectation.expectationSpots[i].Add(new Cell(i, j));
                            found = true;
                        }
                    }
                    columnIndex++;
                    accumulatedNonSplitReelCount++;
                }

                if (!found && continuous) break;
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

            checkAll = UnityEditor.EditorGUILayout.Toggle("Check All", checkAll);
            if (!checkAll)
                masks = (List<bool>)EditorUtils.ReflectedFieldInspector("Masks", masks, typeof(List<bool>));
            ignoreExpectation = UnityEditor.EditorGUILayout.Toggle("Ignore Expectation", ignoreExpectation);
            continuous = UnityEditor.EditorGUILayout.Toggle("Continuous", continuous);
            splitCheckAll = UnityEditor.EditorGUILayout.Toggle("Split Check All", splitCheckAll);
            if (!splitCheckAll)
                splitMasks = (List<bool>)EditorUtils.ReflectedFieldInspector("Split Masks", splitMasks, typeof(List<bool>));
        }

        #endif
    }
}
