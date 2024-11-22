using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class RequestChallengeClaim : ActionTask<Blackboard>
{
    public BBParameter<string> challengeInfo;

    protected override string info { get { return "Request Challenge Claim"; } }

    protected override void OnExecute()
    {
        var claimChallengeInfo = BlackboardUtils.FindVariable<Blackboard>(agent, challengeInfo.value);
        var claimChallengetype = BlackboardUtils.FindVariable<ChallengeType>(claimChallengeInfo.value, "challengeType");

        int multiplierEventID = 0;
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CHALLENGE_CLAIM_MULTIPLY);
        if(eventInfo != null)
            multiplierEventID = eventInfo.id;
            
        BagelCodeClientAPI.ChallengeClaim( claimChallengetype.value, multiplierEventID, 
        (response) =>
        {
            if(agent != null)
            {
                var claimResponse = BlackboardUtils.GetOrCreateBlackboard(agent, "claimResponse");

                ClientAPI2Blackboard.Serialize(claimResponse, response);
                ClientAPI2Blackboard.Serialize(claimChallengeInfo.value, response.nextChallengeInfo);
                BlackboardUtils.SetOrCreateList(claimChallengeInfo.value, "lastRewardResultList", response.rewardResultList, ClientAPI2Blackboard.Serialize);

                BlackboardQueryUtils.UpdateChallengeSimpleInfo(claimChallengeInfo.value);

                ChallengeUtils.PersonalChallengeTypeToMetaChallengeType(claimChallengetype.value, out MetaChallengeType type);
                ChallengeUtils.SetChallengeCheckTimeCurrent(type);

                EndAction(true);
            }
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
}

}
