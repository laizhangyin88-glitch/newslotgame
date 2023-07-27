using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using UnityEngine.CrashReportHandler;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class Login : ActionTask
    {
        protected override string info { get { return "Request Login"; } }

        protected override void OnExecute()
        {
            if (ApplicationSettings.LogTest())
                Debug.Log("Request Login");

#if (UNITY_STANDALONE_WIN || UNITY_WEBGL) && !UNITY_EDITOR
		BagelCodeClientAPI.LoginFacebook(
#else
            BagelCodeClientAPI.Login(
#endif
        (response) =>
        {
#if (UNITY_STANDALONE_WIN || UNITY_WEBGL) && !UNITY_EDITOR
            SocialManager.Instance.SetFacebookLongLivedAccessToken(response.facebookLongLivedAccessToken);
#endif

            LevelUtils.Clear();
            TierUtils.Clear();
            NumberUtils.Clear();
            ClubUtils.Clear();
            EpicPassUtils.Clear();
            BlackboardQueryUtils.ClearGameAndSlotInfos();

            ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);

            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "fromLogin", true);
            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "sessionAlive", true);
            BlackboardQueryUtils.UpdateDailyBoostState();

            LevelUpDash.LevelUpDash.Utils.UpdateLevelUpDashState();

            TimeUtils.UpdateTotalSessionTime(response.me.sessionTotalLength);
            TimeUtils.UpdateSessionBeginTime();

            PassiveEventManager.Instance.Clear();
            PassiveEventManager.Instance.AddEventInfoList(response.serverTime, response.ongoingEventList);

            InitializeMainBlackboard();
            UpdateString(response.clientString);

            if (response.me != null)
            {
                NativeHelper.Instance.SetExternalUserId(response.me.userId);
            }

            CrashReportHandler.SetUserMetadata("Meta.userId", response.me.userId);
            ThirdPartyAnalyticsManager.UpdateRevenueSection(response.adjustConversionValueTargetRevenue);

            BlackboardUtils.SetOrCreateValue(MainBlackboard.Get(), "metaInterruptingCount", 0);

            UpdateAgeGateTry();

            EndAction(true);
        },
            (error) =>
            {
                switch (error.errorCode)
                {
                    case ClientModels.Error.DEPRECATED_VERSION_ERROR:
                        ClientModels.ErrorDetailInfoDeprecatedVersion deprecatedVersion = error.errorDetailInfo as ClientModels.ErrorDetailInfoDeprecatedVersion;

                        if (deprecatedVersion != null)
                        {
                            bool isError = false;

                            ErrorPopupInfo info = new ErrorPopupInfo();

                            info.type = ErrorPopupType.OkWithTitle;
                            info.title = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_TITLE", out isError);
#if UNITY_EDITOR || UNITY_IOS
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_REWARD_FOR_IOS", out isError, deprecatedVersion.rewardCredit);
#else
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DEPRECATED_VERSION_ERROR_WITH_REWARD", out isError, deprecatedVersion.rewardCredit);
#endif

                            info.useXButton = false;

                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OK", out isError);
                            info.buttonAutoClose1 = false;

                            info.callback1 = delegate
                            {
                                Application.OpenURL(deprecatedVersion.appDownloadUrl);
                            };

                            ErrorPopupHandler.Instance.OpenError(info);
                        }
                        break;
                    case ClientModels.Error.INVALID_ACCESS_TOKEN_ERROR:
                        SocialManager.Instance.LoginFB(
                            (FacebookLoginResponse result) =>
                            {
                                GlobalErrorHandler.GlobalError(error);
                            }
                        );
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });
        }

        private void InitializeMainBlackboard()
        {
            BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "feedEventList");
            BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "challengeEventList");
            BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "newEpicEventList");
        }

        private void UpdateString(string clientString)
        {
            if (string.IsNullOrEmpty(clientString)) return;

            var jsonObj = SlotSimpleJson.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(clientString);
            if (jsonObj.ContainsKey("GLOBAL"))
            {
                StringTableUtils.UpdateString(StringTable.StringTableType.Global, jsonObj["GLOBAL"]);
            }
        }

        private void UpdateAgeGateTry()
        {
            int num = PlayerPrefs.GetInt("AGE_GATE_TRY");
            string timeString = PlayerPrefs.GetString("AGE_GATE_DECLINE_TIME");

            if (timeString != "")
            {
                DateTime pastTime = DateTime.Parse(timeString);
                long curTimeGap = (long)(TimeUtils.GetLocalDateTime() - pastTime).Minutes;
#if DEV
                if (curTimeGap > 5)
#else
            if (curTimeGap > 360)
#endif
                {
                    PlayerPrefs.SetInt("AGE_GATE_TRY", 0);
                    PlayerPrefs.SetString("AGE_GATE_DECLINE_TIME", "");
                }
            }
        }
    }
}
