using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using UnityEngine.CrashReportHandler;
using SimpleJSON;
using BagelCode.Protobuf;
using System.Collections;
using SlotMaker.TestSuite;
using RPCBase;
using UnityEngine.Rendering;

namespace BagelCode.Tasks.Actions.ClientAPI
{

    [Category("★ BagelCode/ClientAPI")]
    public class Login : ActionTask
    {
        protected override string info { get { return "Request Login"; } }


        protected override void OnExecute()
        {


#if NEW_NET



 /*
            RPCKenoClassic.ReqKenoSpin req1 = new RPCKenoClassic.ReqKenoSpin();
            req1.betPerTicket = 1000;
            req1.pickInfoList = new List<List<int>>();
            req1.pickInfoList.Add(new List<int> { 25, 41, 52, 55, 56, 63, 68, 71, 77, 80 });
            string buffer1 = JsonUtility.ToJson(req1); //{"betPerTicket":1000}

           Dictionary<string, object> jsonNode = new Dictionary<string, object>
            {
                { "protocol_key", "000"},
                { "data", req1},
            };
            string buffer = SlotSimpleJson.SerializeObject(jsonNode);

            RPCBase.ReqBase tmp =SlotSimpleJson.DeserializeObject<RPCBase.ReqBase>(buffer);

            string buf1 = SlotSimpleJson.SerializeObject(tmp);

            Dictionary<string, object> jsonNode2 = new Dictionary<string, object>
            {
                { "a1", "123"},
                { "b1", 5566},
            };
            Dictionary<string, object> jsonNode3 = new Dictionary<string, object>
            {
                { "protocol_key", "123"},
                { "data", jsonNode2},
            };
            string buffer2 = SlotSimpleJson.SerializeObject(jsonNode3);


            if(jsonNode3 is IDictionary)
            {
                Debug.Log("jsonNode3是字典");
            }
            else
            {
                Debug.Log("jsonNode3不是字典");
            }
            if (req1 is IDictionary)
            {
                Debug.Log("req 是字典");
            }
            else
            {
                Debug.Log("req 不是字典");
            }*/


            List<object> req = new List<object> { globalStore.gToken };
            //res.Add(globalStore.gToken);
            NetManager.Instance.Post(RPCName.login, req,
            (res) =>
            {

                string resStr = res.ToString();

                TextAsset jsn0 = Resources.Load<TextAsset>("tempdata/login_response");
                ClientModels.LoginResponse response = JsonUtility.FromJson<ClientModels.LoginResponse>(jsn0.text);

                response.me.credit = res["balance"].AsLong;
                response.me.userId = (string)res["user_id"] != null ? (string)res["user_id"] : "0";
                response.me.name = (string)res["name"];

                LevelUtils.Clear();
                TierUtils.Clear();
                NumberUtils.Clear();
                ClubUtils.Clear();
                EpicPassUtils.Clear();
                BlackboardQueryUtils.ClearGameAndSlotInfos();

                Debug.Log("@ login 赋值 给 bb");
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

                return;
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




            return;
#endif






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


            string oldJson = JsonUtility.ToJson(response);
            Debug.Log($"@A LoginResponse = {oldJson}");


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
