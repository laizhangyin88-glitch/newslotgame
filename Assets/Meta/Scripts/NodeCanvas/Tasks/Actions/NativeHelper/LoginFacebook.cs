using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class LoginFacebook : ActionTask
{
    public BBParameter<string> id;
    protected override string info
    {
        get
        { 
            return string.Format("Login FB"); 
        }
    }

    protected override void OnExecute()
    {   
#if !UNITY_EDITOR
        SocialManager.Instance.LoginFB(
            (FacebookLoginResponse result) =>
            {
                if (result == null)
                {
                    OpenErrorPopup();
                }
                else
                {
                    id.value = result.id;
                    SendEvent("OnLoginSuccess");
                }
            }
        );
#else
        OpenErrorPopup();
#endif
        EndAction();
    }

    private void OpenErrorPopup()
    {
        bool stringError = false;
        ErrorPopupInfo info = new ErrorPopupInfo();
        info.type = ErrorPopupType.OK;
        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_JOIN_FAIL_TO_FACEBOOK", out stringError);
        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out stringError);

        info.callback1 = delegate
        {
            SendEvent("OnLoginFailed");
        };

        ErrorPopupHandler.Instance.OpenError(info);
    }
}

}
