using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationAllSpot : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
    	public BBParameter<int> column;
        public BBParameter<int> row;
        public BBParameter<List<int>> visibleCounts;
        public SymbolAttribute symbolMask;

    	protected override string info { get { return string.Format("UpdateExpectationAllSpot({0})", symbolMask); } }

    	protected override void OnExecute()
    	{
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

    		for (int i = 0; i < column.value; ++i)
            {
    			for (int j = 0; j < row.value; ++j)
    			{
    				var symbolInfo = deck.GetDeckSymbol(i, j);
                    if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
    				{
    					expectation.expectationSpots[i].Add(new Cell(i, j));
    				}
    			}
    		}
    		EndAction();
    	}
    }
}
