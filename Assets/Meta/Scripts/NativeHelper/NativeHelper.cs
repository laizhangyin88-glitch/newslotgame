using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BagelCode.Sso;

namespace BagelCode
{

public class NativeHelper : SlotMaker.MonoWeakSingleton<NativeHelper>
{
    public const uint PROFILE_IMAGE_MIN_SIDE = 152 * 2;

    private INativeHelper delegator;

    public void Awake()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        delegator = new NativeHelperIOS();
#elif UNITY_ANDROID && !UNITY_EDITOR
        delegator = new NativeHelperAndroid();
#elif UNITY_WSA && !UNITY_EDITOR
        delegator = new NativeHelperWindows();
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        delegator = new NativeHelperGameroom();
#elif UNITY_WEBGL && !UNITY_EDITOR
        delegator = new NativeHelperCanvas();
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
        delegator = new NativeHelperOSXStandalone();
#else
            delegator = new NativeHelperUnity();
#endif
    }

    public void SetBlackboardValue(string data)
    {
        if (SlotMaker.ApplicationSettings.LogSystem())
            Debug.Log("NativeHelper SetBlackboardValue : " + data);

        var tokens = data.Split(':');

        SlotMaker.BlackboardUtils.SetOrCreateValue( SlotMaker.MainBlackboard.Get(), tokens[0], tokens[1] );
    }

    public void Initialize()
    {
        delegator.Initialize(gameObject.name);
    }

    public void OnEnable()
    {
        delegator.OnEnable();
    }

    public void OnDisable()
    {
        delegator.OnDisable();
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        delegator.OnApplicationPause(pauseStatus);
    }

    public void OnApplicationFocus(bool focusStatus)
    {
        delegator.OnApplicationFocus(focusStatus);
#if UNITY_WSA
        if(focusStatus)
        {
            // Debug.LogError("Update Focus");
            UpdatePinState();
        }
#endif
    }

    public bool IsNetworkAvailable()
    {
        return delegator.IsNetworkAvailable();
    }

    public void RegisterForNotification()
    {
        delegator.RegisterForNotification();
    }

    public string GetNotificationToken()
    {
        return delegator.GetNotificationToken();
    }

    public void UpdateOneSignalToken(string token)
    {
        if (String.IsNullOrEmpty(token)) return;

        if (SlotMaker.ApplicationSettings.LogSystem())
            Debug.Log("[OneSignal] : Update token " + token);

        BagelCodeClientAPI.OnesignalRegister(token,
            (res) =>
            {
                if (SlotMaker.ApplicationSettings.LogSystem())
                    Debug.Log("[OneSignal] : Success with " + res.ToString());
            },
            (err) =>
            {
                if (SlotMaker.ApplicationSettings.LogSystem())
                    Debug.LogError("[OneSignal] : Fail with " + err.error);
            });

        SlotMaker.BlackboardUtils.SetOrCreateValue( SlotMaker.MainBlackboard.Get(), "oneSignalId", token );

    }

    private string deviceId;
    public string GetDeviceID()
    {
        if (deviceId == null) {
            deviceId = delegator.GetDeviceID();
        }
        return deviceId;
    }

    private string serverBaseUrl;
    public string GetServerBaseUrl()
    {
        if (serverBaseUrl == null) {
            serverBaseUrl = delegator.GetServerBaseUrl();
        }
        return serverBaseUrl;
    }

    private string chattingUrl;
    public string GetChattingUrl()
    {
        if (chattingUrl == null) {
            chattingUrl = delegator.GetChattingUrl();
        }
        return chattingUrl;
    }

    private string appDownloadUrl;
    public string GetAppDownloadUrl()
    {
        if (appDownloadUrl == null) {
            appDownloadUrl = delegator.GetAppDownloadUrl();
        }
        return appDownloadUrl;
    }

    public void GetProfileImage(int width, int height, bool isCropable, System.Action<byte[]> action)
    {
        delegator.GetProfileImage("OnProfileImageLoaded", width, height, isCropable, action);
    }

    public void OnProfileImageLoaded(string encodedBinary)
    {
        delegator.OnProfileImageLoaded(encodedBinary);
    }

    public void SetLocalPush(int pushID, string sender, string message, int delay, string type, bool doNotDisturb)
    {
        delegator.SetLocalPush(pushID, sender, message, delay, type, doNotDisturb);
    }

    public void DeleteLocalPush(int pushID)
    {
        delegator.DeleteLocalPush(pushID);
    }

    public void TogglePush()
    {
        delegator.TogglePush();
    }

    public void InitAdjust(string objectName, string methodName)
    {
        delegator.InitAdjust(objectName, methodName);
    }

    public string GetAdjustID()
    {
        return delegator.GetAdjustID();
    }

    public void SendAdjustEvent(string eventID, string eventToken)
    {
        delegator.SendAdjustEvent(eventID, eventToken);
    }

    public void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID)
    {
        delegator.SendAdjustRevenueEvent(eventID, eventToken, revenue, purchaseID);
    }

    public void SendFIREvent(string eventID)
    {
        delegator.SendFIREvent(eventID);
    }

    public void CopyClipboard(string str)
    {
        delegator.CopyClipboard(str);
    }

    public void OnApplicationExit()
    {
        delegator.OnApplicationExit();
    }

    public void OnLocalPushMessage(string message)
    {
        delegator.OnLocalPushMessage(message);
    }

    public void OnPendingMessage(string message)
    {
        delegator.OnPendingMessage(message);
    }

    public void OnOpenUrl(string uri)
    {
        delegator.OnOpenUrl(uri);
    }

    public void OpenUrl(string url)
    {
        delegator.OpenUrl(url);
    }

    public long GetFreeDiskSpace()
    {
        return delegator.GetFreeDiskSpace();
    }

    public string GetContextId()
    {
        return delegator.GetContextId();
    }

    public void SetIdleTimerDisabled(bool value)
    {
        delegator.SetIdleTimerDisabled(value);
    }

    private System.Action surveyAction;
    public void DoSurvey(string hash, string userId, System.Action action)
    {
        surveyAction = action;
        delegator.DoSurvey(hash, userId);
    }

    public void OnSurveyEnd(string message)
    {
        if (surveyAction != null)
        {
            surveyAction();
            surveyAction = null;
        }
    }

    public bool GetPushNotificationSubscribed()
    {
        return delegator.GetPushNotificationSubscribed();
    }

    public void SetSupportPageUrl(string url)
    {
        delegator.SetSupportPageUrl(url);
    }

    public void OpenAppSettings()
    {
        delegator.OpenAppSettings();
    }

    public void OpenAppSettingsPipMode()
    {
        delegator.OpenAppSettingsPipMode();
    }

    public bool GetAppSettingsPipModeEnabled()
    {
        return delegator.GetAppSettingsPipModeEnabled();
    }

    public bool GetAppSettingsPipModeAvailable()
    {
        return delegator.GetAppSettingsPipModeAvailable();
    }

    public void LoginWithApple(System.Action<Sso.AppleLogin.AppleLoginCallbackArgs> callback)
    {
        delegator.LoginWithApple(callback);
    }

    public bool IsPinningAllowed()
    {
        return delegator.IsPinningAllowed();
    }

    private System.Action<bool> pinActionCallback;
    public void IsPinned(System.Action<bool> callback)
    {
        pinActionCallback = callback;
        delegator.IsPinned("OnPinActionCallback");
    }

    public void SetPin(System.Action<bool> callback)
    {
        pinActionCallback = callback;
        delegator.SetPin("OnPinActionCallback");
    }

    public void OnPinActionCallback(string callback)
    {
        if(pinActionCallback != null)
        {
            pinActionCallback(callback == "True" ? true : false);
        }
        pinActionCallback = null;
    }

    public void SetExternalUserId(string userId)
    {
        Debug.Log("login call set external user id");
        delegator.SetExternalUserId(userId);
    }

    public void RemoveExternalUserId()
    {
        Debug.Log("logout call remove external user id");

        delegator.RemoveExternalUserId();
    }

    public void TestCrashlytics(string msg)
    {
        delegator.TestCrashlytics(msg);
    }

    public void ShareSNSUrl(string url)
    {
        delegator.ShareSNSUrl(url);
    }

    public void UpdatePinState()
    {
#if UNITY_WSA
        // Debug.LogError("UpdatePinState");
        if( !IsPinningAllowed() )
            return;

        if(pinActionCallback == null)
        {
            IsPinned(
                (isPinned) =>
                {
                    SlotMaker.BlackboardUtils.SetOrCreateValue<bool>(SlotMaker.MainBlackboard.Get(), "isWindowsPinned", isPinned);
                    // Debug.LogError(string.Format("Update Pinned = {0}", isPinned));
                }
            );
        }
#endif
    }

    private System.Action<string> referrerUrlCallback;
    public void GetReferrerUrl(System.Action<string> callback)
    {
        referrerUrlCallback = callback;
        delegator.GetReferrerUrl("OnReferrerUrlCallback");
    }

    public void OnReferrerUrlCallback(string referrerUrl)
    {
        if(referrerUrlCallback != null)
            referrerUrlCallback(referrerUrl);

        referrerUrlCallback = null;
    }

    public void SetPIP(bool enable)
    {
        delegator.SetPIP(enable);
    }

    public void MoveHomeScreen()
    {
        delegator.MoveHomeScreen();
    }

    private System.Action<bool> pipActionCallback;
    public void RegistPipCallback(System.Action<bool> action)
    {
        pipActionCallback = action;
    }

    public void UnRegistPipCallback()
    {
        pipActionCallback = null;
    }

    public void OnChangePipMode(string inPictureInPictureMode)
    {
        // inPictureInPictureMode. True or False

        // Pip mode changed callback.
        if(pipActionCallback != null)
        {
            pipActionCallback(inPictureInPictureMode == "True");
        }
    }
}

}
