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
    public class SlotMachineEventForwarder : MonoBehaviour
    {
        protected const string ON_WIN_EVENT = "OnWinEvent";
        protected const string ON_TOTAL_WIN_EVENT = "TotalWin";
        protected const string ON_SINGLE_WIN_EVENT = "SingleWin";
        protected const string ON_SKIP_WIN_EVENT = "SkipWin";

        protected const string ON_SLOT_EVENT = "OnSlotEvent";
        protected const string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";

        protected const string STOP_EFFECT_ANIMATION_NAME = "Stop Effect";
        protected const string WIN_ANIMATION_NAME = "Win";
        protected const string PREPARE_STOPPED_SPECIAL_SYMBOL = "PrepareStoppedSpecialSymbol";
        protected const string PREPARE_STOPPED_REEL_EVENT = "PrepareStoppedReel";
        protected const string STOPPED_SLOT_MACHINE = "StoppedSlotMachine";

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

        public virtual void OnPrepareStoppedCacadeReel(int reelIndex)
        {
            MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new EventData<int>(PREPARE_STOPPED_REEL_EVENT, slotMachine.slotIndex, reelIndex));
        }

        public virtual void OnPrepareStoppedReel(int reelIndex)
        {
            MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new EventData<int>(PREPARE_STOPPED_REEL_EVENT, slotMachine.slotIndex, reelIndex));

            var spots = ContentCustomData.GetSlotData(slotMachine.slotIndex).expectation.expectationSpots[reelIndex];
            foreach (var cell in spots)
            {
                var symbol = slotMachine.GetPivotSymbol(cell.column, cell.row);
                symbol.Play(STOP_EFFECT_ANIMATION_NAME);

                MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new EventData<BaseSymbol>(PREPARE_STOPPED_SPECIAL_SYMBOL, slotMachine.slotIndex, symbol));
            }
        }

        public virtual void OnStoppedSlotMachine()
        {
            MessageDispatcher.Dispatch(ON_SLOT_EVENT, new EventData(STOPPED_SLOT_MACHINE, slotMachine.slotIndex));
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
                        slotMachine.GetPivotSymbol(cell.column, cell.row).Play(WIN_ANIMATION_NAME);
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
                slotMachine.GetPivotSymbol(cell.column, cell.row).Play(WIN_ANIMATION_NAME);
            }
        }

        protected virtual void OnSkipWin()
        {
            slotMachine.Skip();
        }
    }
}
