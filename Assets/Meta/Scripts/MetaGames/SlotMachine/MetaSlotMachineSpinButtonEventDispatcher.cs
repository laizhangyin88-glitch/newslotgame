using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas;
using NodeCanvas.Framework;
using SlotMaker;
using ParadoxNotion.Services;

namespace BagelCode
{
    [RequireComponent(typeof(MessageRouter))]
    public class MetaSlotMachineSpinButtonEventDispatcher : MonoBehaviour
    {
        private const string ON_META_SLOT_SPIN_BUTTON_EVENT = "OnMetaSlotSpinButtonEvent";

		private MessageRouter _router = null;
		protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

		private void OnEnable()
		{
			MessageDispatcher.Register(ON_META_SLOT_SPIN_BUTTON_EVENT, Dispatch);
		}

		private void OnDisable()
		{
			MessageDispatcher.UnRegister(ON_META_SLOT_SPIN_BUTTON_EVENT, Dispatch);
		}

		public void Dispatch(ParadoxNotion.EventData eventData)
		{
			router.Dispatch(ON_META_SLOT_SPIN_BUTTON_EVENT, eventData);
		}
	}
}