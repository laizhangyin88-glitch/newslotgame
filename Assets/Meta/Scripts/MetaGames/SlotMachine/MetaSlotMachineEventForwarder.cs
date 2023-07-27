using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker;
using ParadoxNotion;

namespace BagelCode
{
    [RequireComponent(typeof(BaseSlotMachine))]
    public class MetaSlotMachineEventForwarder : SlotMachineEventForwarder
    {
        protected const string STOPPED_META_SLOT_MACHINE = "StoppedMetaSlotMachine";
        protected const string TOTAL_META_SLOT_MACHINE_WIN = "TotalMetaSlotMachineWin";

        public override void OnPrepareStoppedReel(int reelIndex)
        {
            MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new EventData<int>(PREPARE_STOPPED_REEL_EVENT, slotMachine.slotIndex, reelIndex));

            var spots = MetaSlotMachineContentCustomData.GetSlotData(slotMachine.slotIndex).expectation.expectationSpots[reelIndex];
            foreach (var cell in spots)
            {
                var symbol = slotMachine.GetPivotSymbol(cell.column, cell.row);
                symbol.Play(STOP_EFFECT_ANIMATION_NAME);

                MessageDispatcher.Dispatch(ON_SLOT_DETAIL_EVENT, new EventData<BaseSymbol>(PREPARE_STOPPED_SPECIAL_SYMBOL, slotMachine.slotIndex, symbol));
            }
        }

        public override void OnStoppedSlotMachine()
        {
            MessageDispatcher.Dispatch(ON_SLOT_EVENT, new EventData(STOPPED_META_SLOT_MACHINE, slotMachine.slotIndex));
        }

        protected override void OnWinEvent(EventData receivedEvent)
        {
            //base.OnWinEvent(receivedEvent);

            if (receivedEvent.id != slotMachine.slotIndex) return;

            if (receivedEvent.name.Equals(TOTAL_META_SLOT_MACHINE_WIN, StringComparison.Ordinal))
                OnTotalWin((List<SymbolWin>)receivedEvent.value);
        }
    }
}