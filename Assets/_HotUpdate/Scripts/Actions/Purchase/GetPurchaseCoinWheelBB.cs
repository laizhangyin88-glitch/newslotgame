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
public class GetPurchaseCoinWheelBB : ActionTask<Blackboard> 
{
    public BBParameter<string>  valueA;

    [BlackboardOnly]
    public BBParameter<float>  origItemPrice;

    [BlackboardOnly]
    public BBParameter<float>  itemPrice;

    [BlackboardOnly]
    public BBParameter<List<long>>  wheelCoinList;

    [BlackboardOnly]
    public BBParameter<long> initCoins;

    [BlackboardOnly]
    public BBParameter<long> minCoins;

    [BlackboardOnly]
    public BBParameter<long> maxCoins;

    [BlackboardOnly]
    public BBParameter<long>  rewardPoint;

    [BlackboardOnly]
    public BBParameter<Blackboard>  coinWheelItemBB;

    protected override string info
    {
        get { return "Get Purchase Coin Wheel Item BB"; }
    }

    protected override void OnExecute()
    {
        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);
        coinWheelItemBB.value = BlackboardQueryUtils.GetItemFromProduct(productBB.value, ItemType.CREDIT_WHEEL);

        itemPrice.value        = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "price").value);
        origItemPrice.value    = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "originalPrice").value);

        var   rp         = BlackboardUtils.FindVariable<long>(coinWheelItemBB.value, "rp");
        var   settingList = BlackboardUtils.FindVariable<List<Blackboard>>(coinWheelItemBB.value, "setting");

        int tier = TierUtils.GetMeTier();

        wheelCoinList.value = new List<long>();

        minCoins.value = System.Int64.MaxValue;
        maxCoins.value = 0;

        for(int i=0; i<settingList.value.Count; ++i)
        {
            var wheelCoins = BlackboardUtils.FindVariable<long>(settingList.value[i], "credit");

            long currentCoins = TierUtils.GetTierFractionCoin(wheelCoins.value, tier);
            currentCoins = LevelUtils.GetLevelMultiplierNumeratorValue(currentCoins, "coin");
            wheelCoinList.value.Add( currentCoins );

            if(minCoins.value > currentCoins)
                minCoins.value = currentCoins;

            if(maxCoins.value < currentCoins)
                maxCoins.value = currentCoins;
        }

        rewardPoint.value = rp.value;

        EndAction();
    }
}

}
