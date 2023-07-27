#if UNITY_WSA && !UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace BagelCode.FacebookWSA
{
    public static class WSANativeFacebook
    {
        private static readonly WSAFacebookApi _facebookApi = new WSAFacebookApi();

        public static bool IsLoggedIn
        {
            get { return _facebookApi.IsLoggedIn; }
        }

        public static void Initialise(string facebookAppId, string packageSID)
        {
            _facebookApi.Initialise(facebookAppId, packageSID);
        }

        public static void Login(List<string> permissions, Action<WSAFacebookLoginResult> response)
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
            {
                WSAFacebookLoginResult result = await _facebookApi.Login(permissions);

                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    if (response != null)
                    {
                        response(result);
                    }
                }, true);
            }, false);
        }

        public static void GetUserDetails(Action<WSAFacebookResponse<WSAFacebookUser>> response)
        {
            GetUserDetailsAsync(response);
        }

        private static async void GetUserDetailsAsync(Action<WSAFacebookResponse<WSAFacebookUser>> response)
        {
            WSAFacebookResponse<WSAFacebookUser> result = await _facebookApi.GetUserDetails();

            if (response != null)
            {
                response(result);
            }
        }

        public static void Logout(bool uninstall)
        {
            _facebookApi.Logout(uninstall);
        }

        public static void Share(string linkURL, string title, string description, string imageURL, Action<WSAFacebookShareResult> response)
        {
            UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
            {
                WSAFacebookShareResult result = await _facebookApi.Share(linkURL, title, description, imageURL);

                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    if (response != null)
                    {
                        response(result);
                    }
                }, true);
            }, false);

        }
    }
}
#endif

