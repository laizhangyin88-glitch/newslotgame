using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationSpotBySubSymbol : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> visibleCounts;
        public int subSymbolIndex;
        public int symbolCount;
        public int minimumSymbolCount;
        public bool multipleSpot = true;

        [SerializeField] protected bool checkAll;
        [SerializeField] protected bool continuous;
        [SerializeField] protected bool ignoreExpectation;
        [SerializeField] protected List<bool> masks;

        protected override string info { get { return string.Format("UpdateExpectationSpot(SubSymbol, " + subSymbolIndex + ")"); } }

        private List<int> hiddenSpotCounts = new List<int>();
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

    	protected override void OnExecute()
    	{
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            int accumulatedSpotCount = 0;
            UpdateHiddenSpotCounts();

            for (int i = 0; i < column.value; ++i)
            {
                if (IsIgnoreColumn(i))
                    continue;

                if (i != 0 && NeedExpectation(hiddenSpotCounts[i], accumulatedSpotCount))
                    expectation.expectations[i] = true;

                if (!NeedExpectationSpot(hiddenSpotCounts[i], accumulatedSpotCount))
                    break;

                bool found = false;
                for (int j = 0; j < row.value; ++j)
                {
                    var symbolInfo = deck.GetDeckSymbol(i, j);
                    if (!SymbolMask.HasReject(symbolInfo) && symbolInfo.subSymbol.symbol == subSymbolIndex)
                    {
                        ++accumulatedSpotCount;
                        expectation.expectationSpots[i].Add(new Cell(i, j));
                        found = true;
                    }
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
        }

        #endif
    }
}
