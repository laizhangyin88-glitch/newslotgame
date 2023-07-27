using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.Contents
{
    [Category("★ BagelCode/Contents")]
    public class UpdateExpectationSpecialSpot : ActionTask
    {
        public BBParameter<int> slotIndex = 0;
        public BBParameter<List<Cell>> specialSpots; // 7 (2, 1)
        public SymbolAttribute symbolMask;

    	protected override string info { get { return string.Format("UpdateExpectationSpecialSpot({0})", symbolMask); } }

    	protected override void OnExecute()
    	{
            var slotData = ContentCustomData.GetSlotData(slotIndex.value);
            var deck = slotData.deck;
            var expectation = slotData.expectation;

            for (int i = 0; i < specialSpots.value.Count; ++i)
            {
                int column = specialSpots.value[i].column;
                int row    = specialSpots.value[i].row;

                var symbolInfo = deck.GetDeckSymbol(column, row);
                if (SymbolMask.HasAttribute(symbolInfo, symbolMask))
                {
                    expectation.expectationSpots[column].Add(new Cell(column, row));
                }
            }
    		EndAction();
    	}
    }
}
