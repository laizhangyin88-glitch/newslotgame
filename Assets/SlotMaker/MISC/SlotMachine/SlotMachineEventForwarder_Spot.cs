using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    [RequireComponent(typeof(BaseSlotMachine))]
    public class SlotMachineEventForwarder_Spot : SlotMachineEventForwarder
    {
        public override void OnPrepareStoppedReel(int reelIndex)
        {
            var slotMachine = GetComponent<BaseSlotMachine>();

            MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new EventData<int>(PREPARE_STOPPED_REEL_EVENT, slotMachine.slotIndex, reelIndex));
            var spots = ContentCustomData.GetSlotData(slotMachine.slotIndex).expectation.expectationSpots[reelIndex];
            foreach (var cell in spots)
            {
                var symbol = slotMachine.GetReel(reelIndex).GetSymbol(cell.column, cell.row);
                symbol.Play(STOP_EFFECT_ANIMATION_NAME);

                MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new EventData<BaseSymbol>(PREPARE_STOPPED_SPECIAL_SYMBOL, slotMachine.slotIndex, symbol));
            }
        }

        protected override void OnTotalWin(List<SymbolWin> winList)
        {
            var slotMachine = GetComponent<BaseSlotMachine>();
            usedCells.Clear();
            int winCount = winList.Count;
            for (int i = 0; i < winCount; ++i)
            {
                var win = winList[i];
                int cellCount = win.cells.Count;
                for (int j = 0; j < cellCount; ++j)
                {
                    Cell cell = win.cells[j];
                    if (!usedCells.Contains(cell.GetHashCode()))
                    {
                        usedCells.Add(cell.GetHashCode());
                        int column = cell.column;
                        int row = cell.row;
                        int reelIndex = row * slotMachine.ColumnCount + column;
                        slotMachine.GetPivotSymbol(reelIndex, row).Play(WIN_ANIMATION_NAME);
                    }
                }
            }
        }

        protected override void OnWin(SymbolWin win)
        {
            var slotMachine = GetComponent<BaseSlotMachine>();
            int cellCount = win.cells.Count;
            for (int i = 0; i < cellCount; ++i)
            {
                int column = win.cells[i].column;
                int row    = win.cells[i].row;
                int reelIndex = row * slotMachine.ColumnCount + column;
                slotMachine.GetPivotSymbol(reelIndex, row).Play(WIN_ANIMATION_NAME);
            }
        }
    }
}
