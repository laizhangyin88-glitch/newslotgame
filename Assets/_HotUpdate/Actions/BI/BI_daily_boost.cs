using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_daily_boost : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
    	var dailyBoost = BlackboardUtils.FindVariable<Blackboard>(agent, "/dailyBoost").value;
        var tier = BlackboardUtils.FindVariable<int>(agent, "/me/tier").value;

        var earn_coin = TierUtils.GetTierFractionCoin(dailyBoost.GetValue<long>("credit"), tier);
        earn_coin = LevelUtils.GetLevelMultiplierDailyBoostCoinNumeratorValue(earn_coin, dailyBoost, "dailyBoost");
        var earn_gem = TierUtils.GetTierFractionCoin(dailyBoost.GetValue<long>("gem"), tier);
            
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["earn_coin"] = System.Convert.ToInt64(earn_coin);//coin
        customData["earn_gem"] = System.Convert.ToInt64(earn_gem);//gem
        customData["collected_count"] = dailyBoost.GetValue<int>("collectCount");//collected_count
        customData["nth_day"] = dailyBoost.GetValue<int>("totalCount") - dailyBoost.GetValue<int>("leftCollectDayCount");//nth_day
        customData["total_day_count"] = dailyBoost.GetValue<int>("totalCount");//total_day_count

        BiEventUtils.AppendLevelMultiplierEventData(customData, "dailyBoost");

        Analytics.CustomEvent("client_daily_boost", customData);

        EndAction();
    }
}

}
