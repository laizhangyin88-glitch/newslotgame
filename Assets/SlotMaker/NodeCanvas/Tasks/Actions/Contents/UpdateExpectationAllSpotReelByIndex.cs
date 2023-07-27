using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationAllSpotReelByIndex : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<int> reelCount;
        public BBParameter<List<int>> visibleCounts;
        public BBParameter<int> symbolIndex;

    	protected override string info { get { return string.Format("UpdateExpectationAllSpot({0})", symbolIndex); } }

    	protected override void OnExecute()
    	{
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            for (int reelIndex = 0; reelIndex < reelCount.value; ++reelIndex)
            {
                int rowIndex    = reelIndex / column.value;
                int columnIndex = reelIndex % column.value;
                var symbolInfo = deck.GetDeckSymbol(columnIndex, rowIndex);

                if (symbolInfo.symbol == symbolIndex.value)
                {
                    expectation.expectationSpots[reelIndex].Add(new Cell(columnIndex, rowIndex));
                }
            }

    		EndAction();
    	}
    }
}
