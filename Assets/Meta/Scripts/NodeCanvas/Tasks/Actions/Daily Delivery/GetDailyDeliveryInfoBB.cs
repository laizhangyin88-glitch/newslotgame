using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Delivery")]

public class GetDailyDeliveryInfoBB : ActionTask<Blackboard>
{
    public BBParameter<Blackboard> dailyDeliveryBB;

    public BBParameter<List<Blackboard>> saveAsRewardList;
    public BBParameter<List<int>> claimedRewardIdxList;
    public BBParameter<int> nextClaimRewardIdx;

    protected override void OnExecute()
    {
        saveAsRewardList.value = dailyDeliveryBB.value.GetValue<List<Blackboard>>("rewardList");
        claimedRewardIdxList.value = dailyDeliveryBB.value.GetValue<List<int>>("claimedRewardIdxList");
        nextClaimRewardIdx.value = dailyDeliveryBB.value.GetValue<int>("nextClaimRewardIdx");

        EndAction();
    }
}

}
