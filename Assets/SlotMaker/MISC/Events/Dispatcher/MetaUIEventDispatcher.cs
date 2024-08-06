using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace SlotMaker
{
    /// <summary>
    /// MetaUIEvent事件派发
    /// </summary>
    /// <remarks>
    /// MetaUIEvent是一个单独的事件系统，由MessageDispatcher派发OnMetaUIEvent类型事件
    /// 通过此脚本再派发到OnMetaUIEvent事件系统
    /// MetaUIEvent是用在NodeCanvas上的事件系统
    /// </remarks>
	[RequireComponent(typeof(MessageRouter))]
	public class MetaUIEventDispatcher : MonoBehaviour
	{
		private const string ON_META_UI_EVENT = "OnMetaUIEvent";

		private MessageRouter _router = null;
		protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

		private void OnEnable()
		{
			MessageDispatcher.Register(ON_META_UI_EVENT, Dispatch);
		}

		private void OnDisable()
		{
			MessageDispatcher.UnRegister(ON_META_UI_EVENT, Dispatch);
		}

		public void Dispatch(EventData eventData)
		{
			router.Dispatch(ON_META_UI_EVENT, eventData);
		}
	}
}
