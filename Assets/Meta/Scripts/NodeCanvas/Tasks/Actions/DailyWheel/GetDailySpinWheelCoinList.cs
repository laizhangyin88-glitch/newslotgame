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
public class GetDailySpinWheelCoinList : ActionTask<Blackboard>
{
    public BBParameter<bool> useInflationEffect;
    public BBParameter<bool> isMegaWheel;

    public BBParameter<List<long>>  saveAsCoinList;
    public BBParameter<string>      saveAsWheelCoinTextKey;

    public BBParameter<bool>        saveAsShowInflation;
    public BBParameter<List<long>>  saveAsBaseCoinList;

    public BBParameter<bool>        saveAsIsInflation;
    public BBParameter<double>      saveAsInflationMultiplier;
    // apply freebie level multiplier - true : freebie level multiplier / false : shop level multiplier
    public BBParameter<bool> isFreebie;

    protected override string info
    {
        get { return string.Format("Get Daily Spin Wheel Coin List"); }
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

        if(isMegaWheel.value)
        {
            coinList = BlackboardUtils.FindVariable<List<long>>(null, "/values/dailyMegaWheel/wheelBaseCreditList").value;
            saveAsWheelCoinTextKey.value = "DAILY_BONUS_MEGA_WHEEL_COIN";
        }
        else
        {
            coinList = BlackboardUtils.FindVariable<List<long>>(null, "/values/dailySpin/purchasedWheelBaseCreditList").value;
            saveAsWheelCoinTextKey.value = "DAILY_BONUS_WHEEL_COIN";
        }

        saveAsCoinList.value = new List<long>();
        saveAsBaseCoinList.value = new List<long>();

        int meTier = TierUtils.GetMeTier();

        for(int i=0; i<coinList.Count; ++i)
        {
            long coinValue = TierUtils.GetTierFractionCoin(coinList[i], meTier);
            if (isFreebie.value == true)
                coinValue = FreebieLevelUtils.GetLevelMultiplierNumeratorValue(coinValue, FreebieLevelUtils.FreebieType.DAILY_BONUS_SPIN);
            else
                coinValue = LevelUtils.GetLevelMultiplierNumeratorValue(coinValue, "wheel");
            saveAsCoinList.value.Add(coinValue);

            if(saveAsShowInflation.value)
                saveAsBaseCoinList.value.Add(NumberUtils.GetDevideNumeratorValue(coinValue, inflationNumerator));
        }

        EndAction();
    }
}

}
