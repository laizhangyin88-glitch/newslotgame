using BagelCode.Sso;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.Sso
{
    [Category("★ BagelCode/ClientAPI")]
    public class ConnectApple : ActionTask<Blackboard>
    {
        public BBParameter<string> appleUserIdKeyValue;
        public BBParameter<string> appleUserIdTokenKeyValue;
        public BBParameter<string> appleUserEmailValue;
        public BBParameter<string> appleUserFirstNameValue;
        public BBParameter<string> appleUserLastNameValue;

        public BBParameter<string> result;

        private StringTable.StringTableType stringTableType = StringTable.StringTableType.Global;

        protected override string info { get { return "Connect Apple"; } }

        protected override void OnExecute()
        {
            result.value = "";
            
            string contextID = PlayerPrefs.GetString("VIP_CLUB_FUNNEL_CONTEXT_ID", "");

            BagelCodeClientAPI.SsoConnectApple( appleUserIdKeyValue.value,
                                                appleUserIdTokenKeyValue.value,
                                                appleUserEmailValue.value,
                                                appleUserFirstNameValue.value,
                                                appleUserLastNameValue.value,
                                                contextID,
            (response) =>
            {
                BlackboardQueryUtils.AddCoins(response.earnCredit);

                if(agent != null)
                {
                    ClientAPI2Blackboard.Serialize(agent, response);
                    var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "ssoAccountInfo");

                    if (response.successSignIn)
                    {
                        var appleID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "ssoAccountInfo/appleId");

                        if (ssoAccountInfo == null || appleID == null || string.IsNullOrEmpty(appleID.value))
                        {
                            ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "ssoAccountInfo"), response.ssoAccountInfo);
                            ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "updatedUserInfo"), response.updatedUserInfo);

                            Blackboard me = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "me").value;
                            me.SetValue("name", response.updatedUserInfo.name);
                            me.SetValue("profileUrl", response.updatedUserInfo.profileUrl);
                            me.SetValue("profileHighResolutionUrl", response.updatedUserInfo.profileUrl);
                            me.SetValue("gender", response.updatedUserInfo.gender);

                            result.value = "OnNewJoin";
                        }
                        else
                        {
                            var associatedser = BlackboardUtils.FindVariable(agent, "associatedUser");
                            if(associatedser != null)
                            {
                                result.value = "OnExistJoin";
                            }
                            else
                            {
                                ClientAPI2Blackboard.Serialize(ssoAccountInfo.value, response.ssoAccountInfo);
                                
                                result.value = "OnSecondsValidate";
                            }
                        }
                    }
                    else
                    {
                        var associatedser = BlackboardUtils.FindVariable(agent, "associatedUser");
                        if (associatedser != null)
                        {
                            result.value = "OnExistJoin";
                        }
                        else
                        {
                            GlobalErrorHandler.OpenErrorOKPopup(StringTableUtils.GetString(stringTableType, "POPUP_JOIN_ALREADY_EMAIL"), "OK", null);
                            result.value = "OnError";
                        }
                    }
                }
            },
            (error) =>
            {
                switch(error.errorCode)
                {
                    case BagelCode.ClientModels.Error.ALREADY_BOUND_ACCOUNT_ERROR:
                        {
                            GlobalErrorHandler.OpenErrorOKPopup(StringTableUtils.GetString(stringTableType, "POPUP_JOIN_ALREADY_EMAIL"), "OK", null);
                            if(agent != null)
                                result.value = "OnError";
                        }
                        break;
                    case BagelCode.ClientModels.Error.APPLE_ERROR:
                        {
                            GlobalErrorHandler.OpenErrorOKPopup(StringTableUtils.GetString(stringTableType, "ERROR_APPLE_LOGIN"), "OK", null);
                            if(agent != null)
                                result.value = "OnError";
                        }
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });

            EndAction();
        }
    }

}
