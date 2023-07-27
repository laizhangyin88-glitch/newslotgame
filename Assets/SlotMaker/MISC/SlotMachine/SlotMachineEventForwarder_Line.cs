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
    public class SlotMachineEventForwarder_Line : SlotMachineEventForwarder
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

        protected override void OnTotalWin(List<SymbolWin> winList)
        {
            base.OnTotalWin(winList);

            OnTotalWinLine(winList);
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
            base.OnWin(win);

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
