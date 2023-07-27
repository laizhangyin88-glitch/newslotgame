#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Facebook.MiniJSON;

namespace BagelCode
{

public class SocialManagerGameroom : ISocialManager
{
    private string socialManagerUnityObject;
    private string SUCCESS_RESPONSE = "Success";
    private string ERROR_RESPONSE = "Error";

    public void Initialize(string name)
    {
        // Fractinoal gameroom SocialManager init process previously done by UpdateFacebookId.cs
        // this exceptional procedure is for making login done before fetching devicdeId for first login
        socialManagerUnityObject = name;
    }

    private void LoginCallback(ILoginResult loginResult, string callback) {
        Debug.Log("[DEBUG] LoginCallback");
        if (FB.IsLoggedIn)
        {
            var friendsParam = new Dictionary<string, string>() {{"fields", "id"}};
            FB.API("/me/friends", HttpMethod.GET, friendsResult =>
            {
                if (friendsResult.Cancelled || !String.IsNullOrEmpty(friendsResult.Error))
                {
                    Debug.Log("[DEBUG] Failed /me/friends: " + friendsResult.Error);
                    NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, ERROR_RESPONSE);
                    return;
                }
                var paramFields = "picture.height(" + NativeHelper.PROFILE_IMAGE_MIN_SIDE + ").width(" + NativeHelper.PROFILE_IMAGE_MIN_SIDE + "){url,is_silhouette},gender,name,id,email";
                var profileParam = new Dictionary<string, string>() {{"fields", paramFields}};
                FB.API("/me", HttpMethod.GET, profileResult =>
                {
                    if (profileResult.Cancelled || !String.IsNullOrEmpty(profileResult.Error))
                    {
                        Debug.Log("[DEBUG] Failed /me: "  + profileResult.Error);
                        NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, ERROR_RESPONSE);
                        return;
                    }
                    Debug.Log("[DEBUG] friendsResult.ResultDictionary = " + Json.Serialize(friendsResult.ResultDictionary));
                    Debug.Log("[DEBUG] profileResult.ResultDictionary = " + Json.Serialize(profileResult.ResultDictionary));
                    var outputDict = new Dictionary<string, object>(profileResult.ResultDictionary);
                    var pictureDataDict = (Dictionary<string, object>)((Dictionary<string, object>)outputDict["picture"])["data"];
                    if ((bool)pictureDataDict["is_silhouette"])
                    {
                        outputDict["picture"] = "";
                    }
                    else
                    {
                        outputDict["picture"] = pictureDataDict["url"];
                    }

                    var friendIdObjList = (List<object>)(friendsResult.ResultDictionary["data"]);
                    var friendIdList = new List<string>();
                    foreach(Dictionary<string, object> idObj in friendIdObjList)
                    {
                        friendIdList.Add((string)idObj["id"]);
                    }
                    outputDict.Add("friendList", friendIdList);
                    outputDict.Add("accessToken", loginResult.AccessToken.TokenString);
                    string outputJsonString = Json.Serialize(outputDict);
                    Debug.Log("[DEBUG] outputJsonString = " + outputJsonString);
                    NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, outputJsonString);
                }, profileParam);
            }, friendsParam);
        }
        else
        {
            Debug.Log("User cancelled facebook login");
            NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, ERROR_RESPONSE);
        }
    }

    public void LoginFB(string callback)
    {
        var perms = new List<string>(){"public_profile", "user_friends", "email"};
        FB.LogInWithReadPermissions(perms, result => LoginCallback(result, callback));
    }

    public void LogoutFB()
    {
        // should not be supported (temporarily enabled)
    }

    private void ShareCallback(IShareResult result, string callback)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("[DEBUG] ShareLink Error: " + result.Error);
            NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, ERROR_RESPONSE);
        }
        else if (!String.IsNullOrEmpty(result.PostId))
        {
            // Print post identifier of the shared content
            Debug.Log("[DEBUG] PostId = " + result.PostId);
            NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, SUCCESS_RESPONSE);
        }
        else
        {
            // Share succeeded without postID
            Debug.Log("[DEBUG] ShareLink success!");
            NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, SUCCESS_RESPONSE);
        }
    }

    public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback)
    {
        string linkUrlWithParams = linkUrl + "?";
        linkUrlWithParams += "title=" + WWW.EscapeURL(title, System.Text.Encoding.UTF8);
        linkUrlWithParams += "&description=" + WWW.EscapeURL(description, System.Text.Encoding.UTF8);
        linkUrlWithParams += "&image=" + WWW.EscapeURL(imageUrl, System.Text.Encoding.UTF8);
        linkUrlWithParams += "&encoded_action=" + WWW.EscapeURL(encodedAction, System.Text.Encoding.UTF8);
        FB.ShareLink(new Uri(linkUrlWithParams), title, description, new Uri(imageUrl), result => ShareCallback(result, callback));
    }

    private void InviteCallback(IAppRequestResult result, string callback)
    {
         if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
         {
             Debug.Log("[DEBUG] Invite Error: " + result.Error);
             NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, ERROR_RESPONSE);
         }
         else
         {
             Debug.Log("[DEBUG] Invite succeses!");
             NativeHelperGameroom.UnitySendMessageWrapper(socialManagerUnityObject, callback, SUCCESS_RESPONSE);
         }
    }

    public void InviteFB(string message, string title, string callback)
    {
        var targetFilters = new List<object>();
        targetFilters.Add("app_non_users");
        FB.AppRequest(message, null, targetFilters, null, 10, "", title, result => InviteCallback(result, callback));
    }

    public string GetFacebookId()
    {
        return AccessToken.CurrentAccessToken.UserId;
    }

    public string GetFacebookAccessToken()
    {
        return AccessToken.CurrentAccessToken.TokenString;
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
