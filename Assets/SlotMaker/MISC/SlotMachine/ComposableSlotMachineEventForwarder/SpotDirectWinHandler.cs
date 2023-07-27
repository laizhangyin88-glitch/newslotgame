using System.Collections.Generic;
using UnityEngine;


namespace SlotMaker
{
    [RequireComponent(typeof(BaseSlotMachine))]
    public class SpotDirectWinHandler : SpotWinHandler
    {
        protected const string DIRECT_WIN_ANIMATION_NAME = "Direct Win";

        public override void TotalWin(List<SymbolWin> winList)
        {
            int winCount = winList.Count;
            for (int i = 0; i < winCount; ++i)
            {
                var win = winList[i];
                int cellCount = win.cells.Count;
                for (int j = 0; j < cellCount; ++j)
                {
                    var cell = win.cells[j];
                    GetSymbol(cell.column, cell.row).Play(DIRECT_WIN_ANIMATION_NAME);
                }
            }
        }

        public override void SingleWin(SymbolWin win)
        {
            int cellCount = win.cells.Count;
            for (int i = 0; i < cellCount; ++i)
            {
                var cell = win.cells[i];
                GetSymbol(cell.column, cell.row).Play(DIRECT_WIN_ANIMATION_NAME);
            }
        }

        public override void SkipWin()
        {
            slotMachine.Visit((sb) => 
            {
                sb.Skip();
            });
        }

        // Note: Override this method in a derived class to take into account symbols in slot overlay.
        protected virtual BaseSymbol GetSymbol(int column, int row)
        {
            return slotMachine.GetSymbol(column, row);
        }
    }
}
