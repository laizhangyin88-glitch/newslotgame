using UnityEngine;

namespace BagelCode
{

public interface INativeHelper
{
    void Initialize(string name);
    bool IsNetworkAvailable();
    void RegisterForNotification();
    string GetNotificationToken();
    string GetDeviceID();
    string GetServerBaseUrl();
    string GetChattingUrl();
    string GetAppDownloadUrl();
    void GetProfileImage(string unityCallBack, int width, int height, bool isCropable, System.Action<byte[]> action);
    void OnProfileImageLoaded(string encodedBinary);
    void SetLocalPush(int pushID, string sender, string message, int delay, string type, bool doNotDisturb);
    void DeleteLocalPush(int pushID);
    void TogglePush();
    void InitAdjust(string objectName, string methodName);
    string GetAdjustID();
    void SendAdjustEvent(string eventID, string eventToken);
    void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID);
    void SendFIREvent(string eventID);
    void OnApplicationPause(bool pauseStatus);
    void OnApplicationFocus(bool focusStatus);
    void OnEnable();
    void OnDisable();
    void CopyClipboard(string str);
    void OnApplicationExit();
    void OnLocalPushMessage(string message);
    void OnPendingMessage(string data);
    void OnOpenUrl(string uri);
    void OpenUrl(string url);
    long GetFreeDiskSpace();
    string GetContextId();
    void SetIdleTimerDisabled(bool value);
    void DoSurvey(string hash, string userId);
    bool GetPushNotificationSubscribed();
    void OpenAppSettingsPipMode();
    bool GetAppSettingsPipModeEnabled();
    bool GetAppSettingsPipModeAvailable();
    void SetSupportPageUrl(string url);
    void OpenAppSettings();
    void LoginWithApple(System.Action<Sso.AppleLogin.AppleLoginCallbackArgs> callback);

    void SetExternalUserId(string userId);
    void RemoveExternalUserId();

    // Windows
    bool IsPinningAllowed();
    void IsPinned(string unityCallBack);
    void SetPin(string unityCallback);
    void TestCrashlytics(string msg);
    void ShareSNSUrl(string url);
    void GetReferrerUrl(string unityCallback);

    void SetPIP(bool enable);
    void MoveHomeScreen();
}

}
