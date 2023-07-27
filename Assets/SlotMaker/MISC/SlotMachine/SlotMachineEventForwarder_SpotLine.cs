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
    public class SlotMachineEventForwarder_SpotLine : SlotMachineEventForwarder
    {
        public List<PayLines> payLinesList;
        private int payLineIndex = 0;

        protected const string ON_CHANGE_PAYLINE = "ChangePayLine";
        protected const string ON_TOTAL_WIN_LINE_EVENT = "TotalWinLine";
        protected const string PAYLINE_SHOW_ANIAMTION = "Show";
        protected const string PAYLINE_BLINK_ANIMATION = "Blink";

        protected override void Awake()
        {
            base.Awake();
            MessageDispatcher.Register(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MessageDispatcher.UnRegister(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected override void OnWinEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;

            base.OnWinEvent(receivedEvent);

            if (receivedEvent.name.Equals(ON_TOTAL_WIN_LINE_EVENT, StringComparison.Ordinal))
                OnTotalWinLine((List<SymbolWin>)receivedEvent.value);
        }

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

        private void OnTotalWinLine(List<SymbolWin> winList)
        {
            int winCount = winList.Count;
            for (int i = 0; i < winCount; ++i)
            {
                var win = winList[i];
                int lineIndex = (win.lineIndex ?? default(int)) - 1;
                payLinesList[payLineIndex].Play(lineIndex, PAYLINE_SHOW_ANIAMTION);
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

             int lineIndex = (win.lineIndex ?? default(int)) - 1;
            if (lineIndex >= 0)
              payLinesList[payLineIndex].Play(lineIndex, PAYLINE_BLINK_ANIMATION);
        }


        protected void OnSlotDetailEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;

            if (receivedEvent.name.Equals(ON_CHANGE_PAYLINE, StringComparison.Ordinal))
              OnChangePayLine((int)receivedEvent.value);
        }

        protected override void OnSkipWin()
        {
            base.OnSkipWin();

            payLinesList[payLineIndex].Stop();
        }

        protected void OnChangePayLine(int lineIndex)
        {
            payLineIndex = lineIndex;
        }
    }
}
