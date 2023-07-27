#if UNITY_WSA && !UNITY_EDITOR

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Windows.System.Profile;
using Windows.Storage.Streams;
using com.adjust.sdk;
using SlotMaker.Json;

namespace BagelCode
{
    public class NativeHelperWindows : INativeHelper
    {
        [DllImport ("__Internal")]
        private static extern void initialize([MarshalAs(UnmanagedType.LPWStr)]string name);

        private delegate void UnitySendMessageDelegate([MarshalAs(UnmanagedType.LPWStr)]string gameObjectName, [MarshalAs(UnmanagedType.LPWStr)]string methodName, [MarshalAs(UnmanagedType.LPWStr)]string message);

        [DllImport ("__Internal")]
        private static extern void setUnitySendMessage(UnitySendMessageDelegate fn);

        public void Initialize(string name)
        {
            initialize(name);
            setUnitySendMessage(UnitySendMessageWrapper);
        }

        [AOT.MonoPInvokeCallback (typeof (UnitySendMessageDelegate))]
        protected static void UnitySendMessageWrapper([MarshalAs(UnmanagedType.LPWStr)]string gameObjectName, [MarshalAs(UnmanagedType.LPWStr)]string methodName, [MarshalAs(UnmanagedType.LPWStr)]string message)
        {
            var gameObject = GameObject.Find(gameObjectName);
            if (gameObject != null)
            {
                gameObject.SendMessage(methodName, message);
            }
        }

        public bool IsNetworkAvailable()
        {
    // TODO
            return true;
        }

        [DllImport("__Internal")]
        private static extern void registerForNotification();
        public void RegisterForNotification()
        {
            registerForNotification();
        }

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getNotificationToken();
        public string GetNotificationToken()
        {
            return getNotificationToken();
        }

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getDeviceId();
        public string GetDeviceID()
        {
            var systemId = SystemIdentification.GetSystemIdForPublisher();

            if (systemId.Source != SystemIdentificationSource.None)
            {
                using (DataReader reader = DataReader.FromBuffer(systemId.Id))
                {
                    byte[] bytes = new byte[systemId.Id.Length];
                    reader.ReadBytes(bytes);

                    return System.Convert.ToBase64String(bytes);
                }
                // var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(systemId.Id);
                // string deviceID = dataReader.ReadString(systemId.Id.Length);
                // Debug.LogError(deviceID);
                // return deviceID;
            }

            return getDeviceId();
        }

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getServerBaseUrl();
        public string GetServerBaseUrl()
        {
            return getServerBaseUrl();
        }

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getChattingUrl();
        public string GetChattingUrl()
        {
            return getChattingUrl();
        }

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAppDownloadUrl();
        public string GetAppDownloadUrl()
        {
            return getAppDownloadUrl();
        }

        private System.Action<byte[]> nativeAction;
        [DllImport("__Internal")]
        private static extern void getProfileImage(uint minSide, [MarshalAs(UnmanagedType.LPWStr)]string callback);
        public void GetProfileImage(string unityCallBack, int width, int height, bool isCropable, System.Action<byte[]> action)
        {
            nativeAction = action;
            getProfileImage((uint)width, unityCallBack);
        }

        public void OnProfileImageLoaded(string encodedBinary)
        {
            Debug.Log("OnProfileImageLoaded");

            byte[] bytes = System.Convert.FromBase64String(encodedBinary);

            if (nativeAction != null)
            {
                nativeAction(bytes);
                nativeAction = null;
            }
        }

        [DllImport("__Internal")]
        private static extern void setLocalPush(int pushID, [MarshalAs(UnmanagedType.LPWStr)]string sender, [MarshalAs(UnmanagedType.LPWStr)]string message, int delay, bool doNotDisturb);
        public void SetLocalPush(int pushID, string sender, string message, int delay, string type, bool doNotDisturb)
        {
            setLocalPush(pushID, sender, message, delay, doNotDisturb);
        }

        [DllImport("__Internal")]
        private static extern void deleteLocalPush(int pushID);
        public void DeleteLocalPush(int pushID)
        {
            deleteLocalPush(pushID);
        }

        [DllImport("__Internal")]
        private static extern void togglePush();
        public void TogglePush()
        {
            togglePush();
        }

        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAdjustEnv();
        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAdjustAppToken();
        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAdjustAppSecret();
        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAdjustInfo1();
        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAdjustInfo2();
        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAdjustInfo3();
        [DllImport("__Internal")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        private static extern string getAdjustInfo4();
        public void InitAdjust(string objectName, string methodName)
        {
            // Using Adjust Unity SDK for Windows
            string adjustEnv = getAdjustEnv();
            string appToken = getAdjustAppToken();
            long appSecret = Convert.ToInt64(getAdjustAppSecret());
            long info1 = Convert.ToInt64(getAdjustInfo1());
            long info2 = Convert.ToInt64(getAdjustInfo2());
            long info3 = Convert.ToInt64(getAdjustInfo3());
            long info4 = Convert.ToInt64(getAdjustInfo4());

            AdjustEnvironment environment = AdjustEnvironment.Sandbox;
            if (adjustEnv == "production") {
                environment = AdjustEnvironment.Production;
            }

            AdjustConfig config = new AdjustConfig(appToken, environment, true);
            if (adjustEnv == "production") {
                config.setLogLevel(AdjustLogLevel.Suppress);
            } else {
                config.setLogLevel(AdjustLogLevel.Verbose);
            }

            config.setAppSecret(appSecret, info1, info2, info3, info4);
            config.setAttributionChangedDelegate(this.adjustAttributionChangedDelegate);
            Adjust.start(config);
        }

        public void adjustAttributionChangedDelegate(AdjustAttribution attribution) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add(AdjustUtils.KeyAdid, attribution.adid);
            dict.Add(AdjustUtils.KeyTrackerName, attribution.trackerName);
            dict.Add(AdjustUtils.KeyTrackerToken, attribution.trackerToken);
            dict.Add(AdjustUtils.KeyNetwork, attribution.network);
            dict.Add(AdjustUtils.KeyCampaign, attribution.campaign);
            dict.Add(AdjustUtils.KeyAdgroup, attribution.adgroup);
            dict.Add(AdjustUtils.KeyCreative, attribution.creative);
            dict.Add(AdjustUtils.KeyClickLabel, attribution.clickLabel);

            // Should be called on Unity Main Thread
            MainThreadDispatcher.Instance.Enqueue(adjustOnAttributionCallback(SlotSimpleJson.SerializeObject(dict)));
        }

        private IEnumerator adjustOnAttributionCallback(string json)
        {
            AdjustManager.Instance.OnAttributionCallback(json);
            yield return null;
        }

        public string GetAdjustID()
        {
            // Using Adjust Unity SDK for Windows
            return Adjust.getAdid();
        }

        public void SendAdjustEvent(string eventID, string eventToken)
        {
            // Using Adjust Unity SDK for Windows
            AdjustEvent adjustEvent = new AdjustEvent(eventToken);
            Adjust.trackEvent(adjustEvent);
        }

        public void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID)
        {
            // Using Adjust Unity SDK for Windows
            AdjustEvent adjustEvent = new AdjustEvent(eventToken);
            adjustEvent.setRevenue(revenue, "USD");
            // DO NOT USE (This function occur JSON parsing error at next launch.)
            // adjustEvent.setTransactionId(purchaseID);
            Adjust.trackEvent(adjustEvent);
        }

        public void SendFIREvent(string eventID)
        {
            // Nothing to do
        }

        public void CopyClipboard(string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            BICustomEvents.FAS(pauseStatus);
            // TODO: clear badge?
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

        public void OnApplicationExit()
        {
            Application.Quit();
        }

        public void OnLocalPushMessage(string message)
        {
            // todo
        }

        public void OnPendingMessage(string data)
        {
            // todo
        }

        public void OnOpenUrl(string uri)
        {
            // todo
        }

        public void OpenUrl(string url)
        {
            Application.OpenURL(url);
        }

        public long GetFreeDiskSpace()
        {
            return 1000l;
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

        //

        public void SetSupportPageUrl(string url)
        {
            // Nothing to do
        }

        public void OpenAppSettings()
        {
            // Nothing to do
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

        [DllImport("__Internal")]
        private static extern bool isPinningAllowed();
        public bool IsPinningAllowed()
        {
            return isPinningAllowed();
        }

        [DllImport("__Internal")]
        private static extern void isPinned([MarshalAs(UnmanagedType.LPWStr)]string unityCallback);
        public void IsPinned(string unityCallback)
        {
            isPinned(unityCallback);
        }

        [DllImport("__Internal")]
        private static extern void setPin([MarshalAs(UnmanagedType.LPWStr)]string unityCallback);
        public void SetPin(string unityCallback)
        {
            setPin(unityCallback);
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
