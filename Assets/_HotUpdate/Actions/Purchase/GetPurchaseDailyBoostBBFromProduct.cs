//#define NEW_NET0
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
public class GetPurchaseDailyBoostBBFromProduct : ActionTask<Blackboard>
{
    public BBParameter<string>  valueA;

    [BlackboardOnly]
    public BBParameter<long>        saveTotalCoins;

    [BlackboardOnly]
    public BBParameter<long>        saveTotalGem;
    
    [BlackboardOnly]
    public BBParameter<long>        saveRP;

    [BlackboardOnly]
    public BBParameter<int>         saveTotalDayCount;

    [BlackboardOnly]
    public BBParameter<float>       origPrice;

    [BlackboardOnly]
    public BBParameter<float>       savePrice;

    [BlackboardOnly]
    public BBParameter<Blackboard>  infoBB;

    protected override string info
    {
        get { return "Get DailyBoost Item BB"; }
    }

    protected override void OnExecute()
    {

#if NEW_NET

        Debug.LogWarning("@【这里要插入假数据】 baseGem 对象暴空");
        EndAction();
        return;
#endif


            var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);
        var price = BlackboardUtils.FindVariable<double>(productBB.value, "price");
        origPrice.value = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "originalPrice").value);

        infoBB.value = BlackboardQueryUtils.GetItemFromProduct(productBB.value, ItemType.DAILY_BOOST);

        var baseCoins = BlackboardUtils.FindVariable<long>(infoBB.value, "baseCreditPerDay");
        var baseGem = BlackboardUtils.FindVariable<long>(infoBB.value, "baseGemPerDay");
        var rp = BlackboardUtils.FindVariable<long>(infoBB.value, "rp");
        var totalDayCount = BlackboardUtils.FindVariable<int>(infoBB.value, "totalDayCount");

        int tier = TierUtils.GetMeTier();

        long totalCoins = TierUtils.GetTierFractionCoin(baseCoins.value, tier);
        totalCoins = LevelUtils.GetLevelMultiplierNumeratorValue(totalCoins, "dailyBoost");

        long totalGem = TierUtils.GetTierFractionCoin(baseGem.value, tier);

        saveTotalCoins.value = totalCoins;
        saveTotalGem.value = totalGem;
        saveRP.value = rp.value;
        saveTotalDayCount.value = totalDayCount.value;
        savePrice.value = (float)(price.value);

        EndAction();
    }
}

}
