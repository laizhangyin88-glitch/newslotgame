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
public class GetPurchaseCoinPotofGoldBBFromProduct : ActionTask<Blackboard> 
{
    public BBParameter<string>  valueA;
    public BBParameter<double>  tierMultiplier;
    public BBParameter<double>  eventMultiplier;
    public BBParameter<long>   totalBaseCredit;
    public BBParameter<long>   totalCredit;
    public BBParameter<long>   rewardPoint;
    public BBParameter<float>  origItemPrice;
    public BBParameter<float>  itemPrice;
    public BBParameter<Blackboard>  coinItemBB;

    protected override string info
    {
        get { return "Get Purchase Coin Pot of gold Item BB"; }
    }

    protected override void OnExecute()
    {
        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);
        coinItemBB.value = BlackboardQueryUtils.GetItemFromProduct(productBB.value, ItemType.CREDIT_POT_OF_GOLD);

        itemPrice.value        = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "price").value);
        origItemPrice.value    = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "originalPrice").value);
        long eventMultiplierNumerator = productBB.value.GetValue<long>("eventMultiplierNumerator");
        eventMultiplier.value  = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);

        var   rp         = BlackboardUtils.FindVariable<long>(coinItemBB.value, "rp");
        var   baseCredit = BlackboardUtils.FindVariable<long>(coinItemBB.value, "baseCredit");

        int tier = TierUtils.GetMeTier();

        tierMultiplier.value   = TierUtils.GetTierMultiplier( tier );
        long baseCoin          = TierUtils.GetTierFractionCoin(baseCredit.value, tier);
        baseCoin               = LevelUtils.GetLevelMultiplierNumeratorValue(baseCoin, "pog");
        totalBaseCredit.value  = baseCoin;
        totalCredit.value      = NumberUtils.GetMultiplierNumeratorValue(totalBaseCredit.value, eventMultiplierNumerator );
        rewardPoint.value      = rp.value;

        EndAction();
    }
}

}
