using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.Sso.AppleLogin;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class LoginWithApple : ActionTask
    {
        public BBParameter<string> appleUserIdKeyValue;
        public BBParameter<string> appleUserIdTokenKeyValue;
        public BBParameter<string> appleUserEmailValue;
        public BBParameter<string> appleUserFirstNameValue;
        public BBParameter<string> appleUserLastNameValue;

        public BBParameter<string> result;

        private StringTable.StringTableType stringTableType = StringTable.StringTableType.Global;

        protected override string info { get { return "Login With Apple"; } }

        protected override void OnExecute()
        {
#if UNITY_IOS && !UNITY_EDITOR
            result.value = "";
            // Login with apple is only supported on devices with iOS 13 or above.
            if (BagelCode.Utils.DeviceUtils.GetDeviceOSVersion() < 13)
            {
                // Version 13 >
                GlobalErrorHandler.OpenErrorOKPopup(StringTableUtils.GetString(stringTableType, "ERROR_LOGIN_APPLE_IOS_UNSUPPORTED"), "OK", ()=>{if(agent != null) EndAction();} );
                result.value = "OnError";
            }
            else
            {
                NativeHelper.Instance.LoginWithApple(OnHandleResponseFromApple);
            }
#else
            GlobalErrorHandler.OpenErrorOKPopup(StringTableUtils.GetString(stringTableType, "ERROR_APPLE_LOGIN"), "OK", ()=>{if(agent != null) EndAction();} );
            result.value = "OnError";
#endif
            EndAction();
        }

        private void OnHandleResponseFromApple(AppleLoginCallbackArgs callBackInfo)
        {
            if (!callBackInfo.Equals(null) && string.IsNullOrEmpty(callBackInfo.error))
            {
                appleUserIdKeyValue.value = callBackInfo.userInfo.userId;
                appleUserIdTokenKeyValue.value = callBackInfo.userInfo.idToken;
                appleUserEmailValue.value = callBackInfo.userInfo.email;
                appleUserFirstNameValue.value = callBackInfo.userInfo.firstName;
                appleUserLastNameValue.value = callBackInfo.userInfo.surname;

                result.value = "OnSuccess";
            }
            else
            {
                // An error has occured, show the player the error overlay.
                string errorContent = "";

                if (callBackInfo.Equals(null))
                { 
                    // The native function hasn't returned an object.
                    errorContent = "Info is null";
                }
                else
                {
                    // Theres an error with the capability
                    errorContent = callBackInfo.error;
                }
#if DEV
                GlobalErrorHandler.OpenErrorOKPopup(StringTableUtils.GetString(stringTableType, "ERROR_APPLE_LOGIN") + "<style=body>" + errorContent + "</style>", "OK", ()=>{if(agent != null) EndAction();} );
#else
                GlobalErrorHandler.OpenErrorOKPopup(StringTableUtils.GetString(stringTableType, "ERROR_APPLE_LOGIN"), "OK", ()=>{if(agent != null) EndAction();} );
#endif
                result.value = "OnError";
            }
        }
    }
}
