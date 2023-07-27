#if UNITY_STANDALONE_OSX && !UNITY_EDITOR

using UnityEngine;

namespace BagelCode
{
    public class NativeHelperOSXStandalone : INativeHelper
    {
        private static string oneTimeDeviceId = string.Empty;
        public void Initialize(string name)
        {

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
            return "DUMMY_UNITY_TOKEN";
        }

        public string GetDeviceID()
        {
            if (oneTimeDeviceId == string.Empty)
            {
                oneTimeDeviceId = System.Guid.NewGuid().ToString();
            }
            return oneTimeDeviceId;
        }

        public string GetServerBaseUrl()
        {
            return BuildStrings.serverBaseUrl;
        }

        public string GetChattingUrl()
        {
            return BuildStrings.chattingUrl;
        }

        public string GetAppDownloadUrl()
        {
            return BuildStrings.appDownloadUrl;
        }

        public void GetProfileImage(string unityCallBack, int width, int height, bool isCropable, System.Action<byte[]> action)
        {
            // Nothing to do
        }

        public void OnProfileImageLoaded(string encodedBinary)
        {
            // Nothing to do
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

        public string GetAdjustID()
        {
            return "DUMMY_UNITY_ADJUST_ID";
        }

        public void SendAdjustEvent(string eventID, string eventToken)
        {
            // Nothing to do
        }

        public void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID)
        {
            // Nothing to do
        }

        public void SendFIREvent(string eventID)
        {
            // Nothing to do
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            BICustomEvents.FAS(pauseStatus);
            // Nothing to do
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
            GUIUtility.systemCopyBuffer = str;
        }

        public void OnApplicationExit()
        {
            // Nothing to do
        }

        public void OnLocalPushMessage(string message)
        {
            // nothing todo
        }

        public void OnPendingMessage(string data)
        {
            // nothing todo
        }

        public void OnOpenUrl(string uri)
        {
                // nothing todo
        }
        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        public long GetFreeDiskSpace()
        {
            return 1000L;
        }

        public string GetContextId()
        {
            // no need to implement (just for Canvas)
            return "";
        }

        public void SetIdleTimerDisabled(bool value)
        {
            // Nothing to do
        }

        public void DoSurvey(string hash, string userId)
        {
            // Nothing to do
        }

        public bool GetPushNotificationSubscribed()
        {
            // Nothing to do
            return false;
        }

        public void SetSupportPageUrl(string url)
        {
            // Nothing to do
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
