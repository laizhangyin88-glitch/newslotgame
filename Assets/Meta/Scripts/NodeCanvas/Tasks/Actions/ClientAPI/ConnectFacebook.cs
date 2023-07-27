using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class ConnectFacebook : ActionTask<Blackboard>
{
    public BBParameter<string> id;
    protected override string info { get { return "Connect Facebook"; } }

    protected override void OnExecute()
    {
#if DEV
        Debug.Log( string.Format("Facebook Connect ID : {0}", id.value));
#endif

        string contextId = PlayerPrefs.GetString("VIP_CLUB_FUNNEL_CONTEXT_ID", "");
        
        BagelCodeClientAPI.SsoConnectFacebook(id.value, contextId, 
        (response) =>
        {
            BlackboardQueryUtils.AddCoins(response.earnCredit);

            if(agent != null)
            {
                ClientAPI2Blackboard.Serialize(agent, response);

                var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "ssoAccountInfo");

                if (response.successSignIn)
                {
                    var facebookId = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "ssoAccountInfo/facebookId");

                    if (ssoAccountInfo == null || facebookId == null || string.IsNullOrEmpty(facebookId.value))
                    {
                        ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "ssoAccountInfo"), response.ssoAccountInfo);
                        ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "updatedUserInfo"), response.updatedUserInfo);

                        Blackboard me = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "me").value;
                        me.SetValue("name", response.updatedUserInfo.name);
                        me.SetValue("profileUrl", response.updatedUserInfo.profileUrl);
                        me.SetValue("profileHighResolutionUrl", response.updatedUserInfo.profileUrl);
                        me.SetValue("gender", response.updatedUserInfo.gender);
                        me.SetValue("profileImageUploadCount", response.updatedUserInfo.profileImageUploadCount);

                        SendEvent("OnNewJoin");
                    }
                    else
                    {
                        SendEvent("OnExistJoin");
                    }
                }
                else
                {
                    if (response.associatedUser != null)
                    {
                        SendEvent("OnExistJoin");
                    }
                    else
                    {
                        SocialManager.Instance.LogoutFB();
                        SendEvent<string>("Error", "POPUP_JOIN_ALREADY_FACEBOOK");
                    }
                }
            }
        },
        (error) =>
        {
#if DEV
            Debug.Log( string.Format("ConnectFacebook error : {0}", error.errorCode));
#endif

            SocialManager.Instance.LogoutFB();

            switch(error.errorCode)
            {
                case ClientModels.Error.ALREADY_BOUND_ACCOUNT_ERROR:
                {
                    SendEvent<string>("Error", "POPUP_JOIN_ALREADY_FACEBOOK");
                    break;
                }
                case ClientModels.Error.FACEBOOK_ERROR:
                {
                    SendEvent<string>("Error", "ERROR_CONNECT_FACEBOOK_FAILED");
                    break;
                }
                default:
                {
                    SendEvent<string>("Error", "POPUP_JOIN_FAIL_TO_FACEBOOK");
                    break;
                }
            }
        });
        
        EndAction(true);
    }
    
}

}
