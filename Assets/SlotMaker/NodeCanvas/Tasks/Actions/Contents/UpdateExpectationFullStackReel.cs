using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    [Description("Sets the Expectation for when all symbols on the reel have that mask.")]
    public class UpdateExpectationFullStackReel : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public SymbolAttribute symbolMask;
        public BBParameter<int> minFullStackedReelCount;
        [SerializeField] protected bool checkSpecificReels;
        [SerializeField] protected List<bool> masks;
        [SerializeField] protected bool continuous;
        [SerializeField] protected bool ignoreExpectation;

        protected override string info => $"UpdateExpectationFullStackReel({symbolMask})";

        private bool IsIgnoreColumn(int col)
        {
            return !checkSpecificReels ? false : !masks[col];
        }

        protected override void OnExecute()
        {
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            var fullStackedReelCount = 0;

            for (int colIndex = 0; colIndex < column.value; colIndex++)
            {
                if (IsIgnoreColumn(colIndex))
                    continue;

                // Check Scatter Win Available
                var leftColumnCount = column.value - colIndex;
                if (leftColumnCount < minFullStackedReelCount.value - fullStackedReelCount)
                    break;

                // Check Reel Expectation Condition
                if (fullStackedReelCount >= minFullStackedReelCount.value - 1)
                {
                    expectation.expectations[colIndex] = true;
                }

                // Check Spot Expectation Condition
                if (CheckReelFullStacked(deck, colIndex))
                {
                    for (int rowIndex = 0; rowIndex < row.value; rowIndex++)
                        expectation.expectationSpots[colIndex].Add(new Cell(colIndex, rowIndex));
                    fullStackedReelCount++;
                }
                else if (continuous) { break; }
            }
            EndAction();
    	}

        private bool CheckReelFullStacked(Deck deck, int reelIndex)
        {
            for (int rowIndex = 0; rowIndex < row.value; rowIndex++)
            {
                var symbolInfo = deck.GetDeckSymbol(reelIndex, rowIndex);

                if (!SymbolMask.HasAttribute(symbolInfo, symbolMask))
                {
                    return false;
                }
            }

            return true;
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR

        protected override void OnTaskInspectorGUI()
        {
            DrawDefaultInspector();

            checkSpecificReels = UnityEditor.EditorGUILayout.Toggle("Check Only Specific Reels", checkSpecificReels);
            if (checkSpecificReels)
                masks = (List<bool>)EditorUtils.ReflectedFieldInspector("Masks", masks, typeof(List<bool>));

            ignoreExpectation = UnityEditor.EditorGUILayout.Toggle("Ignore Expectation", ignoreExpectation);
            continuous = UnityEditor.EditorGUILayout.Toggle("Continuous", continuous);
        }

        #endif
    }
}

