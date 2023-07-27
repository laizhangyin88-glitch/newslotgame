using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Daily Bonus")]
public class GetDailyBonusWheelCoinList : ActionTask<Blackboard>
{
    public BBParameter<bool> useInflationEffect;

    public BBParameter<List<long>>  saveAsCoinList;
    public BBParameter<string>      saveAsWheelCoinTextKey;

    public BBParameter<bool>        saveAsShowInflation;
    public BBParameter<List<long>>  saveAsBaseCoinList;

    public BBParameter<bool>        saveAsIsInflation;
    public BBParameter<double>      saveAsInflationMultiplier;

    protected override string info
    {
        get { return string.Format("Get Daily Bonus Wheel Coin List"); }
    }

    protected override void OnExecute()
    {
        saveAsShowInflation.value = false;
        saveAsIsInflation.value = false;
        saveAsInflationMultiplier.value = 1;

        List<long> coinList = new List<long>();

        Variable<bool> isUsedInflation = null;

        long inflationNumerator = NumberUtils.GetShopInflationNumerator(ShopType.DAILY_BONUS);

        if(inflationNumerator > NumberUtils.GetGlobalDenominator())
        {
            saveAsIsInflation.value = true;
            saveAsInflationMultiplier.value = NumberUtils.GetMultiplierFromNumerator(inflationNumerator);

            if(useInflationEffect.value)
            {
                // if(isMegaWheel.value)
                    isUsedInflation = BlackboardUtils.GetOrCreateVariable<bool>(agent, "isShowMegaInflation");
                // else
                    // isUsedInflation = BlackboardUtils.GetOrCreateVariable<bool>(agent, "isShowNormalInflation");

                saveAsShowInflation.value = !isUsedInflation.value;
                isUsedInflation.value = true;
            }
        }

        coinList = BlackboardUtils.FindVariable<List<long>>(null, "/values/dailyBonus/wheelBaseCreditList").value;
        saveAsWheelCoinTextKey.value = "DAILY_BONUS_WHEEL_COIN";

        saveAsCoinList.value = new List<long>();
        saveAsBaseCoinList.value = new List<long>();

        int meTier = TierUtils.GetMeTier();

        for(int i=0; i<coinList.Count; ++i)
        {
            long coinValue = TierUtils.GetTierFractionCoin(coinList[i], meTier);
            coinValue = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(coinValue, FreebieLevelUtils.FreebieType.DAILY_BONUS_SPIN);

            saveAsCoinList.value.Add(coinValue);

            if(saveAsShowInflation.value)
                saveAsBaseCoinList.value.Add(NumberUtils.GetDevideNumeratorValue(coinValue, inflationNumerator));
        }

        EndAction();
    }
}

}
