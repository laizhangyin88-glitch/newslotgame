#if UNITY_WEBGL && !UNITY_EDITOR

using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{
    
public class SocialManagerCanvas : ISocialManager
{
    private string socialManagerUnityObject;
    private string SUCCESS_RESPONSE = "Success";
    private string ERROR_RESPONSE = "Error";

    public void Initialize(string name)
    {
        socialManagerUnityObject = name;
    }

    [DllImport("__Internal")]
    private static extern void LoginFacebook(string callbackObjectName, string callbackMethodName);
    public void LoginFB(string callback)
    {
        LoginFacebook(socialManagerUnityObject, callback);
    }

    public void LogoutFB()
    {
        // should not be supported
    }

    [DllImport("__Internal")]
    private static extern void ShareFacebook(string callbackObjectName, string callbackMethodName, string linkUrl);
    public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback)
    {
        string linkUrlWithParams = linkUrl + "?";
        linkUrlWithParams += "title=" + WWW.EscapeURL(title, System.Text.Encoding.UTF8);
        linkUrlWithParams += "&description=" + WWW.EscapeURL(description, System.Text.Encoding.UTF8);
        linkUrlWithParams += "&image=" + WWW.EscapeURL(imageUrl, System.Text.Encoding.UTF8);
        linkUrlWithParams += "&encoded_action=" + WWW.EscapeURL(encodedAction, System.Text.Encoding.UTF8);
        ShareFacebook(socialManagerUnityObject, callback, linkUrlWithParams);
    }

    [DllImport("__Internal")]
    private static extern void InviteFacebook(string callbackObjectName, string callbackMethodName, string message, string title);
    public void InviteFB(string message, string title, string callback)
    {
        InviteFacebook(socialManagerUnityObject, callback, message, title);
    }
    
    [DllImport("__Internal")]
    private static extern string GetLoginCachedFacebookId();
    public string GetFacebookId()
    {
        return GetLoginCachedFacebookId();
    }

    [DllImport("__Internal")]
    private static extern string GetLoginCachedFacebookAccessToken();
    public string GetFacebookAccessToken()
    {
        return GetLoginCachedFacebookAccessToken();
    }

    [DllImport("__Internal")]
    private static extern void OpenFBLikePopup();

    public void ShowFBLikePopup()
    {
        OpenFBLikePopup();
    }

    public bool HasPermission(string permission)
    {
        // todo
        return false;
    }

    public void GetPublishPermission(string callback)
    {
        // no need to implement
    }

    public void ShareMessengerFB(string linkUrl, string callback)
    {
        // no need to implement
    }

    public bool GetAvailableFacebookMessenger(string linkUrl)
    {
        return false;
    }
}

}

#endif
