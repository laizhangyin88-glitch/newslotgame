#if UNITY_ANDROID && !UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;

namespace BagelCode
{
    public class NativeHelperAndroid : INativeHelper
    {
        private AndroidJavaClass ajc = new AndroidJavaClass("com.cryfx.slots.unityandroid.MainActivity");//new AndroidJavaClass("com.bagelcode.v3.UnityPlayerActivity");

        public void Initialize(string name)
        {
            Debug.Log("before call NativeHelperAndroid Initialize" + name);
            ajc.CallStatic("Initialize", name);
            Debug.Log("after call NativeHelperAndroid Initialize" + name);
        }

        public bool IsNetworkAvailable()
        {
            return ajc.CallStatic<bool>("isNetworkAvailable");
        }

        public void RegisterForNotification()
        {
            // Nothing to do
        }

        public string GetNotificationToken()
        {
            return "DUMMY_UNITY_TOKEN";//return ajc.CallStatic<string>("getNotificationToken");
        }

        public string GetDeviceID()
        {
            return ajc.CallStatic<string>("getDeviceID");
        }

        public string GetServerBaseUrl()
        {
            return null;//ajc.CallStatic<string>("getServerBaseUrl");
        }

        public string GetChattingUrl()
        {
            return null;//ajc.CallStatic<string>("getChattingUrl");
        }

        public string GetAppDownloadUrl()
        {
            return null;//return ajc.CallStatic<string>("getAppDownloadUrl");
        }

        private System.Action<byte[]> nativeAction;
        public void GetProfileImage(string unityCallBack, int width, int height, bool isCropable, System.Action<byte[]> action)
        {
            nativeAction = action;
            ajc.CallStatic("getProfileImage", width, height, isCropable, unityCallBack);
        }

        public void OnProfileImageLoaded(string encodedBinary)
        {
            byte[] bytes = System.Convert.FromBase64String(encodedBinary);

            if (nativeAction != null)
            {
                nativeAction(bytes);
                nativeAction = null;
            }
        }

        public void SetLocalPush(int pushID, string sender, string message, int delay, string type, bool doNotDisturb)
        {
        /*
            long getPushTimestamp = TimeUtils.GetCurrentTime() + delay;

            var doNotDisturbSetting = SlotMaker.BlackboardUtils.FindVariable<NodeCanvas.Framework.Blackboard>(SlotMaker.MainBlackboard.Get(), "values/misc/LOCAL_PUSH/DO_NOT_DISTURB_SETTING");

            if (doNotDisturb) {
                System.DateTime dateTime = TimeUtils.ParseTimestampToLocalDateTime(getPushTimestamp * 1000);
                int hour = dateTime.Hour;
                int minute = dateTime.Minute;
                int second = dateTime.Second;

                int alternativeHour = doNotDisturbSetting.value.GetValue<int>("ALTERNATIVE_HOUR");

                int additionalDelaySec = 0;
                if (doNotDisturbSetting.value.GetValue<List<int>>("HOUR_LIST").Contains(hour)) {
                    int alternativeSeconds = alternativeHour * 60 * 60;
                    int originalSeconds = hour * 3600 + minute * 60 + second;
                    if (hour >= alternativeHour) {
                        originalSeconds = originalSeconds - (24 * 3600);
                    }
                    additionalDelaySec = alternativeSeconds - originalSeconds;
                    delay += additionalDelaySec;
                    getPushTimestamp += additionalDelaySec;
                }
            }

            ajc.CallStatic("setLocalPush", pushID, sender, message, type, delay, getPushTimestamp * 1000);
            */
        }

        public void DeleteLocalPush(int pushID)
        {
            //ajc.CallStatic("cancelLocalPushNotification", pushID);
        }

        public void TogglePush()
        {
            //bool isAcceptingPush = ajc.CallStatic<bool>("isAcceptingPush");
            //ajc.CallStatic("setAcceptingPush", !isAcceptingPush);
        }

        public void InitAdjust(string objectName, string methodName)
        {
           // ajc.CallStatic("initAdjust", objectName, methodName);
        }

        public string GetAdjustID()
        {
            return "DUMMY_UNITY_ADJUST_ID";//return ajc.CallStatic<string>("getAdjustID");
        }

        public void SendAdjustEvent(string eventID, string eventToken)
        {
            //ajc.CallStatic("sendAdjustEvent", eventToken);
        }

        public void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID)
        {
           // ajc.CallStatic("sendAdjustRevenueEvent", eventToken, revenue, purchaseID);
        }

        public void SendFIREvent(string eventID)
        {
           // ajc.CallStatic("sendFIREvent", eventID);
        }

        public void CopyClipboard(string str)
        {
            ajc.CallStatic("copyClipboard", str);
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            BICustomEvents.FAS(pauseStatus);

            if (!pauseStatus)
            {
                //PurchaseManager.Instance.ResumeUsingService();
            }
            else
            {
                //PurchaseManager.Instance.StopUsingService();
            }
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
            if (string.IsNullOrEmpty(message)) return;

            PendingActionManager.Instance.OnLocalPushMessage(message);
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
            Application.OpenURL(url);
        }

        public long GetFreeDiskSpace()
        {
#if PLATFORM_AMAZON
            return 1000;
#else
            return ajc.CallStatic<long>("getFreeDiskSpace");
#endif
        }

        public string GetContextId()
        {
            // no need to implement (just for Canvas)
            return "";
        }

        public void SetIdleTimerDisabled(bool value)
        {
            ajc.CallStatic("setIdleTimerDisabled", value);
        }

        public void DoSurvey(string hash, string userId)
        {
            ajc.CallStatic("OpenSurveyMonkey", hash, userId);
        }
        
        public bool GetPushNotificationSubscribed()
        {
           return false;// return ajc.CallStatic<bool>("areNotificationsEnabled");
        }

        public void SetSupportPageUrl(string url)
        {
            // Nothing to do
        }

        public void OpenAppSettings()
        {
            ajc.CallStatic("OpenAppNotificationSettings");
        }
        
        public bool GetAppSettingsPipModeEnabled()
        {
           return false;// return ajc.CallStatic<bool>("arePipModeEnabled");
        }
        
        public bool GetAppSettingsPipModeAvailable()
        {
            return true;//return ajc.CallStatic<bool>("arePipModeAvailable");
        }

        public void OpenAppSettingsPipMode()
        {
            ajc.CallStatic("OpenAppSettingsPipMode");
        }

        public void LoginWithApple(System.Action<Sso.AppleLogin.AppleLoginCallbackArgs> callback)
        {
            // Nothing to do
        }

        public void SetExternalUserId(string userId)
        {
            Debug.Log("unityHelper Android");
            ajc.CallStatic("setExternalUserId", userId);
        }

        public void RemoveExternalUserId()
        {
            Debug.Log("logout call remove external user id");
            ajc.CallStatic("removeExternalUserId");
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

        public void GetReferrerUrl(string unityCallBack)
        {
#if !PLATFORM_AMAZON
            ajc.CallStatic("GetReferrerURL", unityCallBack);
#endif
        }

        public void SetPIP(bool enablePIP)
        {
#if !PLATFORM_AMAZON
            ajc.CallStatic("SetPIP", enablePIP);
#endif
        }

        public void TestCrashlytics(string msg)
        {
            var message = new AndroidJavaObject("java.lang.String", msg);
            var exception = new AndroidJavaObject("java.lang.Exception", message);

            var looperClass = new AndroidJavaClass("android.os.Looper");
            var mainLooper = looperClass.CallStatic<AndroidJavaObject>("getMainLooper");
            var mainThread = mainLooper.Call<AndroidJavaObject>("getThread");
            var exceptionHandler = mainThread.Call<AndroidJavaObject>("getUncaughtExceptionHandler");
            exceptionHandler.Call("uncaughtException", mainThread, exception);
            // ajc.CallStatic("OnTestRuntimeCrash", msg);
        }

        public void ShareSNSUrl(string url)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
            intentObject.Call<AndroidJavaObject> ("setAction", intentClass.GetStatic<string> ("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject> ("setType", "text/plain");
            intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_TEXT"), url);
            AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
            currentActivity.Call("startActivity", intentObject);
        }

        public void MoveHomeScreen()
        {
#if !PLATFORM_AMAZON
            ajc.CallStatic("MoveHomeScreen");
#endif
        }
    }
}
#endif
