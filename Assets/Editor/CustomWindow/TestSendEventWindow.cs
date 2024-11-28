using BagelCode;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion;
using Sirenix.OdinInspector;
using UnityEditor;

public class TestSendEventWindow : OdinEditorWindow
{
    public enum ValueType
    {
        None,
        Number,
        String
    }
    
    [InfoBox("使用EventSender.SendGlobalEvent发送全局事件\n支持事件值（空，数字，字符串）")]
    [ValueDropdown("EventTypeList", AppendNextDrawer = true)]
    public string eventType;
    public string eventName;

    public ValueType valueType = ValueType.None;
    [ShowIf("DisplayFieldEventValue")]
    public string eventValue;

    private bool DisplayFieldEventValue=> valueType != ValueType.None;
    private string[] EventTypeList => new string[]{
        "OnCustomEvent",
        "OnContentUIEvent",
        "OnSlotEvent",
        "OnCreditEvent",
        "OnSymbolEvent",
        "OnWinEvent",
        "OnSpinButtonEvent",
        "OnMachineButtonEvent",
        "OnContentEvent",
    };

    [Button]
    public void SendEvent()
    {
        if (string.IsNullOrEmpty(eventType))
            return;

        if (string.IsNullOrEmpty(eventName))
            return;

        if (Application.isPlaying == false)
            return;

        switch (valueType)
        {
            case ValueType.None:
                EventSender.SendGlobalEvent(eventType, new EventData(eventName));
                break;
            case ValueType.Number:
                if (ConvertUtility.TryConvert<float>(eventValue, out float v))
                {
                    EventSender.SendGlobalEvent(eventType, new EventData<float>(eventName, v));
                }
                break;
            case ValueType.String:
                EventSender.SendGlobalEvent(eventType, new EventData<string>(eventName, eventValue));
                break;
            default:
                break;
        }
    }

    [MenuItem("Tools/Test/发送全局事件")]
    public static void OpenWindow()
    {
        GetWindow<TestSendEventWindow>().Show();
    }
}
