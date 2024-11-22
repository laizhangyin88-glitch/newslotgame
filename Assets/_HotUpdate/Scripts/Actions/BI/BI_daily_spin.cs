using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_daily_spin : ActionTask<Blackboard>
{

	protected override void OnExecute()
	{
    	var response = BlackboardUtils.FindVariable<Blackboard>(agent, "response").value;
        var spinCount = BlackboardUtils.FindVariable<int>(response, "wheelBonus/spinCount").value;
        var tier = BlackboardUtils.FindVariable<int>(null, "/me/tier").value;
        var earnCredit = response.GetValue<long>("earnCredit");

        var rewardType = BlackboardUtils.FindVariable<DailyBonusWheelRewardType>(agent, "response/wheelBonus/rewardType").value;
        if(rewardType != DailyBonusWheelRewardType.JACKPOT)
        {
            earnCredit = TierUtils.GetTierFractionCoin(earnCredit ,tier);
        }

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["earn_coin"] = response.GetValue<long>("earnCredit");
        customData["remain_wheel_count"] = (spinCount + 1);

        BiEventUtils.AppendLevelMultiplierEventData(customData, "wheel");

        Analytics.CustomEvent("client_purchased_wheel", customData);

		EndAction();
	}

}

}
