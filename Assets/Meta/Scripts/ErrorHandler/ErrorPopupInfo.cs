using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BagelCode
{

public enum ErrorPopupType
{
    None = 0,
    TextOnly,
    OK,
    OkWithTitle,
    YesNo,
    SystemReset
}

public class ErrorPopupInfo
{
    public ErrorPopupType type;

    public string title;
    public string text;
    public string buttonText1;
    public string buttonText2;

    public bool useXButton = false;

    public bool buttonAutoClose1 = true;
    public bool buttonAutoClose2 = true;

    public UnityAction callback1;
    public UnityAction callback2;
    public UnityAction callbackX;
}

}
