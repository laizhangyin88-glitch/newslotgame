#if UNITY_IPHONE && !UNITY_EDITOR

using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;

namespace BagelCode
{
    public class NativeHelperIOS : INativeHelper
    {
        //[DllImport("__Internal")]
        //private static extern bool _isNetworkAvailable();

        public bool IsNetworkAvailable()
        {
            //return _isNetworkAvailable();
            return true;
        }

        public void RegisterForNotification()
        {
            //NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound, true);
        }

        public string GetNotificationToken()
        {
            //string hexToken = null;
            //byte[] token = NotificationServices.deviceToken;
            //if (token != null) {
            //    // send token to a provider
            //    hexToken = System.BitConverter.ToString(token).Replace("-", "");
            //}
            //return hexToken;
            return "DUMMY_UNITY_TOKEN";
        }

        //[DllImport ("__Internal")]
        //public static extern string _getDeviceID();

        public string GetDeviceID()
        {
            //return _getDeviceID();
            return SystemInfo.deviceUniqueIdentifier;
        }

        //[DllImport("__Internal")]
        //private static extern string _getServerBaseUrl();
        public string GetServerBaseUrl()
        {
            //return _getServerBaseUrl();
            return null;
        }

        //[DllImport("__Internal")]
        //private static extern string _getChattingUrl();
        public string GetChattingUrl()
        {
            //return _getChattingUrl();
            return null;
        }

        //[DllImport("__Internal")]
        //private static extern string _getAppDownloadUrl();
        public string GetAppDownloadUrl()
        {
            //return _getAppDownloadUrl();
            return null;
        }

        private System.Action<byte[]> nativeAction;

        //[DllImport ("__Internal")]
        //public static extern void _pickThumbnailFromAlbum(int width, int height, bool isCropable, string unityOkayCallback, string unityCancelCallback);

        public void GetProfileImage(string unityCallBack, int width, int height, bool isCropable, System.Action<byte[]> action)
        {
            //nativeAction = action;
            //_pickThumbnailFromAlbum(width, height, isCropable, unityCallBack, unityCallBack);
        }

        public void OnProfileImageLoaded(string encodedBinary)
        {
            //Debug.Log("OnProfileImageLoaded");

            //byte[] bytes = System.Convert.FromBase64String(encodedBinary);

            //if (nativeAction != null)
            //{
            //    nativeAction(bytes);
            //}
        }

        //[DllImport ("__Internal")]
        //public static extern void _setLocalPush(string pushID, string sender, string text, string type, int seconds, long getPushTimestamp);

        public void SetLocalPush(int pushID, string sender, string message, int delay, string type, bool doNotDisturb)
        {
            //long getPushTimestamp = TimeUtils.GetCurrentTime() + delay;

            //var doNotDisturbSetting = SlotMaker.BlackboardUtils.FindVariable<NodeCanvas.Framework.Blackboard>(SlotMaker.MainBlackboard.Get(), "values/misc/LOCAL_PUSH/DO_NOT_DISTURB_SETTING");

            //if (doNotDisturb) {
            //    System.DateTime dateTime = TimeUtils.ParseTimestampToLocalDateTime(getPushTimestamp * 1000);
            //    int hour = dateTime.Hour;
            //    int minute = dateTime.Minute;
            //    int second = dateTime.Second;

            //    int alternativeHour = doNotDisturbSetting.value.GetValue<int>("ALTERNATIVE_HOUR");

            //    int additionalDelaySec = 0;
            //    if (doNotDisturbSetting.value.GetValue<List<int>>("HOUR_LIST").Contains(hour)) {
            //        int alternativeSeconds = alternativeHour * 60 * 60;
            //        int originalSeconds = hour * 3600 + minute * 60 + second;
            //        if (hour >= alternativeHour) {
            //            originalSeconds = originalSeconds - (24 * 3600);
            //        }
            //        additionalDelaySec = alternativeSeconds - originalSeconds;
            //        delay += additionalDelaySec;
            //        getPushTimestamp += additionalDelaySec;
            //    }
            //}
            //if (!string.IsNullOrEmpty(sender)) {
            //    message = sender + "\n" + message;
            //}
            //_setLocalPush(pushID.ToString(), sender, message, type, delay, getPushTimestamp * 1000);
        }

        //[DllImport ("__Internal")]
        //public static extern void _deleteLocalPush(string pushID);

        public void DeleteLocalPush(int pushID)
        {
            //_deleteLocalPush(pushID.ToString());
        }

        public void TogglePush()
        {
            // Nothing to do
        }

        //[DllImport ("__Internal")]
        //public static extern void _initAdjust(string objectName, string methodName);
        public void InitAdjust(string objectName, string methodName)
        {
            //_initAdjust(objectName, methodName);
        }

        //[DllImport ("__Internal")]
        //public static extern string _getAdjustID();
        public string GetAdjustID()
        {
            //return _getAdjustID();
            return "DUMMY_UNITY_ADJUST_ID";
        }

        //[DllImport ("__Internal")]
        //public static extern void _sendAdjustEvent(string eventToken);

        public void SendAdjustEvent(string eventID, string eventToken)
        {
            //_sendAdjustEvent(eventToken);
        }

        //[DllImport ("__Internal")]
        //public static extern void _sendAdjustRevenueEvent(string eventToken, double revenue, string orderID);

        public void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID)
        {
            //_sendAdjustRevenueEvent(eventToken, revenue, purchaseID);
        }

        //[DllImport ("__Internal")]
        //public static extern void _sendFIREvent(string eventID);

        public void SendFIREvent(string eventID)
        {
            //_sendFIREvent(eventID);
        }

        //[DllImport ("__Internal")]
        //public static extern void _copyClipboard(string str);

        public void CopyClipboard(string str)
        {
            //_copyClipboard(str);

        }

        //[DllImport ("__Internal")]
        //public static extern void _clearBadge();

        public void OnApplicationPause(bool pauseStatus)
        {
            BICustomEvents.FAS(pauseStatus);

            //if (!pauseStatus)
            //{
            //    _clearBadge();
            //}
        }

        public void OnApplicationFocus(bool focusStatus)
        {
            //BICustomEvents.FAS(!focusStatus);
        }


        //[DllImport ("__Internal")]
        //private static extern void _logToiOS(string debugMessage);

        private static void LogToiOS(string logString, string stackTrace, LogType type)
        {
            //_logToiOS(logString);
        }

        //[DllImport ("__Internal")]
        //private static extern void _initialize(string name);

        public void Initialize(string name)
        {
            //_initialize(name);
            //_clearBadge();
        }

        public void OnEnable()
        {
    //#if DEV
    //        Application.logMessageReceived += LogToiOS;
    //#endif
        }

        public void OnDisable()
        {
    //#if DEV
    //        Application.logMessageReceived -= LogToiOS;
    //#endif
        }

        public void OnApplicationExit()
        {
            // Nothing to do
        }

        public void OnLocalPushMessage(string message)
        {
            //if (string.IsNullOrEmpty(message)) return;

            //PendingActionManager.Instance.OnLocalPushMessage(message);
        }

        public void OnPendingMessage(string data)
        {
            //if (string.IsNullOrEmpty(data)) return;

            //PendingActionManager.Instance.PushPendingAction(data);
        }

        public void OnOpenUrl(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return;

            PendingActionManager.Instance.OpenUrl(uri);
        }

        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        //[DllImport ("__Internal")]
        //private static extern long _getFreeDiskSpace();

        public long GetFreeDiskSpace()
        {
            //return _getFreeDiskSpace();
            return 1000L;
        }

        public string GetContextId()
        {
            // no need to implement (just for Canvas)
            return "";
        }

        //[DllImport ("__Internal")]
        //private static extern void _setIdleTimerDisabled(bool value);

        public void SetIdleTimerDisabled(bool value)
        {
            //_setIdleTimerDisabled(value);
        }

        //[DllImport ("__Internal")]
        //private static extern void _openSurveyMonkey(string hash, string userId);
        public void DoSurvey(string hash, string userId)
        {
            //_openSurveyMonkey(hash, userId);
        }

        //[DllImport ("__Internal")]
        //private static extern bool _getPushNotificationSubscribed();
        public bool GetPushNotificationSubscribed()
        {
            //return _getPushNotificationSubscribed();
            return false;
        }

        public void SetSupportPageUrl(string url)
        {
            // Nothing to do
        }

        //[DllImport ("__Internal")]
        //private static extern void _openPresentAppSettings();
        public void OpenAppSettings()
        {
            //_openPresentAppSettings();
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
            

        //[DllImport("__Internal")]
        //private static extern void UnitySignInWithApple_Login(System.IntPtr callback);

        private delegate void AppleLoginCompleted(int result, Sso.AppleLogin.UserInfo info);

        private static System.Action<Sso.AppleLogin.AppleLoginCallbackArgs> _loginAppleCallback;
        public void LoginWithApple(System.Action<Sso.AppleLogin.AppleLoginCallbackArgs> callback)
        {
            //_loginAppleCallback = callback;

            //System.IntPtr cback = System.IntPtr.Zero;
            //AppleLoginCompleted d = NativeLoginCompletedCallback;
            //cback = Marshal.GetFunctionPointerForDelegate(d);

            //UnitySignInWithApple_Login(cback);
        }

        //[DllImport ("__Internal")]
        //private static extern void _setExternalUserId(string userId);
        public void SetExternalUserId(string userId)
        {
            //_setExternalUserId(userId);
        }

        //[DllImport ("__Internal")]
        //private static extern void _removeExternalUserId();
        public void RemoveExternalUserId()
        {
            //_removeExternalUserId();
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

        //[DllImport ("__Internal")]
        //private static extern void _testCrash();
        public void TestCrashlytics(string msg)
        {
            //_testCrash();
        }

        //[DllImport ("__Internal")]
        //private static extern void _shareSNSUrl(string url);
        public void ShareSNSUrl(string url)
        {
            //_shareSNSUrl(url);
        }

        public void GetReferrerUrl(string unityCallBack)
        {
            // Nothing to do
        }

        public void SetPIP(bool enable)
        {
            // TODO : PIP
        }

        public void MoveHomeScreen()
        {
            // Nothing to do
        }

        //[AOT.MonoPInvokeCallback(typeof(AppleLoginCompleted))]
        //private static void NativeLoginCompletedCallback(int result, Sso.AppleLogin.UserInfo info)
        //{
        //    var args = new Sso.AppleLogin.AppleLoginCallbackArgs();
        //    if (result != 0)
        //    {
        //        args.userInfo = new Sso.AppleLogin.UserInfo
        //        {
        //            idToken = info.idToken,
        //            authorisationCode = info.authorisationCode,
        //            firstName = info.firstName,
        //            surname = info.surname,
        //            email = info.email,
        //            userId = info.userId,
        //            userDetectionStatus = info.userDetectionStatus
        //        };
        //    }
        //    else
        //    {
        //        args.error = info.error;
        //    }

        //    _loginAppleCallback?.Invoke(args);
        //    _loginAppleCallback = null;
        //}

        //[DllImport("__Internal")]
        //private static extern void UnitySignInWithApple_GetCredentialState(string userID, System.IntPtr callback);
    }
}

#endif
