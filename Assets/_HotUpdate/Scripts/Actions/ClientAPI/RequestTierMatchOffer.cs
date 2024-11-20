using System.Collections;
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
public class RequestTierMatchOffer : ActionTask <Blackboard>
{
    public BBParameter<string> matchCode;
    
    public BBParameter<bool> isSuccess;
    public BBParameter<ClientModels.Error> errorCode;


    protected override string info { get { return "RequestTierMatchOffer"; } }

    protected override void OnExecute()
    {
        BagelCodeClientAPI.TierMatchOfferClaimRequest(matchCode.value,
            (response) =>
            {
                var bb = agent.GetComponent<Blackboard>();
                isSuccess.value = true;
                ClientAPI2Blackboard.Serialize(bb, response);

                if (response.isTierIncreased)
                {
                    BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                    BlackboardQueryUtils.ApplyUserSyncInfo();
                }

                BlackboardQueryUtils.UpdateInboxItemList(response.inboxList);
                
                EndAction(true);
            },
            (error) =>
            {
                isSuccess.value = false;
                errorCode.value = error.errorCode;
                switch(error.errorCode)
                {
                    case ClientModels.Error.ALREADY_TIER_MATCHED_CODE_ERROR:
                    case ClientModels.Error.EXPIRED_TIER_MATCH_CODE_ERROR:
                    case ClientModels.Error.INVALID_TIER_MATCH_CODE_ERROR:
                        EndAction(true);
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });
    }
}

}

