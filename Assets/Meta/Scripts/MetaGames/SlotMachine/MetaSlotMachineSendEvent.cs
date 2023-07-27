using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{
    public class MetaSlotMachineSendEvent : SendEvent
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