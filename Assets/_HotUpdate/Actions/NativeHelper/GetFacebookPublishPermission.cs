using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class GetFacebookPublishPermission : ActionTask
{
    public BBParameter<string> id;
    protected override string info
    {
        get
        { 
            return string.Format("Login FB with publish permission"); 
        }
    }

    protected override void OnExecute()
    {   
#if !UNITY_EDITOR
        Debug.Log("Login FB");
        SocialManager.Instance.GetPublishPermission(
            (FacebookLoginResponse result) =>
            {
                if (result == null)
                {
                    SendEvent("OnLoginError", "POPUP_JOIN_FAIL_TO_FACEBOOK");
                }
                else
                {
                    id.value = result.id;
                    SendEvent("OnLoginSuccess");
                }
                // EndAction();
            }
        );
#else
        SendEvent("OnLoginError", "POPUP_JOIN_FAIL_TO_FACEBOOK");
        // EndAction();
#endif
    }
}

}
