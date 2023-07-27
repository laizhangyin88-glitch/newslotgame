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
    public class SlotMachineEventForwarder_LinkOrigin : MonoBehaviour
    {
        protected const string ON_WIN_EVENT = "OnWinEvent";
        protected const string ON_TOTAL_WIN_EVENT = "TotalWin";
        protected const string ON_SINGLE_WIN_EVENT = "SingleWin";
        protected const string ON_SKIP_WIN_EVENT = "SkipWin";

        protected const string LINK_ORIGIN_WIN_ANIMATION_NAME = "Link Origin Win";

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
                OnSkipWin();
            else if (receivedEvent.name.Equals(ON_SINGLE_WIN_EVENT, StringComparison.Ordinal))
                OnWin((SymbolWin)receivedEvent.value);
            else if (receivedEvent.name.Equals(ON_TOTAL_WIN_EVENT, StringComparison.Ordinal))
                OnTotalWin((List<SymbolWin>)receivedEvent.value);
        }

        protected virtual void OnTotalWin(List<SymbolWin> winList)
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

                        int hashCode = symbol.GetHashCode();
                        if (!slotMachine.ignoreOverlay && slotMachine.HasOverlaySymbol(hashCode))
                        {
                            symbol = slotMachine.GetOverlaySymbol(hashCode);
                        }

                        PlayLinkOriginWin(symbol);
                    }
                }
            }
        }

        protected virtual void OnWin(SymbolWin win)
        {
            int cellCount = win.cells.Count;
            for (int i = 0; i < cellCount; ++i)
            {
                var cell = win.cells[i];
                var symbol = slotMachine.GetSymbol(cell.column, cell.row);

                int hashCode = symbol.GetHashCode();
                if (!slotMachine.ignoreOverlay && slotMachine.HasOverlaySymbol(hashCode))
                {
                    symbol = slotMachine.GetOverlaySymbol(hashCode);
                }

                PlayLinkOriginWin(symbol);
            }
        }

        protected virtual void OnSkipWin()
        {
            slotMachine.Visit((sb) =>
            {
                int hashCode = sb.GetHashCode();
                if (!slotMachine.ignoreOverlay && slotMachine.HasOverlaySymbol(hashCode))
                {
                    sb = slotMachine.GetOverlaySymbol(hashCode);
                }

                if (!sb.isPivot)
                {
                    sb.Skip();
                }
            });
        }

        private void PlayLinkOriginWin(BaseSymbol symbol)
        {
            if (!symbol.unitSymbol)
            {
                symbol.Play(LINK_ORIGIN_WIN_ANIMATION_NAME);
            }
        }
    }
}
