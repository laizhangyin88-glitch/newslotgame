using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker.Json;

namespace BagelCode
{

public class SocialManager : SlotMaker.MonoWeakSingleton<SocialManager>
{
    private ISocialManager delegator;
    private string facebookLongLivedAccessToken = string.Empty;

    public void Awake()
    {
        //#if UNITY_IPHONE && !UNITY_EDITOR
        //delegator = new SocialManagerIOS();
        //#elif UNITY_ANDROID && !UNITY_EDITOR
        //delegator = new SocialManagerAndroid();
        //#elif UNITY_WSA && !UNITY_EDITOR
        //delegator = new SocialManagerWindows();
        //#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        //delegator = new SocialManagerGameroom();
        //#elif UNITY_WEBGL && !UNITY_EDITOR
        //delegator = new SocialManagerCanvas();
        //#else
        delegator = new SocialManagerUnity();
       // #endif
    }

    public void Initialize()
    {
        delegator.Initialize(gameObject.name);
    }
    private System.Action<FacebookLoginResponse> loginFBCallback;
    public void LoginFB(System.Action<FacebookLoginResponse> callback)
    {
        loginFBCallback = callback;
        delegator.LoginFB("OnLoginFBCallback");
    }

    public void OnLoginFBCallback(string json)
    {
        if (loginFBCallback != null)
        {
            FacebookLoginResponse result = null;
            if (!json.Equals("Error"))
            {
                result = SlotSimpleJson.DeserializeObject<FacebookLoginResponse>(json);
            }
            loginFBCallback(result);
            loginFBCallback = null;
        }
    }

    public void LogoutFB()
    {
        delegator.LogoutFB();
    }

    private System.Action<string> shareFBCallback;
    public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, System.Action<string> callback)
    {
        shareFBCallback = callback;
        delegator.ShareFB(linkUrl, title, description, imageUrl, encodedAction, "OnShareFBCallback");
    }

    public void OnShareFBCallback(string message)
    {
        Debug.Log("OnShareFBCallback: " + message);
        if (shareFBCallback != null)
        {
            shareFBCallback(message);
            shareFBCallback = null;
        }
    }

    private System.Action<string> inviteFBCallback;
    public void InviteFB(string message, string title, System.Action<string> callback)
    {
        inviteFBCallback = callback;
        delegator.InviteFB(message, title, "OnInviteFBCallback");
    }

    public void OnInviteFBCallback(string message)
    {
        Debug.Log("OnInviteFBCallback: " + message);
        if (inviteFBCallback != null)
        {
            inviteFBCallback(message);
            inviteFBCallback = null;
        }
    }

    public string GetFacebookId()
    {
        return delegator.GetFacebookId();
    }

    public string GetFacebookAccessToken()
    {
        if (facebookLongLivedAccessToken == string.Empty)
            return delegator.GetFacebookAccessToken();
        else
            return facebookLongLivedAccessToken;
    }

    public void SetFacebookLongLivedAccessToken(string token)
    {
        facebookLongLivedAccessToken = token;
    }

    public void ShowFBLikePopup()
    {
        delegator.ShowFBLikePopup();
    }

    public bool HasPermission(string permission)
    {
        return delegator.HasPermission(permission);
    }

    public void GetPublishPermission(System.Action<FacebookLoginResponse> callback)
    {
        delegator.GetPublishPermission("loginFBCallback");
        loginFBCallback = callback;
    }

    private System.Action<string> shareMessengerFBCallback;
    public void ShareMessengerFB(string linkUrl, System.Action<string> callback)
    {
        shareMessengerFBCallback = callback;
        delegator.ShareMessengerFB(linkUrl, "OnShareMessengerFBCallback");
    }

    public void OnShareMessengerFBCallback(string message)
    {
        Debug.Log("OnShareMessengerFBCallback: " + message);
        if (shareMessengerFBCallback != null)
        {
            shareMessengerFBCallback(message);
            shareMessengerFBCallback = null;
        }
    }

    public bool GetAvailableFacebookMessenger(string linkUrl)
    {
        return delegator.GetAvailableFacebookMessenger(linkUrl);
    }
}
}
