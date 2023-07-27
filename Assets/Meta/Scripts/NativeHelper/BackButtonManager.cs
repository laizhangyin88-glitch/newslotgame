using UnityEngine;
using ParadoxNotion.Services;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;

namespace BagelCode
{

public class BackButtonManager : SlotMaker.MonoWeakSingleton<BackButtonManager>
{
    public const string ON_ENABLE_BACK_BUTTON = "OnEnableBackButton";
    public const string ON_DISABLE_BACK_BUTTON = "OnDisableBackButton";

    private long lastBackButtonClickTime = 0;
    private const long ANDROID_BACK_BUTTON_DELAY = 500; // 0.5sec
    private Dictionary<int, Action> listenerDict = new Dictionary<int, Action>();
    private List<Action> listenerStack = new List<Action>();

    private bool enableBackButton = true;

    private Dictionary<string, MessageDispatcher.EventDelegate> metaUIEventDelegates = new Dictionary<string, MessageDispatcher.EventDelegate>();
    private const string ON_META_UI_EVENT = "OnMetaUIEvent";

    private void Awake()
    {
        metaUIEventDelegates[ON_ENABLE_BACK_BUTTON] = OnEnableBackButton;
        metaUIEventDelegates[ON_DISABLE_BACK_BUTTON] = OnDisableBackButton;

        MessageDispatcher.Register(ON_META_UI_EVENT, OnMetaUIEvent);
    }

    protected override void OnDestroy()
    {
        MessageDispatcher.UnRegister(ON_META_UI_EVENT, OnMetaUIEvent);
        base.OnDestroy();
    }

    private void OnMetaUIEvent(EventData eventData)
    {
        MessageDispatcher.EventDelegate del;
        if (metaUIEventDelegates.TryGetValue(eventData.name, out del))
            del.Invoke(eventData);
    }

    private void OnEnableBackButton(EventData eventData)
    {
        enableBackButton = true;
    }

    private void OnDisableBackButton(EventData eventData)
    {
        enableBackButton = false;
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        if(!enableBackButton) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            long currentTime                = BagelCode.TimeUtils.GetTimeStamp();
            long backButtonClickableTime    = lastBackButtonClickTime + ANDROID_BACK_BUTTON_DELAY;
            if (currentTime > backButtonClickableTime)
            {
                lastBackButtonClickTime = currentTime;
                OnBackButtonDown();
            }
        }
#endif
    }

    public void OnBackButtonDown()
    {
        // Check Playing Tutorial
        var playingTutorial = SlotMaker.BlackboardUtils.GetOrCreateVariable<bool>(SlotMaker.MainBlackboard.Get(), "/IsPlayingTutorial");
        if (playingTutorial != null && playingTutorial.value == true)
        {
            return;
        }

        if (listenerStack.Count > 0)
        {
            listenerStack[listenerStack.Count-1].Invoke();
        }
    }

    public void SubscribeBackButton(Action action, int ownerHash)
    {
        if(listenerDict.ContainsKey(ownerHash))
            return;

        listenerDict.Add(ownerHash, action);
        listenerStack.Add(action);
    }

    public void UnSubscribeBackButton(int ownerHash)
    {
        if(!listenerDict.ContainsKey(ownerHash))
            return;

        listenerStack.Remove(listenerDict[ownerHash]);
        listenerDict.Remove(ownerHash);
    }

    public void ClearListenerStack()
    {
        listenerDict.Clear();
        listenerStack.Clear();
    }

}

}
