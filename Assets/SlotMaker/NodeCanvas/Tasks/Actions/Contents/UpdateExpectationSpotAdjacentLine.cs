using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationSpotAdjacentLine : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> visibleCounts;
        public BBParameter<List<Blackboard>> payLines;
        public BBParameter<SymbolAttribute> symbolMask;
        public BBParameter<int> symbolCount;
        public BBParameter<int> minimumSymbolCount;
        public BBParameter<bool> ignoreExpectation;
        public BBParameter<bool> ignoreExpectationSpot;

        [SerializeField] protected bool checkAll;
        [SerializeField] protected List<bool> masks;

        protected override string info { get { return string.Format("UpdateExpectationSpotAdjacentLine({0}, {1})", symbolMask.value, symbolCount.value); } }

        private List<int> GetHiddenSpotCounts()
        {
            List<int> hiddenSpotCounts = new List<int>(column.value);
            int spotCount = 0;

            for (int colIndex = column.value - 1; colIndex >= 0; --colIndex)
            {
                if (!IsIgnoreColumn(colIndex))
                    ++spotCount;

                hiddenSpotCounts.Add(spotCount);
            }
            hiddenSpotCounts.Reverse();

            return hiddenSpotCounts;
        }

        private bool IsIgnoreColumn(int column)
        {
            return checkAll ? false : !masks[column];
        }

        private bool NeedExpectation(int hiddenSpotCount, int accumulatedSpotCount)
        {
            return ignoreExpectation.value ? false : ((hiddenSpotCount + accumulatedSpotCount) >= symbolCount.value) && (accumulatedSpotCount >= minimumSymbolCount.value);
        }

        private bool NeedExpectationSpot(int hiddenSpotCount, int accumulatedSpotCount)
        {
            return ignoreExpectationSpot.value ? false : (hiddenSpotCount + accumulatedSpotCount) >= symbolCount.value;
        }

        private SymbolInfo GetSymbolInfo(int column, int row)
        {
            return ContentCustomData.GetSlotData(slotIndex.value).deck.GetSymbol(column, row);
        }

        public List<int> GetPayLine(int lineIndex)
        {
            return payLines.value[lineIndex].GetValue<List<int>>("value");
        }

    	protected override void OnExecute()
    	{
            Expectation expectation = ContentCustomData.GetSlotData(slotIndex.value).expectation;
            List<int> hiddenSpotCounts = GetHiddenSpotCounts();
            List<int> hitMap = new List<int>();

            for (int lineIndex = 0; lineIndex < payLines.value.Count; ++lineIndex)
            {
                List<int> line = GetPayLine(lineIndex);
                int accumulatedSpotCount = 0;

                for (int colIndex = 0; colIndex < column.value; ++colIndex)
                {
                    if (IsIgnoreColumn(colIndex))
                    {
                        accumulatedSpotCount = 0;
                        continue;
                    }

                    if (colIndex != 0 && NeedExpectation(hiddenSpotCounts[colIndex], accumulatedSpotCount))
                        expectation.expectations[colIndex] = true;

                    int rowIndex = line[colIndex];
                    var symbolInfo = GetSymbolInfo(colIndex, rowIndex);

                    if (!SymbolMask.HasAttribute(symbolInfo, symbolMask.value))
                    {
                        accumulatedSpotCount = 0;
                    }
                    else
                    {
                        if (NeedExpectationSpot(hiddenSpotCounts[colIndex], accumulatedSpotCount))
                        {
                            int spotIndex = (rowIndex * column.value) + colIndex;
                            if (hitMap.IndexOf(spotIndex) == -1)
                            {
                                hitMap.Add(spotIndex);
                                expectation.expectationSpots[colIndex].Add(new Cell(colIndex, rowIndex));
                            }                            
                        }
                        ++accumulatedSpotCount;
                    }
                }
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
        }

        #endif
    }
}
