using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/NativeHelper")]
public class UpdateFacebookId : ActionTask
{
     protected override string info
    {
        get
        {
            return string.Format("Update FacebookId for SocialManager");
        }
    }

    protected override void OnExecute()
    {
//#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
//        InitAndLogin();
//#else
        EndAction();
//#endif
    }
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    private void LoginIfNeeded()
    {
        if (Facebook.Unity.FB.IsLoggedIn && Facebook.Unity.AccessToken.CurrentAccessToken != null)
        {
            var accessToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            if (System.DateTime.Compare(accessToken.ExpirationTime, System.DateTime.Now) > 0)
            {
                EndAction();
                return;
            }
        }
        var perms = new List<string>(){"public_profile", "user_friends", "email"};
        Facebook.Unity.FB.LogInWithReadPermissions(perms,
        (loginResult) =>
        {
            if (!Facebook.Unity.FB.IsLoggedIn || loginResult.Cancelled || !string.IsNullOrEmpty(loginResult.Error))
            {
                bool stringError = false;
                ErrorPopupInfo info = new ErrorPopupInfo();
                info.type = ErrorPopupType.OK;
                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_JOIN_FAIL_TO_FACEBOOK", out stringError);
                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                info.callback1 = delegate
                {
                    InitAndLogin();
                };

                ErrorPopupHandler.Instance.OpenError(info);
            }
            else
            {
                EndAction();
                return;
            }
        });
    }

    private void InitAndLogin()
    {
        if (!Facebook.Unity.FB.IsInitialized)
        {
            Facebook.Unity.FB.Init(
            () =>
            {
                if (Facebook.Unity.FB.IsInitialized)
                {
                    Facebook.Unity.FB.ActivateApp();
                }
                LoginIfNeeded();
            }, null);
        }
        else
        {
            LoginIfNeeded();
        }
    }
#endif
}

}
