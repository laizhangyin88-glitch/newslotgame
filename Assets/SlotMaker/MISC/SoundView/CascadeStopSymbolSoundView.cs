using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    public class CascadeStopSymbolSoundView : MonoBehaviour
    {
        public int slotIndex = 0;

        public List<string> symbolStopSounds;

        public static readonly string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";
        public static readonly string ON_PREPARE_STOPPED_SYMBOL_EVENT = "PrepareStoppedSymbol";

        protected virtual void OnEnable()
        {
            MessageDispatcher.Register(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnDisable()
        {
            MessageDispatcher.UnRegister(ON_SLOT_DETAIL_EVENT, OnSlotDetailEvent);
        }

        protected virtual void OnSlotDetailEvent(EventData receivedEvent)
        {
            if (receivedEvent.id != slotIndex) return;
            if (receivedEvent.name.Equals(ON_PREPARE_STOPPED_SYMBOL_EVENT, StringComparison.Ordinal))
                OnPrepareStoppedSymbol((BaseSymbol)receivedEvent.value);
        }

        protected virtual void OnPrepareStoppedSymbol(BaseSymbol symbol)
        {
            string id = symbolStopSounds[ Mathf.Min(symbol.column, symbolStopSounds.Count - 1) ];
            GSManager.Instance.GetHandler(id).Play();
        }
    }
}
