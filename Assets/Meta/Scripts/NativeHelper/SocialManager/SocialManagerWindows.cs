#if UNITY_WSA && !UNITY_EDITOR
using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker.Json;
using BagelCode.FacebookWSA;
using SlotMaker;

namespace BagelCode
{

public class SocialManagerWindows : ISocialManager
{
    private static string accessToken = "";

    [DllImport ("__Internal")]
    private static extern void initializeSocial([MarshalAs(UnmanagedType.LPWStr)]string name);
    public void Initialize(string name)
    {
        initializeSocial(name);
        // WSANativeFacebook.Initialise(GetAppID(), GetAppIDName());
    }

    // public void LoginFB(string callback)
    // {
    //     WSANativeFacebook.Login(new List<string>() { "public_profile", "email" }, result =>
    //     {
    //         if (result.Success)
    //         {
    //             accessToken = result.AccessToken;
    //             WSANativeFacebook.GetUserDetails(response =>
    //             {
    //                 if (response.Success)
    //                 {
    //                     WSAFacebookUser user = response.Data;
    //                     FacebookLoginResponse callbackData = new FacebookLoginResponse();
    //                     callbackData.id = user.Id;
    //                     string data = SlotSimpleJson.SerializeObject(callbackData);
    //                     SocialManager.Instance.OnLoginFBCallback(data);
    //                 }
    //                 else
    //                 {
    //                     SocialManager.Instance.OnLoginFBCallback("Error");
    //                 }
    //             });
    //         }
    //         else
    //         {
    //             SocialManager.Instance.OnLoginFBCallback("Error");
    //         }
    //     });
    // }

    // public void LogoutFB()
    // {
    //     WSANativeFacebook.Logout(false);
    // }

    // public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback)
    // {
    //     WSANativeFacebook.Share(linkUrl, title, description, imageUrl, result =>
    //     {
    //         if (result.Success)
    //         {
    //             SocialManager.Instance.OnShareFBCallback("Success");
    //         }
    //         else
    //         {
    //             Debug.LogError(result.ErrorMessage);
    //             SocialManager.Instance.OnShareFBCallback("Error");
    //         }
    //     });
    // }

    // public void InviteFB(string linkUrl, string imageUrl, string callback)
    // {
    //     // inviteFB(linkUrl, imageUrl, callback);
    // }

    [DllImport ("__Internal")]
    private static extern void loginFB([MarshalAs(UnmanagedType.LPWStr)]string callback);
    public void LoginFB(string callback)
    {
        loginFB(callback);
    }

    [DllImport ("__Internal")]
    private static extern void logoutFB();
    public void LogoutFB()
    {
        logoutFB();
    }

    [DllImport ("__Internal")]
    private static extern void shareFB([MarshalAs(UnmanagedType.LPWStr)]string linkUrl, [MarshalAs(UnmanagedType.LPWStr)]string title, [MarshalAs(UnmanagedType.LPWStr)]string description, [MarshalAs(UnmanagedType.LPWStr)]string imageUrl, [MarshalAs(UnmanagedType.LPWStr)]string callback);
    public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback)
    {
        // todo : implement encodedAction
        shareFB(linkUrl, title, description, imageUrl, callback);
    }

    [DllImport ("__Internal")]
    private static extern void inviteFB([MarshalAs(UnmanagedType.LPWStr)]string linkUrl, [MarshalAs(UnmanagedType.LPWStr)]string imageUrl, [MarshalAs(UnmanagedType.LPWStr)]string callback);
    public void InviteFB(string linkUrl, string imageUrl, string callback)
    {
        inviteFB(linkUrl, imageUrl, callback);
    }

    [DllImport ("__Internal")]
    [return: MarshalAs(UnmanagedType.LPWStr)]
    private static extern string getFacebookAppId();
    private string GetAppID()
    {
        return getFacebookAppId();
    }

    [DllImport ("__Internal")]
    [return: MarshalAs(UnmanagedType.LPWStr)]
    private static extern string getFacebookAppIdName();
    private string GetAppIDName()
    {
        return getFacebookAppIdName();
    }

    public string GetFacebookId()
    {
        return null;
    }

    // public string GetFacebookAccessToken()
    // {
    //     return accessToken;
    // }

    [DllImport ("__Internal")]
    [return: MarshalAs(UnmanagedType.LPWStr)]
    private static extern string getFacebookAccessToken();
    public string GetFacebookAccessToken()
    {
        return getFacebookAccessToken();
    }

    public void ShowFBLikePopup()
    {
        // no need to implement (just for canvas)
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
