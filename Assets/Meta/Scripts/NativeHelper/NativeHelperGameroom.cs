#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;

namespace BagelCode
{
    public class NativeHelperGameroom : INativeHelper
    {
        private string nativeHelperUnityObject;

        public void Initialize(string name)
        {
            nativeHelperUnityObject = name;
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
            bool networkUp = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            return networkUp;
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
            string facebookId = SocialManager.Instance.GetFacebookId();
            System.Guid guid = Logos.Utility.GuidUtility.Create(System.Guid.Empty, facebookId);
            return guid.ToString();
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

        private System.Action<byte[]> nativeAction;
        public void GetProfileImage(string unityCallBack, int width, int height, bool isCropable, System.Action<byte[]> action)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Image selectedImage = System.Drawing.Image.FromFile(ofd.FileName);
                System.Drawing.Bitmap resizedBitmap = ImageUtilsGameroom.ResizeImage(selectedImage, width, height);
                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                resizedBitmap.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] byteToEncode = new byte[memStream.Length];
                memStream.Position = 0;
                memStream.Read(byteToEncode, 0, byteToEncode.Length);
                memStream.Close();

                string encodedData = System.Convert.ToBase64String(byteToEncode);
                nativeAction = action;
                UnitySendMessageWrapper(nativeHelperUnityObject, unityCallBack, encodedData);
            }
            else
            {
                nativeAction = action;
                UnitySendMessageWrapper(nativeHelperUnityObject, unityCallBack, "");
            }
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
            // not supported
        }

        public void DeleteLocalPush(int pushID)
        {
            // not supported
        }

        public void TogglePush()
        {
            // not supported
        }

        public void InitAdjust(string objectName, string methodName)
        {
            //Adjust Windows SDK not implemented
        }

        public string GetAdjustID()
        {
            //Adjust Windows SDK not implemented
            return "GAMEROOM_DUMMY_ADJUST_ID";
        }

        public void SendAdjustEvent(string eventID, string eventToken)
        {
            //Adjust Windows SDK not implemented
        }

        public void SendAdjustRevenueEvent(string eventID, string eventToken, double revenue, string purchaseID)
        {
            //Adjust Windows SDK not implemented
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

        public void OnApplicationExit()
        {
            Application.Quit();
        }

        public void OnLocalPushMessage(string message)
        {
            // Nothing to do
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

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,
        out ulong lpFreeBytesAvailable,
        out ulong lpTotalNumberOfBytes,
        out ulong lpTotalNumberOfFreeBytes);
        public long GetFreeDiskSpace()
        {
            ulong freeBytesAvail;
            ulong totalNumOfBytes;
            ulong totalNumOfFreeBytes;

            if (!GetDiskFreeSpaceEx(System.IO.Path.GetPathRoot(System.Environment.CurrentDirectory), out freeBytesAvail, out totalNumOfBytes, out totalNumOfFreeBytes))
            {
                Debug.LogError("Failed to execute GetDiskFreeSpaceEx");
                // to be conservative
                return 1000L;
            }
            else
            {
                Debug.Log("    Available bytes : " + freeBytesAvail);
                Debug.Log("    Total # of bytes: " + totalNumOfBytes);
                Debug.Log("    Total free bytes: " + totalNumOfFreeBytes);
                ulong totalNumOfFreeMbytes = totalNumOfFreeBytes / 1024 / 1024;
                return (long)totalNumOfFreeMbytes;
            }
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
