using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_challenge_claim : ActionTask<Blackboard>
{
    public BBParameter<string> rewardResultList;
    public BBParameter<string> newChallengeInfo;
    public BBParameter<string> prevChallengeID;
    public BBParameter<string> challengeProgressCount;
    public BBParameter<string> challengeEventMultiplier;

    protected override void OnExecute()
    {
        var rewardResultListBB = BlackboardUtils.FindVariable<List<Blackboard>>(agent, rewardResultList.value);
        var newChallengeBB = BlackboardUtils.FindVariable<Blackboard>(agent, newChallengeInfo.value);
        var progress = BlackboardUtils.FindVariable<int>(agent, challengeProgressCount.value);
        var multiplier = BlackboardUtils.FindVariable<double>(agent, challengeEventMultiplier.value);

        for (int i = 0; i < rewardResultListBB.value.Count; i++)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
    
            var challengeType = BlackboardUtils.FindVariable<ChallengeType>(newChallengeBB.value, "challengeType");
            customData["challenge_type"] = challengeType.value.ToString().ToLower();
    
            customData["action_type"] = "challenge_claim";
    
            BiEventUtils.AppendCommonRewardEventData(customData, rewardResultListBB.value[i]);
    
            customData["challenge_progress"] = progress.value;
    
            int challengeCompleteCount = BlackboardQueryUtils.GetChallengeMinCount(challengeType.value);
            customData["challenge_complete_count"] = challengeCompleteCount;
    
            if (prevChallengeID != null && !string.IsNullOrEmpty(prevChallengeID.value))
            {
                customData["challenge_id"] = prevChallengeID.value;
            }
    
            customData["multiplier"] = multiplier.value;

            BiEventUtils.AppendLevelMultiplierEventData(customData, "coin");
            Analytics.CustomEvent("client_challenge", customData);
        }

        EndAction();
    }
}

}
