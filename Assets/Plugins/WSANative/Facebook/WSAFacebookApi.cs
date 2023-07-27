#if UNITY_WSA && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using UnityEngine;
using Windows.Storage;
using Windows.Security.Authentication.Web;

namespace BagelCode.FacebookWSA
{

    public class WSAFacebookApi
    {
        public bool IsLoggedIn { get; private set; }

        private string _facebookAppId;
        private string _packageSID;
        private string _accessToken;

        private const string _authenticationErrorCode = "190";

        public WSAFacebookApi()
        {
        }

        public void Initialise(string facebookAppId, string packageSID)
        {
            _facebookAppId = facebookAppId;
            _packageSID = packageSID;
        }

        public async Task<WSAFacebookLoginResult> Login(List<string> permissions)
        {
            WSAFacebookLoginResult loginResult = new WSAFacebookLoginResult();

            try
            {
                Logout(false);

                string requestPermissions = "public_profile";

                if (permissions != null && permissions.Count > 0)
                {
                    requestPermissions = string.Join(",", permissions);
                }

                string accessToken = string.Empty;
                Uri appCallbackUri = new Uri("ms-app://" + _packageSID);

                Uri requestUri = new Uri(
                    string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&response_type=token&redirect_uri={1}&scope={2}", 
                                    _facebookAppId, appCallbackUri, requestPermissions));

                WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, requestUri, appCallbackUri);

                if (result.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    Match match = Regex.Match(result.ResponseData, "access_token=([^&]+)&");

                    accessToken = match.Groups[1].Value;
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    _accessToken = accessToken;
                    IsLoggedIn = true;
                }
            }
            catch (Exception e)
            {
                IsLoggedIn = false;
                loginResult.ErrorMessage = e.Message;
            }

            loginResult.Success = IsLoggedIn;
            loginResult.AccessToken = !string.IsNullOrWhiteSpace(_accessToken) ? _accessToken : null;

            return loginResult;
        }

        public async void Logout(bool uninstall)
        {
            IsLoggedIn = false;
            _accessToken = null;
        }

        public async Task<WSAFacebookResponse<WSAFacebookUser>> GetUserDetails()
        {
            WSAFacebookResponse<WSAFacebookUser> userDetailsResponse = new WSAFacebookResponse<WSAFacebookUser>();

            if (IsLoggedIn)
            {
                string fields = "id,age_range,birthday,email,first_name,gender,last_name,link,locale,name,picture,timezone";

                string requestUri = string.Format("{0}me?fields={1}&access_token={2}", WSAFacebookConstants.GraphApiUri, fields, _accessToken);

                try
                {
                    HttpClient client = new HttpClient();

                    HttpResponseMessage response = await client.Get(requestUri);

                    string responseAsString = response.Data;

                    if (response.IsSuccessStatusCode)
                    {
                        userDetailsResponse.Data = WSAFacebookUser.FromDto(JsonUtility.FromJson<WSAFacebookUserDto>(responseAsString));
                        userDetailsResponse.Success = true;
                    }
                    else
                    {
                        WSAFacebookError errorMessage = WSAFacebookError.FromDto(JsonUtility.FromJson<WSAFacebookErrorDto>(responseAsString));

                        if (errorMessage.Code == _authenticationErrorCode)
                        {
                            Logout(false);
                            errorMessage.AccessTokenExpired = true;
                        }

                        userDetailsResponse.Success = false;
                        userDetailsResponse.Error = errorMessage;
                    }
                }
                catch
                {
                    userDetailsResponse.Success = false;
                }
            }
            else
            {
                userDetailsResponse.Success = false;
                userDetailsResponse.Error = new WSAFacebookError()
                {
                    AccessTokenExpired = true
                };
            }

            return userDetailsResponse;
        }

        public async Task<WSAFacebookShareResult> Share(string linkURL, string title, string description, string imageURL)
        {
            WSAFacebookShareResult shareResult = new WSAFacebookShareResult();

            try
            {
                string contentsURL = string.Format("{0}?image={1}&title={2}&description={3}", linkURL, imageURL, title, description);
                UriBuilder baseUri = new UriBuilder(linkURL);
                Uri appCallbackUri = new Uri("https://www.facebook.com/connect/login_success.html");

                string requestURI = string.Format("https://www.facebook.com/dialog/share?app_id={0}&display=popup&href={1}&redirect_uri={2}", _facebookAppId, contentsURL, appCallbackUri);
                Uri requestUri = new Uri(requestURI);
                WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, requestUri, appCallbackUri);

                shareResult.Success = result.ResponseStatus == WebAuthenticationStatus.Success;
            }
            catch (Exception e)
            {
                shareResult.Success = false;
                shareResult.ErrorMessage = e.Message;
            }

            return shareResult;
        }
    }

}

#endif
