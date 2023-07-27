using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using ParadoxNotion;
using ParadoxNotion.Services;

namespace BagelCode
{

[RequireComponent(typeof(MessageRouter))]
public class SystemEventDispatcher : MonoBehaviour
{
	private const string ON_SYSTEM_EVENT = "OnSystemEvent";

	private MessageRouter _router = null;
	protected MessageRouter router { get { return _router ?? (_router = GetComponent<MessageRouter>()); } }

	private void OnEnable()
	{
		MessageDispatcher.Register(ON_SYSTEM_EVENT, Dispatch);
	}

	private void OnDisable()
	{
		MessageDispatcher.UnRegister(ON_SYSTEM_EVENT, Dispatch);
	}

	public void Dispatch(EventData eventData)
	{
		router.Dispatch(ON_SYSTEM_EVENT, eventData);
	}
}

}
