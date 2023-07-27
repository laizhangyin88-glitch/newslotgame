using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bonus")]

public class GetDailyBonusResultInfo : ActionTask<Blackboard>
{
    public BBParameter<string> eventMultiplierNumerator;

    public BBParameter<string> isJackpot;

	[BlackboardOnly]
    public BBParameter<long> baseCredit;
	[BlackboardOnly]
    public BBParameter<double> tierMultiplier;
	[BlackboardOnly]
    public BBParameter<long> wheelBonusTotalCredit;
	[BlackboardOnly]
    public BBParameter<long> returnBonusTotalCredit;
	[BlackboardOnly]
    public BBParameter<int> friendCount;
	[BlackboardOnly]
    public BBParameter<long> friendBonusCredit;
	[BlackboardOnly]
    public BBParameter<int> friendMaxCount;
	[BlackboardOnly]
    public BBParameter<long> friendBonusTotalCredit;
	[BlackboardOnly]
    public BBParameter<long> totalEarnCredit;


    protected override string info
    {
        get { return string.Format("Get Daily Bingo Result Info"); }
    }

    protected override void OnExecute()
    {
    	var tier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/me/tier");
    	var realBaseCredit = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "/dailyBonusResult/wheelBonus/baseCredit");
    	var _eventMultiplierNumerator = BlackboardUtils.FindVariable<long>(agent, eventMultiplierNumerator.value);
        var _isJackpot = BlackboardUtils.FindVariable<bool>(agent, isJackpot.value);

        if(_isJackpot.value)
        {
            realBaseCredit.value = NumberUtils.GetMultiplierNumeratorValue(realBaseCredit.value, _eventMultiplierNumerator.value);
            realBaseCredit.value = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(realBaseCredit.value, FreebieLevelUtils.FreebieType.DAILY_BONUS_SPIN);
            wheelBonusTotalCredit.value = baseCredit.value = realBaseCredit.value;
            tierMultiplier.value = TierUtils.GetTierMultiplier(tier.value);
        }
        else
        {
            tierMultiplier.value = TierUtils.GetTierMultiplier(tier.value);
            baseCredit.value = NumberUtils.GetMultiplierNumeratorValue(realBaseCredit.value, _eventMultiplierNumerator.value);
            baseCredit.value = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(baseCredit.value, FreebieLevelUtils.FreebieType.DAILY_BONUS_SPIN);
            wheelBonusTotalCredit.value = TierUtils.GetTierFractionCoin(baseCredit.value, tier.value);
        }

		returnBonusTotalCredit.value = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "/dailyBonusResult/extraBonus/consecutiveCredit").value;

		friendCount.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/dailyBonusResult/extraBonus/friendCount").value;
		friendBonusCredit.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/dailyBonus/friendBonusCredit").value;
		friendMaxCount.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/dailyBonus/friendBonusMaxCount").value;
		friendBonusTotalCredit.value = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "/dailyBonusResult/extraBonus/friendBonusCredit").value;

		totalEarnCredit.value = wheelBonusTotalCredit.value + returnBonusTotalCredit.value + friendBonusTotalCredit.value;

        EndAction();
    }
}

}
