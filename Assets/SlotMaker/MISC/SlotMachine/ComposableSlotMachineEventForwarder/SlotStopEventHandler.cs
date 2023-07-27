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
    public class SlotStopEventHandler : MonoBehaviour
    {
        protected const string ON_SLOT_EVENT = "OnSlotEvent";
        protected const string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";

        protected const string STOP_EFFECT_ANIMATION_NAME = "Stop Effect";
        protected const string PREPARE_STOPPED_SPECIAL_SYMBOL = "PrepareStoppedSpecialSymbol";
        protected const string PREPARE_STOPPED_REEL_EVENT = "PrepareStoppedReel";
        protected const string STOPPED_SLOT_MACHINE = "StoppedSlotMachine";

        private BaseSlotMachine _slotMachine;
        protected BaseSlotMachine slotMachine { get { return _slotMachine ?? (_slotMachine = GetComponent<BaseSlotMachine>()); } }

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
    }
}
