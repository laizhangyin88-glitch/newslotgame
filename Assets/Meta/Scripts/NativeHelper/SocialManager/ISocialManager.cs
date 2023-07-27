using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{

public interface ISocialManager
{
    void Initialize(string name);
    void LoginFB(string callback);
    void LogoutFB();
    void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback);
    void InviteFB(string message, string title, string callback);
    string GetFacebookId();
    string GetFacebookAccessToken();
    void ShowFBLikePopup();
    bool HasPermission(string permission);
    void GetPublishPermission(string callback);
    void ShareMessengerFB(string linkUrl, string callback);
    bool GetAvailableFacebookMessenger(string linkUrl);
}

public class FacebookLoginResponse
{
    public string id;
}
}
