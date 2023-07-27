using UnityEngine;
using ParadoxNotion.Services;
using System;
using System.Collections.Generic;
using ParadoxNotion;
using SlotMaker;

namespace BagelCode
{

public class BackButtonBlocker : MonoBehaviour
{
    private const string ON_META_UI_EVENT = "OnMetaUIEvent";
    private const string ON_ENABLE_BACK_BUTTON = "OnEnableBackButton";
    private const string ON_DISABLE_BACK_BUTTON = "OnDisableBackButton";

    private void Awake()
    {
        var e = new EventData(ON_DISABLE_BACK_BUTTON);
        MessageDispatcher.Dispatch(ON_META_UI_EVENT, e);
    }

    private void OnDestroy()
    {
        var e = new EventData(ON_ENABLE_BACK_BUTTON);
        MessageDispatcher.Dispatch(ON_META_UI_EVENT, e);
    }
}

}
