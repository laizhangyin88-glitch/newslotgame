using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_daily_bonus : ActionTask
{
	public BBParameter<bool> isJackpot;

    protected override void OnExecute()
    {
    	var friendCoin = BlackboardUtils.FindVariable<long>(null, "/dailyBonusResult/extraBonus/friendBonusCredit").value;
    	var consecutiveCoin = BlackboardUtils.FindVariable<long>(null, "/dailyBonusResult/extraBonus/consecutiveCredit").value;
    	var wheelCoin = BlackboardUtils.FindVariable<long>(null, "/dailyBonusResult/wheelBonus/baseCredit").value;
        var totalCoin = BlackboardUtils.FindVariable<long>(null, "/dailyBonusResult/earnCredit").value;
    	var tier = BlackboardUtils.FindVariable<int>(null, "/me/tier").value;
    	if(!isJackpot.value)
    	{
	    	wheelCoin = TierUtils.GetTierFractionCoin(wheelCoin ,tier);
            wheelCoin = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(wheelCoin, FreebieLevelUtils.FreebieType.DAILY_BONUS_SPIN);
        }

        Dictionary<string, object> customData = new Dictionary<string, object>();
        customData["earn_coin"] = totalCoin;                //total_coin
        customData["wheel_coin"] = wheelCoin;               //wheel_coin
        customData["friend_coin"] = friendCoin;             //friend_coin
        customData["consecutive_coin"] = consecutiveCoin;   //consecutive_coin

        BiEventUtils.AppendLevelMultiplierEventData(customData, "dailySpin");
        Analytics.CustomEvent("client_daily_bonus", customData);

        EndAction();
    }
}

}
