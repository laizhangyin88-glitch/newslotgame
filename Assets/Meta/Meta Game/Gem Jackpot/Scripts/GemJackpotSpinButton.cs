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
    public class GemJackpotSpinButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private GameObject slotMachine;

        private static readonly string ON_META_SLOT_SPIN_BUTTON_EVENT = "OnMetaSlotSpinButtonEvent";

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Spin();
        }

        private void Spin()
        {
            if (slotMachine == null) slotMachine = GemJackpotUtils.GetSlotMachine();

            if (slotMachine != null)
                slotMachine.GetComponent<GemJackpotSpinSendEvent>().DispatchMetaSlotMachineSpinButtonEvent("Skip");
        }
    }
}