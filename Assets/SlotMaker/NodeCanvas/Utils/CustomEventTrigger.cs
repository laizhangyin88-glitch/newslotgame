using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NodeCanvas.Framework;
using ParadoxNotion.Services;

namespace SlotMaker
{

public class CustomEventTrigger : MonoBehaviour
{
	public GraphOwner owner;
	public string eventName;

	public UnityEvent onReceivedEvent;

	private MessageRouter _router;
	private MessageRouter router
	{
		get
		{
			if (_router == null)
			{
				_router = owner.GetComponent<MessageRouter>();
				if (_router == null)
					_router = owner.gameObject.AddComponent<MessageRouter>();
			}
			return _router;
		}
	}

	private void OnEnable()
	{
		router.RegisterCallback(eventName, OnReceivedEvent);
	}

	private void OnDisable()
	{
		router.UnRegister((Action)OnReceivedEvent);
	}

	private void OnReceivedEvent()
	{
		onReceivedEvent.Invoke();
	}
}

}
