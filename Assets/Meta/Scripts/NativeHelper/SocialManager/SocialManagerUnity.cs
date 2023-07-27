using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{

public class SocialManagerUnity : ISocialManager
{
    public void Initialize(string name)
    {

    }

    public void LoginFB(string callback)
    {
        // for debug gameroom
        // GameObject socialManagerObject = GameObject.Find("NativeHelper");
        // socialManagerObject.SendMessage(callback, "Error");
    }

    public void LogoutFB()
    {
        
    }

    public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback)
    {

    }

    public void InviteFB(string message, string title, string callback)
    {

    }

    public string GetFacebookId()
    {
        return "";
    }

    public string GetFacebookAccessToken()
    {
        return "";
    }

    public void ShowFBLikePopup()
    {
        // no need to implement (just for canvas)
    }

    public bool HasPermission(string permission)
    {
        // no need to implement
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
        return true;
    }
}

}
