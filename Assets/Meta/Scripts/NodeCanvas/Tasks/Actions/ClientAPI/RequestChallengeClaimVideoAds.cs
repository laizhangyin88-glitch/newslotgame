using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestChallengeClaimVideoAds : ActionTask<Blackboard>
{
    public BBParameter<string> challengeTypeValue;

    protected override string info { get { return "Request Challenge Claim Video Ads"; } }

    protected override void OnExecute()
    {
        var claimChallengeType = BlackboardUtils.FindVariable<ChallengeType>(agent, challengeTypeValue.value);

        BagelCodeClientAPI.ChallengeClaimVideoAds( claimChallengeType.value,
        (response) =>
        {
            // if(agent != null)
            // {
                // var claimResponse = BlackboardUtils.GetOrCreateBlackboard(agent, "claimResponse");

                // ClientAPI2Blackboard.Serialize(claimResponse, response);
                // ClientAPI2Blackboard.Serialize(claimChallengeInfo.value, response.nextChallengeInfo);

                // var lastRewardResultBB = BlackboardUtils.GetOrCreateBlackboard(claimChallengeInfo.value, "lastRewardResult");
                // ClientAPI2Blackboard.Serialize(lastRewardResultBB, response.rewardResult);

                // BlackboardQueryUtils.UpdateChallengeSimpleInfo(claimChallengeInfo.value);
            // }

            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
