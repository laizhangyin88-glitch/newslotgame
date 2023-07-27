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
    public class SpotWinHandler : MonoBehaviour
    {
        protected const string WIN_ANIMATION_NAME = "Win";

        private BaseSlotMachine _slotMachine;
        protected BaseSlotMachine slotMachine { get { return _slotMachine ?? (_slotMachine = GetComponent<BaseSlotMachine>()); } }

        public virtual void TotalWin(List<SymbolWin> winList)
        {
            int winCount = winList.Count;
            for (int i = 0; i < winCount; ++i)
            {
                var win = winList[i];
                int cellCount = win.cells.Count;
                for (int j = 0; j < cellCount; ++j)
                {
                    var cell = win.cells[j];
                    slotMachine.GetPivotSymbol(cell.column, cell.row).Play(WIN_ANIMATION_NAME);
                }
            }
        }

        public virtual void SingleWin(SymbolWin win)
        {
            int cellCount = win.cells.Count;
            for (int i = 0; i < cellCount; ++i)
            {
                var cell = win.cells[i];
                slotMachine.GetPivotSymbol(cell.column, cell.row).Play(WIN_ANIMATION_NAME);
            }
        }

        public virtual void SkipWin()
        {
            slotMachine.Skip();
        }
    }
}
