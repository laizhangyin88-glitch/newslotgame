using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    [Serializable]
    public class SymbolWinEvent : UnityEvent<SymbolWin> {}
    [Serializable]
    public class SymbolWinListEvent : UnityEvent<List<SymbolWin>> {}

    [RequireComponent(typeof(BaseSlotMachine))]
    public class SlotWinEventForwarder : MonoBehaviour
    {
        protected const string ON_WIN_EVENT = "OnWinEvent";
        protected const string ON_TOTAL_WIN_EVENT = "TotalWin";
        protected const string ON_SINGLE_WIN_EVENT = "SingleWin";
        protected const string ON_SKIP_WIN_EVENT = "SkipWin";
        protected const string ON_TOTAL_WIN_LINE_EVENT = "TotalWinLine";

        private BaseSlotMachine _slotMachine;
        protected BaseSlotMachine slotMachine { get { return _slotMachine ?? (_slotMachine = GetComponent<BaseSlotMachine>()); } }

        public UnityEvent onSkipWin;
        public SymbolWinEvent onSingleWin;
        public SymbolWinListEvent onTotalWin;
        public SymbolWinListEvent onTotalWinLine;

        protected virtual void Awake()
        {
            MessageDispatcher.Register(ON_WIN_EVENT, OnWinEvent);
        }

        protected virtual void OnDestroy()
        {
            MessageDispatcher.UnRegister(ON_WIN_EVENT, OnWinEvent);
        }

        protected virtual void OnWinEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;

            if (receivedEvent.name.Equals(ON_SKIP_WIN_EVENT, StringComparison.Ordinal))
                OnSkipWin();
            else if (receivedEvent.name.Equals(ON_SINGLE_WIN_EVENT, StringComparison.Ordinal))
                OnWin((SymbolWin)receivedEvent.value);
            else if (receivedEvent.name.Equals(ON_TOTAL_WIN_EVENT, StringComparison.Ordinal))
                OnTotalWin((List<SymbolWin>)receivedEvent.value);
            else if (receivedEvent.name.Equals(ON_TOTAL_WIN_LINE_EVENT, StringComparison.Ordinal))
                OnTotalWinLine((List<SymbolWin>)receivedEvent.value);
        }

        protected virtual void OnTotalWinLine(List<SymbolWin> winList)
        {
            onTotalWinLine.Invoke(winList);
        }

        protected virtual void OnTotalWin(List<SymbolWin> winList)
        {
            onTotalWin.Invoke(winList);
        }

        protected virtual void OnWin(SymbolWin win)
        {
            onSingleWin.Invoke(win);
        }

        protected virtual void OnSkipWin()
        {
            onSkipWin.Invoke();
        }
    }
}
