using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    [RequireComponent(typeof(BaseSlotMachine))]
    public class SlotMachineEventForwarder_WinBorder : MonoBehaviour
    {
        public ObjectPool pool;

        protected const string ON_WIN_EVENT = "OnWinEvent";
        protected const string ON_TOTAL_WIN_EVENT = "TotalWinBorder";
        protected const string ON_SINGLE_WIN_EVENT = "SingleWinBorder";
        protected const string ON_SKIP_WIN_EVENT = "SkipWinBorder";

        protected readonly HashSet<int> usedCells = new HashSet<int>();

        private MessageRouter _router;
        public MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

        private BaseSlotMachine _slotMachine;
        protected BaseSlotMachine slotMachine { get { return _slotMachine ?? (_slotMachine = GetComponent<BaseSlotMachine>()); } }

        protected virtual void Awake()
        {
            MessageDispatcher.Register(ON_WIN_EVENT, OnWinEvent);
        }

        protected virtual void OnDestroy()
        {
            MessageDispatcher.UnRegister(ON_WIN_EVENT, OnWinEvent);
        }

        public virtual void OnPrepareStoppedReel(int reelIndex) {}
        public virtual void OnStoppedSlotMachine() {}

        protected virtual void OnWinEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotMachine.slotIndex) return;

            if (receivedEvent.name.Equals(ON_SKIP_WIN_EVENT, StringComparison.Ordinal))
                OnSkipWinBorder();
            else if (receivedEvent.name.Equals(ON_SINGLE_WIN_EVENT, StringComparison.Ordinal))
                OnWinBorder((SymbolWin)receivedEvent.value);
            else if (receivedEvent.name.Equals(ON_TOTAL_WIN_EVENT, StringComparison.Ordinal))
                OnTotalWinBorder((List<SymbolWin>)receivedEvent.value);
        }

        protected virtual void OnTotalWinBorder(List<SymbolWin> winList)
        {
            usedCells.Clear();
            int winCount = winList.Count;
            for (int i = 0; i < winCount; ++i)
            {
                var win = winList[i];
                int cellCount = win.cells.Count;
                for (int j = 0; j < cellCount; ++j)
                {
                    var cell = win.cells[j];
                    if (!usedCells.Contains(cell.GetHashCode()))
                    {
                        usedCells.Add(cell.GetHashCode());
                        var symbol = slotMachine.GetSymbol(cell.column, cell.row);
                        symbol.GetComponent<WinBorderDelegator>().GetObject(pool, symbol.anchor);
                    }
                }
            }
        }

        protected virtual void OnWinBorder(SymbolWin win)
        {
            int cellCount = win.cells.Count;
            for (int i = 0; i < cellCount; ++i)
            {
                var cell = win.cells[i];
                var symbol = slotMachine.GetSymbol(cell.column, cell.row);
                symbol.GetComponent<WinBorderDelegator>().GetObject(pool, symbol.anchor);
            }
        }

        protected virtual void OnSkipWinBorder()
        {
            slotMachine.Visit((sb) =>
            {
                sb.GetComponent<WinBorderDelegator>().ReturnToPool();
            });
        }
    }
}
