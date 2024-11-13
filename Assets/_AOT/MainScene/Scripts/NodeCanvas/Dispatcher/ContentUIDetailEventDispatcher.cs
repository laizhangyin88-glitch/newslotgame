using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
	[RequireComponent(typeof(MessageRouter))]
	public class ContentUIDetailEventDispatcher : MonoBehaviour
	{
		private const string ON_CONTENT_UI_DETAIL_EVENT = "OnContentUIDetailEvent";

		private MessageRouter _router = null;
		protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

		private void OnEnable()
		{
			MessageDispatcher.Register(ON_CONTENT_UI_DETAIL_EVENT, Dispatch);
		}

		private void OnDisable()
		{
			MessageDispatcher.UnRegister(ON_CONTENT_UI_DETAIL_EVENT, Dispatch);
		}

		public void Dispatch(EventData eventData)
		{
			router.Dispatch(ON_CONTENT_UI_DETAIL_EVENT, eventData);
		}
	}
}
