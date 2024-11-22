using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Challenge")]

public class GetChallengeClaimBB : ActionTask<Transform>
{
    public BBParameter<Blackboard> completeChallengeBB;
    public BBParameter<Blackboard> claimResponseBB;

    public BBParameter<string> titleText;
    public BBParameter<string> rewardText;

    public BBParameter<string> imageIconParentName;

    public BBParameter<string> multiplierText;
    public BBParameter<bool> isEvent;

    protected override string info
    {
        get { return "Get Challenge Claim BB"; }
    }

    protected override void OnExecute()
    {
        var challengeType = BlackboardUtils.FindVariable<ChallengeType>(completeChallengeBB.value, "challengeType");
        
        MetaIconUtils.MakeChallengeImageIcon(challengeType.value, agent, imageIconParentName.value);
        // "Anchor/Layout/Contents Area Full/Contents Layout/Image/Anchor/Image Area");

        bool isError = false;

        var rewardMultiplier = BlackboardUtils.FindVariable<double>(claimResponseBB.value, "multiplier");
        multiplierText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", out isError, rewardMultiplier.value);

        if(rewardMultiplier.value > 1L)
            isEvent.value = true;
        else
            isEvent.value = false;
        
        titleText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_REWARD_TITLE", out isError, challengeType.value.ToString());

        var rewardResultListBB = BlackboardUtils.FindVariable<List<Blackboard>>(claimResponseBB.value, "rewardResultList");
        string eachRewardText = BlackboardQueryUtils.GetChallengeInfoRewardResultText(rewardResultListBB.value, 100L, false);
        rewardText.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_CHALLENGE_REWARD_TEXT", eachRewardText);

        EndAction();
    }
}

}
