using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class InAppMessageClaimVideoAds : ActionTask <Blackboard> 
{
    public BBParameter<string> iamIDValue;
    public BBParameter<long> earnCredit;

    protected override string info
    { 
        get 
        { 
            return string.Format("IAM Video Ads Claim");
        } 
    }
    protected override void OnExecute()
    {
        int iamID = BlackboardUtils.FindVariable<int>(agent, iamIDValue.value).value;
        earnCredit.value = 0;

        BagelCodeClientAPI.IAMVideoAdsCliam(iamID, 
        (response) =>
        {
            if (agent != null)
            {
                ClientAPI2Blackboard.Serialize(agent, response);

                var rewardResult = BlackboardUtils.FindVariable<Blackboard>(agent, "rewardResult");
                if(rewardResult != null)
                {
                    var rewardType = BlackboardUtils.FindVariable<RewardType>(rewardResult.value, "rewardType");
                    switch(rewardType.value)
                    {
                        case RewardType.CREDIT:
                        case RewardType.CREDIT_WITH_MULTIPLIER:
                            {
                                var credit = BlackboardUtils.FindVariable<long>(rewardResult.value, "credit");
                                if(credit != null)
                                    earnCredit.value += credit.value;
                            }
                            break;
                    }
                }
                

                EndAction(true);
            }
        },
        (error) =>
        {
            switch(error.errorCode)
            {
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }
}

}
