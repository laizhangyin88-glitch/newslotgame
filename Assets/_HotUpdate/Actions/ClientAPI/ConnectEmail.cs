using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class ConnectEmail : ActionTask
{
    public BBParameter<string> email;
    public BBParameter<string> validationCode;
    public BBParameter<string> result;
    public BBParameter<string> errorMessage;

    protected override string info { get { return "Connect Email"; } }

    protected override void OnExecute()
    {
        bool stringError = false;
        result.value = "";
        // GraphOwner owner = agent.GetComponent<GraphOwner>();
        string contextId = PlayerPrefs.GetString("VIP_CLUB_FUNNEL_CONTEXT_ID", "");

        if(!string.IsNullOrEmpty(validationCode.value))
            validationCode.value = validationCode.value.Trim();

        BagelCodeClientAPI.SsoConnectEmail(email.value, validationCode.value, contextId,  
        (response) =>
        {
            if(agent != null)
            {
                Blackboard bb = agent.GetComponent<Blackboard>();
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardQueryUtils.AddCoins(response.earnCredit);

                var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "ssoAccountInfo");

                if (response.successSignIn)
                {
                    if (ssoAccountInfo == null)
                    {
                        // new
                        ClientAPI2Blackboard.Serialize(BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "ssoAccountInfo"), response.ssoAccountInfo);

                        result.value = "OnNewJoin";
                    }
                    else
                    {
                        var associatedser = BlackboardUtils.FindVariable(bb, "associatedUser");
                        if(associatedser != null)
                        {
                            result.value = "OnExistJoin";
                        }
                        else
                        {
                            ClientAPI2Blackboard.Serialize(ssoAccountInfo.value, response.ssoAccountInfo);
                            // update sso account info including email
                            // UpdateSSOAccountInfo(bb);
                            
                            result.value = "OnSecondsValidate";
                        }
                    }
                }
                else
                {
                    var associatedser = BlackboardUtils.FindVariable(bb, "associatedUser");
                    if (associatedser != null)
                    {
                        result.value = "OnExistJoin";
                    }
                    else
                    {
                        errorMessage.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_JOIN_ALREADY_EMAIL", out stringError);
                        result.value = "OnError";
                    }
                }
            }
            
        },
        (error) =>
        {
            switch (error.errorCode)
            {
                case BagelCode.ClientModels.Error.ALREADY_BOUND_ACCOUNT_ERROR:
                    errorMessage.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_JOIN_ALREADY_EMAIL", out stringError);
                    result.value = "OnError";
                    break;
                case BagelCode.ClientModels.Error.INVALID_VALIDATION_CODE:
                    errorMessage.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_EMAIL_JOIN_INVALID_CODE_TEXT", out stringError);
                    result.value = "OnRetry";
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });

        EndAction();
    }

    // private void UpdateSSOAccountInfo(Blackboard bb)
    // {
    //     var sourceBB = BlackboardUtils.FindVariable(bb, "ssoAccountInfo");

    //     if (sourceBB != null)
    //     {
    //         IBlackboard locationBB = null;
    //         string targetBBName = null;

    //         locationBB = BlackboardUtils.FindBlackboard(locationBB, "/ssoAccountInfo", ref targetBBName);

    //         IBlackboard target = BlackboardUtils.GetOrCreateBlackboard(locationBB, targetBBName);
    //         target.variables.Clear();

    //         string sourceInfo = ((Blackboard)sourceBB.value).Serialize();
    //         ((Blackboard)target).Deserialize(sourceInfo);
    //     }
    // }
}

}
