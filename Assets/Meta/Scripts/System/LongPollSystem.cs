using System;﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode
{

public class LongPollSystem : MonoBehaviour 
{
    public long lastReceivedId;
    
    public GraphOwner owner;
    
    private Dictionary<string, MessageDispatcher.EventDelegate> delegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
    
    private const string ON_SYSTEM_EVENT = "OnSystemEvent";
    private const string ON_SYSTEM_RESET_EVENT = "SystemReset";
    private const string ON_FINISHED_LOGIN_EVENT = "FinishedLogin";
    
    private void Awake()
    {
        delegates[ON_SYSTEM_RESET_EVENT] = OnSystemReset;
		delegates[ON_FINISHED_LOGIN_EVENT] = OnFinishedLogin;
    }
    
    private void OnEnable()
	{
		MessageDispatcher.Register(ON_SYSTEM_EVENT, OnSystemEvent);
	}

	private void OnDisable()
	{
		MessageDispatcher.UnRegister(ON_SYSTEM_EVENT, OnSystemEvent);
	}

	private void OnSystemEvent(EventData eventData)
	{
        MessageDispatcher.EventDelegate del;
		if (delegates.TryGetValue(eventData.name, out del))
			del.Invoke(eventData);
	}
    
    private void OnSystemReset(EventData eventData)
    {
        lastReceivedId = 0;
        
        if (owner.isRunning)
            owner.StopBehaviour();
    }
    
    private void OnFinishedLogin(EventData eventData)
    {
        if (!owner.isRunning)
            owner.StartBehaviour();
    }
}

}
