using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
	[RequireComponent(typeof(MessageRouter))]
	public class WinEventDispatcher : MonoBehaviour
	{
		private const string ON_WIN_EVENT = "OnWinEvent";

		private MessageRouter _router = null;
		protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

		private void OnEnable()
		{
			MessageDispatcher.Register(ON_WIN_EVENT, Dispatch);
		}

		private void OnDisable()
		{
			MessageDispatcher.UnRegister(ON_WIN_EVENT, Dispatch);
		}

		public void Dispatch(EventData eventData)
		{
			router.Dispatch(ON_WIN_EVENT, eventData);
		}
	}
}
