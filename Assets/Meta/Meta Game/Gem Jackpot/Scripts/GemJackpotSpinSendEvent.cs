using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion;

namespace BagelCode.GemJackpot
{
    public class GemJackpotSpinSendEvent : SendEvent
    {
        public static readonly string ON_META_SLOT_SPIN_BUTTON_EVENT = "OnMetaSlotSpinButtonEvent";

        public void DispatchMetaSlotMachineSpinButtonEvent(string eventName)
        {
            var e = new ParadoxNotion.EventData(eventName, eventId);
            if (sendGlobal)
                MessageDispatcher.Dispatch(ON_META_SLOT_SPIN_BUTTON_EVENT, e);
            else
                GetComponent<MetaSlotMachineSpinButtonEventDispatcher>().Dispatch(e);
        }
    }
}