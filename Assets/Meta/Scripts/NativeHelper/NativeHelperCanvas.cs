#if UNITY_WEBGL && !UNITY_EDITOR

using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace BagelCode
{
    public class NativeHelperCanvas : INativeHelper
    {
        [DllImport("__Internal")]
        private static extern string GetPageContextId();

        [DllImport("__Internal")]
        private static extern void GetImageFromBrowser(int imageWidth, int imageHeight, string objectName, string callBackFuncName);

        [DllImport("__Internal")]
        private static extern void JSCopyClipboard(string copyText);

        [DllImport("__Internal")]
        private static extern void OpenWindow(string url);

        [DllImport("__Internal")]
        private static extern string GetServerBaseUrlConfig();

        [DllImport("__Internal")]
        private static extern string GetChattingUrlConfig();

        [DllImport("__Internal")]
        private static extern string GetAppDonwloadUrlConfig();

        [DllImport("__Internal")]
        private static extern void SetSupportPage(string url);

        private string nativeHelperUnityObject;
        private string contextId = "";

        public void Initialize(string name)
        {
            nativeHelperUnityObject = name;

            contextId = GetPageContextId();

            string url = Application.absoluteURL;
            int iqs = url.IndexOf('?');
            string queryString = "";
            var queryParams = new System.Collections.Specialized.NameValueCollection();
            if (iqs >= 0)
            {
                queryString = (iqs < url.Length - 1) ? url.Substring(iqs + 1) : "";
                foreach (string paramString in queryString.Split('&'))
                {
                    string[] singlePair = paramString.Split('=');
                    if (singlePair.Length == 2)
                    {
                        string decodedData = System.Uri.UnescapeDataString(singlePair[1]);
                        queryParams.Add(singlePair[0], decodedData);
                    }
                    else if (singlePair.Length == 1 && singlePair[0].Length > 0)
                    {
                        // only one key with no value specified in query string
                        queryParams.Add(singlePair[0], "");
                    }
                }
            }
            foreach(string queryParamKey in queryParams.AllKeys)
            {
                if (queryParamKey == "deeplink_action")
                {
                    UnitySendMessageWrapper(nativeHelperUnityObject, "OnOpenUrl", "clubvegas://adjust?action=" + queryParams[queryParamKey]);
                }
                else if (queryParamKey == "push_action")
                {
                    UnitySendMessageWrapper(nativeHelperUnityObject, "OnPendingMessage", queryParams[queryParamKey]);
                }
            }
        }

        private static void UnitySendMessageWrapper(string gameObjectName, string methodName, string message)
        {
            var gameObject = GameObject.Find(gameObjectName);
            if (gameObject != null && methodName != null)
            {
                gameObject.SendMessage(methodName, message);
            }
        }

        public bool IsNetworkAvailable()
        {
            return true;
        }

        public void RegisterForNotification()
        {
            // Nothing to do
        }

        public string GetNotificationToken()
        {
            // facebook does not need client-side push token
            return null;
        }

        public string GetDeviceID()
        {
            //string facebookId = SocialManager.Instance.GetFacebookId();
            
           // System.Guid guid = Logos.Utility.GuidUtility.Create(System.Guid.Empty, facebookId);
           // return guid.ToString();

           //for debug test
#if UNITY_EDITOR && DEV
        if (!string.IsNullOrEmpty(SlotMaker.TestSuite.TestSuiteManager.Instance.id))
            return SystemInfo.deviceUniqueIdentifier + SlotMaker.TestSuite.TestSuiteManager.Instance.id;
        else
#endif
            return SystemInfo.deviceUniqueIdentifier;
        }

        private static string serverBaseUrl = string.Empty;
        public string GetServerBaseUrl()
        {
            if (serverBaseUrl == string.Empty) {
                serverBaseUrl = GetServerBaseUrlConfig();
            }
            return serverBaseUrl;
        }

        private static string chattingUrl = string.Empty;
        public string GetChattingUrl()
        {
            if (chattingUrl == string.Empty) {
                chattingUrl = GetChattingUrlConfig();
            }
            return chattingUrl;
        }

        private static string appDownloadUrl = string.Empty;
        public string GetAppDownloadUrl()
        {
            if (appDownloadUrl == string.Empty) {
                appDownloadUrl = GetAppDonwloadUrlConfig();
            }
            return appDownloadUrl;
        }

        private System.Action<byte[]> nativeAction;
        public void GetProfileImage(string unityCallBack, int width, int height, bool isCropable, System.Action<byte[]> action)
        {
            nativeAction = action;
            GetImageFromBrowser(width, height, nativeHelperUnityObject, unityCallBack);
        }

        private string s_dataUrlPrefix = "data:image/png;base64,";
        public void OnProfileImageLoaded(string dataUrl)
        {
            if (dataUrl.StartsWith(s_dataUrlPrefix))
            {
                byte[] pngData = System.Convert.FromBase64String(dataUrl.Substring(s_dataUrlPrefix.Length));
                if (nativeAction != null)
                {
                    nativeAction(pngData);
                    nativeAction = null;
                }
            }
            else
            {
                Debug.LogError("Error getting image:" + dataUrl);
                if (nativeAction != null)
                {
                    nativeAction(new byte[0]);
                    nativeAction = null;
                }
            }
        }

        public void SetLocalPush(int pushID, string sender, string message, int delay, string type, bool doNotDisturb)
        {
            // Nothing to do
        }

        public void DeleteLocalPush(int pushID)
        {
            // Nothing to do
        }

        public void TogglePush()
        {
            // Nothing to do
        }

        public void InitAdjust(string objectName, string methodName)
        {
            // Nothing to do
        }

        public void SendFIREvent(string eventID)
        {
            // Nothing to do
        }

        public string GetAdjustID()
        {
            return SocialManager.Instance.GetFacebookId();
        }

        [DllImport("__Internal")]
        private static extern void LogAppEventFacebook(string eventId);
        public void SendAdjustEvent(string eventID, string eventToken)
        {
            LogAppEventFacebook(eventID);
        }

        [DllImport("__Internal")]
        private static extern void LogPurchaseFacebook(string purchaseId, float revenue);
        public void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID)
        {
            LogPurchaseFacebook(purchaseID, (float)revenue);
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            BICustomEvents.FAS(pauseStatus);
            // what to do?
        }

        public void OnApplicationFocus(bool focusStatus)
        {
            // Nothing to do
        }

        public void OnEnable()
        {
            // Nothing to do
        }

        public void OnDisable()
        {
            // Nothing to do
        }

        public void CopyClipboard(string str)
        {
            JSCopyClipboard(str);
        }

        public void OnApplicationExit()
        {
            Application.Quit();
        }

        public void OnLocalPushMessage(string message)
        {
            // nothing todo
        }

        public void OnPendingMessage(string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            PendingActionManager.Instance.PushPendingAction(data);
        }

        public void OnOpenUrl(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return;

            PendingActionManager.Instance.OpenUrl(uri);
        }

        public void OpenUrl(string url)
        {
            OpenWindow(url);
        }

        public long GetFreeDiskSpace()
        {
            return 1000L;
        }

        public string GetContextId()
        {
            return contextId;
        }

        public void SetIdleTimerDisabled(bool value)
        {
            // Nothing to do
        }

        public void DoSurvey(string hash, string userId)
        {
            OpenUrl("https://www.surveymonkey.com/r/" + hash + "?userId=" + userId);
        }

        public bool GetPushNotificationSubscribed()
        {
            // Nothing to do
            return false;
        }

        public void SetSupportPageUrl(string url)
        {
            SetSupportPage(url);
        }

        public void OpenAppSettings()
        {
            // Nothing to do
        }

        public void OpenAppSettingsPipMode()
        {
            // Nothing to do
        }
        
        public bool GetAppSettingsPipModeEnabled()
        {
            // Nothing to do
            return false;
        }
        
        public bool GetAppSettingsPipModeAvailable()
        {
            // Nothing to do
            return false;
        }

        public void LoginWithApple(System.Action<Sso.AppleLogin.AppleLoginCallbackArgs> callback)
        {
            // Nothing to do
        }

        public void SetExternalUserId(string userId)
        {
            // Nothing to do
        }

        public void RemoveExternalUserId()
        {
            // Nothing to do
        }

        public bool IsPinningAllowed()
        {
            return false;
        }

        public void IsPinned(string unityCallback)
        {
            // Nothing to do
        }

        public void SetPin(string unityCallback)
        {
            // Nothing to do
        }

        public void TestCrashlytics(string msg)
        {
            // Nothing to do
        }

        public void ShareSNSUrl(string url)
        {
            // Nothing to do
        }

        public void GetReferrerUrl(string unityCallBack)
        {
            // Nothing to do
        }

        public void SetPIP(bool enable)
        {
            // Nothing to do
        }

        public void MoveHomeScreen()
        {
            // Nothing to do
        }
    }
}

#endif
