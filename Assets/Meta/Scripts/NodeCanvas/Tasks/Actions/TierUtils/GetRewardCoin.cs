using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Purchase")]
public class GetRewardCoin : ActionTask<Blackboard> 
{
    public BBParameter<string>  coinValue;
    public BBParameter<string>  multiplierValue;
    public BBParameter<string>  typeValue;

    public BBParameter<long> saveValue;

    protected override string info
    {
        get { return string.Format("{0} = TierUtils.GetRewardCoin({1}, {2})", saveValue, coinValue, typeValue); }
    }

    protected override void OnExecute()
    {
        var coin = BlackboardUtils.FindVariable<long>(agent, coinValue.value);
        var multiplier = BlackboardUtils.FindVariable<double>(agent, multiplierValue.value);
        long baseCoin = TierUtils.GetRewardCoin(coin.value, multiplier.value);
        saveValue.value = LevelUtils.GetLevelMultiplierNumeratorValue(baseCoin, typeValue.value);

        EndAction();
    }
}

}
