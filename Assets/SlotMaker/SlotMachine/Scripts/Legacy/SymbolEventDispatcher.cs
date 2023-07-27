using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker
{
	[RequireComponent(typeof(GraphOwner))]
	[RequireComponent(typeof(MessageRouter))]
	public class SymbolEventDispatcher : MonoBehaviour
	{
		private const string ON_SYMBOL_EVENT = "OnSymbolEvent";

		private GraphOwner _owner = null;
		protected GraphOwner owner { get { return _owner ?? (_owner = GetComponent<GraphOwner>()); } }

		private MessageRouter _router = null;
		protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

		private void Awake()
		{
			owner.StartBehaviour(false, null);
			owner.UpdateBehaviour();

			MessageDispatcher.Register(ON_SYMBOL_EVENT, Dispatch);
		}

		private void OnDestroy()
		{
			MessageDispatcher.UnRegister(ON_SYMBOL_EVENT, Dispatch);
		}

		public void Dispatch(EventData eventData)
		{
			if (enabled)
	        {
				router.Dispatch(ON_SYMBOL_EVENT, eventData);
	            owner.UpdateBehaviour();
	        }
		}
	}
}
