using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using BagelCode.Internal;
using SlotMaker;

namespace BagelCode
{

public class ErrorPopupHandler : SlotMaker.MonoWeakSingleton<ErrorPopupHandler>
{
    public bool isBlock = false;
    public GraphOwner handlerOwner;
    public Blackboard handlerBB;

    public List<ErrorPopupInfo> errorList = new List<ErrorPopupInfo>();

    public void Init()
    {
        // errorList.Clear();
        isBlock = false;
    }

    public void BloackErrorHandler()
    {
        isBlock = true;
    }

    public void OpenError(ErrorPopupInfo info)
    {
        errorList.Add(info);

        // Open Popup Event..
        ShowPopup();
    }

    public void ShowPopup()
    {
        if(errorList.Count == 0) return;

        BlackboardUtils.SetOrCreateValue(handlerBB, "popupTitle", errorList[0].title);
        BlackboardUtils.SetOrCreateValue(handlerBB, "popupText", errorList[0].text);
        BlackboardUtils.SetOrCreateValue(handlerBB, "popupButtonText1", errorList[0].buttonText1);
        BlackboardUtils.SetOrCreateValue(handlerBB, "popupButtonText2", errorList[0].buttonText2);

        BlackboardUtils.SetOrCreateValue(handlerBB, "useXButton", errorList[0].useXButton);

        BlackboardUtils.SetOrCreateValue(handlerBB, "buttonAutoClose1", errorList[0].buttonAutoClose1);
        BlackboardUtils.SetOrCreateValue(handlerBB, "buttonAutoClose2", errorList[0].buttonAutoClose2);

            switch (errorList[0].type)
            {
                case ErrorPopupType.TextOnly:
                    handlerOwner.SendEvent("OnOpenText");
                    break;
                case ErrorPopupType.OK:
                    handlerOwner.SendEvent("OnOpenOk");
                    break;
                case ErrorPopupType.OkWithTitle:
                    handlerOwner.SendEvent("OnOpenOkWithTitle");
                    break;
                case ErrorPopupType.YesNo:
                    handlerOwner.SendEvent("OnOpenYesNo");
                    break;
                case ErrorPopupType.SystemReset:
                    handlerOwner.SendEvent("OnOpenSystemReset");
                    break;
                case ErrorPopupType.YesNoClose:
                    handlerOwner.SendEvent("OnOpenYesNoClose");
                    break;
            }
    }

    public void ExecuteCallback(int buttonIndex)
    {
        if(errorList.Count > 0)
        {
            if(buttonIndex == 0 && errorList[0].callback1 != null)
            {
                errorList[0].callback1();
            }
            else if(buttonIndex == 1 && errorList[0].callback2 != null)
            {
                errorList[0].callback2();
            }
            else if(buttonIndex == 2 && errorList[0].callbackX != null)
            {
                errorList[0].callbackX();
            }
        }
    }

    public void CloseErrorPopup()
    {
        if(errorList.Count > 0)
        {
            errorList.RemoveAt(0);
        }
    }

    public void ClearErrorList()
    {
        errorList.Clear();
    }


    /// <summary>
    /// This new API is only used in the script.
    /// </summary>
    /// <param name="eventName"></param>
    public void ClosePopup(string eventName = null)
    {
            if (errorList.Count > 0)
            {
                switch (errorList[0].type)
                {
                    case ErrorPopupType.TextOnly:
                        handlerOwner.SendEvent(eventName ?? "CloseTextOnlyPopup");
                        break;
                    case ErrorPopupType.OK:
                        handlerOwner.SendEvent(eventName ?? "OnClose");
                        break;
                    case ErrorPopupType.OkWithTitle:
                        handlerOwner.SendEvent(eventName ?? "OnXClose"); //OnOK
                        break;
                    case ErrorPopupType.YesNo:
                        handlerOwner.SendEvent(eventName ?? "OnNo");
                        break;
                    case ErrorPopupType.SystemReset:
                        handlerOwner.SendEvent("OnClose");
                        break;
                }
                EventSender.SendGlobalEvent("OnClose");
            }
    }

    }

}
