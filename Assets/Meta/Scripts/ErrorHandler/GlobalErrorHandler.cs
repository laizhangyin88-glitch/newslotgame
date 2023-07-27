using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BagelCode.Internal;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public static class GlobalErrorHandler
    {
        public static void GlobalError(BagelCodeHTTPError httpError)
        {
            ErrorPopupInfo info = new ErrorPopupInfo();

            bool stringError = false;
            switch (httpError.errorCode)
            {
                case Error.SERVER_UNDER_CONSTRUCTION:
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_SERVER_UNDER_CONSTRUCTION", out stringError);
#if UNITY_ANDROID
                    info.type = ErrorPopupType.OK;
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                    info.buttonAutoClose1 = false;

                    info.callback1 = delegate
                    {
                        Application.Quit();
                    };
#else
                info.type = ErrorPopupType.TextOnly;
#endif
                    break;
                case Error.CLIENT_TIME_ERROR:
                    info.type = ErrorPopupType.SystemReset;
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_CLIENT_TIME", out stringError);
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                    break;
                // case Error.DEPRECATED_VERSION_ERROR:
                //     info.type = ErrorPopupType.OK;
                //     info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR", out stringError);
                //     info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                //     info.buttonAutoClose1 = false;
                //     info.callback1 = delegate
                //     {
                //         Application.OpenURL(NativeHelper.Instance.GetAppDownloadUrl());
                //         // ErrorPopupHandler.Instance.OpenError(info);
                //     };

                //     // Open Google or Store Link..
                //     break;
                case Error.SESSION_EXPIRED_ERROR:
                    info.type = ErrorPopupType.SystemReset;
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_SESSION_EXPIRED", out stringError);
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                    BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "sessionAlive", false);
                    break;
                case Error.SESSION_MULTI_LOGIN_ERROR:
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_SESSION_MULTI_LOGIN", out stringError);
#if UNITY_ANDROID
                    info.type = ErrorPopupType.OK;
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

                    info.buttonAutoClose1 = false;

                    info.callback1 = delegate
                    {
                        Application.Quit();
                    };
#else
                info.type = ErrorPopupType.TextOnly;
#endif
                    BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "sessionAlive", false);
                    break;
                case Error.TIMEZONE_OFFSET_ERROR:
                    info.type = ErrorPopupType.SystemReset;
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_TIMEZONE_OFFSET", out stringError);
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                    break;
                case Error.BANNED_USER_ERROR:
                    var detailInfo = (ErrorDetailInfoBannedUser)httpError.errorDetailInfo;

                    info.text = detailInfo.isPermanent ?
                        StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_BANNED_USER_PERMANENT",out stringError) :
                        StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_BANNED_USER", out stringError,
                        detailInfo.banEndTimestamp);

                    info.buttonAutoClose2 = false;
#if UNITY_ANDROID
                    info.type = ErrorPopupType.YesNo;
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_CLOSE", out stringError);
                    info.buttonText2 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_CUSTOMER_SUPPORT", out stringError);

                    info.callback1 = delegate
                    {
                        Application.Quit();
                    };

                    info.callback2 = delegate
                    {
                        string supportUrl = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_SUPPORT_PAGE_URL", out stringError);
                        Application.OpenURL(supportUrl);
                    };
#else
                    info.type = ErrorPopupType.OK;
                    info.buttonAutoClose1 = false;

                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_CUSTOMER_SUPPORT", out stringError);

                    info.callback1 = delegate
                    {
                        string supportUrl = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_SUPPORT_PAGE_URL", out stringError);
                        Application.OpenURL(supportUrl);
                    };
#endif
                    break;
                case Error.ACCOUNT_ALREADY_REMOVED_ERROR:
                    {
                        info.type = ErrorPopupType.SystemReset;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_ACCOUNT_REMOVED", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                    }
                    break;
                // case Error.TARGET_ALREADY_FRIEND_ERROR:
                //     info.type = ErrorPopupType.OK;
                //     info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_TARGET_ALREADY_FRIEND", out stringError);
                //     info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                //     break;
                // case Error.NOT_EXIST_FRIEND_CODE_ERROR:
                //     info.type = ErrorPopupType.OK;
                //     info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_NOT_EXIST_FRIEND_CODE", out stringError);
                //     info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                //     break;
                // case Error.CANT_ADD_MYSELF_FRIEND_ERROR:
                //     info.type = ErrorPopupType.OK;
                //     info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_CANT_ADD_MYSELF_FRIEND", out stringError);
                //     info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                //     break;
                default:
                    info.type = ErrorPopupType.SystemReset;
                    info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_UNEXPECTED", out stringError);
                    info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                    BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "sessionAlive", false);
                    break;
            }

#if DEV
            info.text += "<style=body> (" + httpError.errorCode.ToString() + ")</style>";
#endif

            ErrorPopupHandler.Instance.OpenError(info);
        }

        public static void OpenAlertPopup(string messageKey)
        {
            ErrorPopupInfo info = new ErrorPopupInfo();

            bool stringError = false;
            info.type = ErrorPopupType.OK;
            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, messageKey, out stringError);
            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

            ErrorPopupHandler.Instance.OpenError(info);
        }

        public static void OpenAlertPopup(string messageKey, object arg1)
        {
            ErrorPopupInfo info = new ErrorPopupInfo();

            bool stringError = false;
            info.type = ErrorPopupType.OK;
            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, messageKey, arg1, out stringError);
            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

            ErrorPopupHandler.Instance.OpenError(info);
        }

        public static void OpenErrorOKPopup(string message, string buttonText, System.Action callback)
        {
            bool stringError = false;
            ErrorPopupInfo info = new ErrorPopupInfo();
            info.type = ErrorPopupType.OK;
            info.text = message;
            info.buttonText1 = buttonText;

            info.callback1 = delegate
            {
                if (callback != null)
                {
                    callback();
                }
            };

            ErrorPopupHandler.Instance.OpenError(info);
        }
    }
}
