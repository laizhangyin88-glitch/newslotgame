#if UNITY_IPHONE && !UNITY_EDITOR

using System.Runtime.InteropServices;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{

    public class SocialManagerIOS : ISocialManager
    {
        //[DllImport ("__Internal")]
        //private static extern void _initializeFacebookController(string name);
        public void Initialize(string name)
        {
            //_initializeFacebookController(name);
        }

        //[DllImport ("__Internal")]
        //private static extern void _FBLogin(string callback);
        public void LoginFB(string callback)
        {
            //_FBLogin(callback);
        }

        //[DllImport ("__Internal")]
        //private static extern void _FBLogout();
        public void LogoutFB()
        {
            //_FBLogout();

            //NativeHelper.Instance.RemoveExternalUserId();
        }

        //[DllImport ("__Internal")]
        //private static extern void _FBShare(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback);
        public void ShareFB(string linkUrl, string title, string description, string imageUrl, string encodedAction, string callback)
        {
            //_FBShare(linkUrl, title, description, imageUrl, encodedAction, callback);
        }

        //[DllImport ("__Internal")]
        //private static extern void _FBGameRequest(string message, string title, string callback);
        public void InviteFB(string message, string title, string callback)
        {
            //_FBGameRequest(message, title, callback);
        }

        public string GetFacebookId()
        {
            // not implemented
            return "";
        }

        //[DllImport ("__Internal")]
        //private static extern string _FBGetAccessToken();
        public string GetFacebookAccessToken()
        {
            //string token = _FBGetAccessToken();

            //if (token == null) token = "";

            //return token;
            return "";
        }

        public void ShowFBLikePopup()
        {
            // no need to implement (just for canvas)
        }

        //[DllImport ("__Internal")]
        //private static extern bool _FBHasPermission(string permission);
        public bool HasPermission(string permission)
        {
            //return _FBHasPermission(permission);
            return false;
        }

        //[DllImport ("__Internal")]
        //private static extern void _FBGetPublishPermission(string callback);
        public void GetPublishPermission(string callback)
        {
            //_FBGetPublishPermission(callback);
        }

        //[DllImport ("__Internal")]
        //private static extern void _FBShareMessenger(string linkUrl, string callback);
        public void ShareMessengerFB(string linkUrl, string callback)
        {
            //_FBShareMessenger(linkUrl, callback);
        }

        //[DllImport ("__Internal")]
        //private static extern bool _FBGetAvailableFacebookMessenger(string linkUrl);
        public bool GetAvailableFacebookMessenger(string linkUrl)
        {
            //return _FBGetAvailableFacebookMessenger(linkUrl);
            return true;
        }
    }

}

#endif
