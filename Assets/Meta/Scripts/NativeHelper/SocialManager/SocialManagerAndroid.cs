#if UNITY_ANDROID && !UNITY_EDITOR

using System.Runtime.InteropServices;
using UnityEngine;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{

public class SocialManagerAndroid : ISocialManager
{
    private AndroidJavaClass ajc = new AndroidJavaClass("com.bagelcode.v3.FacebookManager");

    public void Initialize(string name)
    {
        ajc.CallStatic("Initialize", name);
    }

    // Methods
    public void LoginFB(string callback)
    {
        ajc.CallStatic("LoginFB", callback);
    }

    public void LogoutFB()
    {
        ajc.CallStatic("LogoutFB");

        NativeHelper.Instance.RemoveExternalUserId();
    }

    public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback)
    {
        ajc.CallStatic("ShareFB", linkUrl, title, description, imageUrl, encodedAction, callback);
    }

    public void InviteFB(string message, string title, string callback)
    {
        ajc.CallStatic("GameRequestFB", message, title, callback);
    }

    public string GetFacebookId()
    {
        // not implemented
        return "";
    }

    public string GetFacebookAccessToken()
    {
        string token = "";
        
        token = ajc.CallStatic<string>("GetAccessTokenFB");

        return token;
    }

    public void ShowFBLikePopup()
    {
        // no need to implement (just for canvas)
    }

    public bool HasPermission(string permission)
    {
        return ajc.CallStatic<bool>("HasPermissionFB", permission);
    }

    public void GetPublishPermission(string callback)
    {
        ajc.CallStatic("GetPublishPermissionFB", callback);
    }

    public void ShareMessengerFB(string linkUrl, string callback)
    {
        ajc.CallStatic("ShareMessengerFB", linkUrl, callback);
    }

    public bool GetAvailableFacebookMessenger(string linkUrl)
    {
        return ajc.CallStatic<bool>("GetAvailableFacebookMessengerFB", linkUrl);
    }
}

}

#endif
