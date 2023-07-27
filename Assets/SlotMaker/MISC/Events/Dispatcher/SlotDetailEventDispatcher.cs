using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
	[RequireComponent(typeof(MessageRouter))]
	public class SlotDetailEventDispatcher : MonoBehaviour
	{
		private const string ON_SLOT_DETAIL_EVENT = "OnSlotDetailEvent";

		private MessageRouter _router = null;
		protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

		private void OnEnable()
		{
			MessageDispatcher.Register(ON_SLOT_DETAIL_EVENT, Dispatch);
		}

		private void OnDisable()
		{
			MessageDispatcher.UnRegister(ON_SLOT_DETAIL_EVENT, Dispatch);
		}

		public void Dispatch(EventData eventData)
		{
			router.Dispatch(ON_SLOT_DETAIL_EVENT, eventData);
		}
	}
}
