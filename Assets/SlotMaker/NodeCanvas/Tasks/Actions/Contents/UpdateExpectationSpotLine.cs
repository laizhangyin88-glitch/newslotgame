using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationSpotLine : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> visibleCounts;
        public BBParameter<List<Blackboard>> payLine;
        public SymbolAttribute symbolMask;
        public int symbolCount;
        public int minimumSymbolCount;
        public bool multipleSpot = false;
        public bool bidirectional;

        [SerializeField] protected bool checkAll;
        [SerializeField] protected bool continuous;
        [SerializeField] protected bool ignoreExpectation;
        [SerializeField] protected bool ignoreStopEffect;
        [SerializeField] protected List<bool> masks;

        protected override string info { get { return string.Format("UpdateExpectationSpotLine({0}, {1})", symbolMask, symbolCount); } }

        private List<int> hiddenSpotCounts = new List<int>();
        private List<int> spotIndexes = new List<int>();
        private void UpdateHiddenSpotCounts()
        {
            int spotCount = 0;
            for (int i = column.value - 1; i >= 0; --i)
            {
                if (!IsIgnoreColumn(i))
                {
                    spotCount += multipleSpot ? visibleCounts.value[i] : 1;
                }

                hiddenSpotCounts.Add(spotCount);
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

        protected int GetDirectionalColumn(int column, int direction)
        {
            return direction == 1 ? column : (this.column.value - 1) - column;
        }

        public List<int> GetPayLine(int lineIndex)
        {
            return payLine.value[lineIndex].GetValue<List<int>>("value");
        }

    	protected override void OnExecute()
    	{
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            UpdateHiddenSpotCounts();
            spotIndexes.Clear();

            for (int lineIndex = 0; lineIndex < payLine.value.Count; ++lineIndex)
            {
                int accumulatedSpotCount = 0;
                int direction = 1;
                if (bidirectional)
                    direction = -1;
                List<int> line = GetPayLine(lineIndex);

                for (int i = 0; i < column.value; ++i)
                {
                    if (IsIgnoreColumn(i))
                        continue;

                    if (i != 0 && NeedExpectation(hiddenSpotCounts[i], accumulatedSpotCount))
                        expectation.expectations[i] = true;

                    if (!NeedExpectationSpot(hiddenSpotCounts[i], accumulatedSpotCount))
                        break;

                    int dColumn = GetDirectionalColumn(i, direction);
                    int j = line[dColumn];
                    bool found = false;
                    var symbolInfo = deck.GetDeckSymbol(i, j);
                    if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
                    {
                        ++accumulatedSpotCount;
                        if (!ignoreStopEffect && spotIndexes.IndexOf(i * column.value + j) == -1)
                        {
                            spotIndexes.Add(i * column.value + j);
                            expectation.expectationSpots[i].Add(new Cell(i, j));
                        }
                        found = true;
                    }

                    if (!found && continuous) break;
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
            ignoreExpectation = UnityEditor.EditorGUILayout.Toggle("Ignore Expectation", ignoreExpectation);
            ignoreStopEffect = UnityEditor.EditorGUILayout.Toggle("Ignore Stop Effect", ignoreStopEffect);
            continuous = UnityEditor.EditorGUILayout.Toggle("Continuous", continuous);
        }

        #endif
    }
}
