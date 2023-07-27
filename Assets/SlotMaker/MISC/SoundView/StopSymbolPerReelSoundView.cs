using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class StopSymbolPerReelSoundView : MonoBehaviour
    {
        public int slotIndex = 0;

        public SymbolAttribute mask;
        public List<string> symbolStopSounds;

        private int symbolAppearReelCount;
        private int prevReelIndex;

        public static readonly string ON_SLOT_EVENT = "OnSlotEvent";
        public static readonly string ON_SPIN_SLOTMACHINE_EVENT = "SpinSlotMachine";

        public static readonly string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";
        public static readonly string ON_PREPARE_STOPPED_SPECIAL_SYMBOL_EVENT = "PrepareStoppedSpecialSymbol";

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register(ON_SLOT_EVENT, OnSlotEvent);
            MessageDispatcher.Register(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_SLOT_EVENT, OnSlotEvent);
            MessageDispatcher.UnRegister(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnSlotEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotIndex) return;
            if (receivedEvent.name.Equals(ON_SPIN_SLOTMACHINE_EVENT, StringComparison.Ordinal))
            {
                OnSpinSlotMachine();
            }
        }

        protected virtual void OnSlotDetailEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotIndex) return;
            if (receivedEvent.name.Equals(ON_PREPARE_STOPPED_SPECIAL_SYMBOL_EVENT, StringComparison.Ordinal))
            {
                OnPrepareStoppedSpecialSymbol((BaseSymbol)receivedEvent.value);
            }
        }

        private void OnSpinSlotMachine()
        {
            symbolAppearReelCount = -1;
            prevReelIndex = 0;
        }

        protected virtual void OnPrepareStoppedSpecialSymbol(BaseSymbol symbol)
        {
            if (SymbolMask.HasAttribute(symbol.symbolInfo, mask))
            {
                if (symbolAppearReelCount == -1 || prevReelIndex != symbol.column)
                {
                    prevReelIndex = symbol.column;
                    symbolAppearReelCount++;
                }

                string id = symbolStopSounds[ Mathf.Min(symbolAppearReelCount, symbolStopSounds.Count - 1) ];
                GSManager.Instance.GetHandler(id).Play();
            }
        }
    }
}
