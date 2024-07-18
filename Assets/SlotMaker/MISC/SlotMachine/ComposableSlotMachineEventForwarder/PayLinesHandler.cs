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
    public class PayLinesHandler : MonoBehaviour
    {
        public List<PayLines> payLinesList;
        private int payLineIndex = 0;

        protected const string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";
        protected const string ON_CHANGE_PAYLINE = "ChangePayLine";
        protected const string PAYLINE_SHOW_ANIAMTION = "Show";
        protected const string PAYLINE_BLINK_ANIMATION = "Blink";

        private BaseSlotMachine _slotMachine;
        protected BaseSlotMachine slotMachine { get { return _slotMachine ?? (_slotMachine = GetComponent<BaseSlotMachine>()); } }

        protected virtual void Awake()
        {
            MessageDispatcher.Register(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnDestroy()
        {
            MessageDispatcher.UnRegister(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected void OnSlotDetailEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;
            
            if (receivedEvent.name.Equals(ON_CHANGE_PAYLINE, StringComparison.Ordinal))
              OnChangePayLine((int)receivedEvent.value);
        }

        protected void OnChangePayLine(int lineIndex)
        {
            payLineIndex = lineIndex;
        }

        public virtual void TotalWinLine(List<SymbolWin> winList)
        {
            int winCount = winList.Count;
            for (int i = 0; i < winCount; ++i)
            {
                var win = winList[i];
                int lineIndex = (win.lineIndex ?? default(int)) - 1;
                payLinesList[payLineIndex].Play(lineIndex, PAYLINE_SHOW_ANIAMTION);
            }
        }

        public virtual void SingleWin(SymbolWin win)
        {
            int lineIndex = (win.lineIndex ?? default(int)) - 1;
            if (lineIndex >= 0)
              payLinesList[payLineIndex].Play(lineIndex, PAYLINE_BLINK_ANIMATION);
        }

        public virtual void SkipWin()
        {
            payLinesList[payLineIndex].Stop();
        }
    }
}
