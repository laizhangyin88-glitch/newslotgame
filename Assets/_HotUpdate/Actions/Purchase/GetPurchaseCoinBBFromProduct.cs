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
public class GetPurchaseCoinBBFromProduct : ActionTask<Blackboard>
{
    public BBParameter<string> valueA;

    [BlackboardOnly]
    public BBParameter<int> purchaseMultiplierPercent;

    [BlackboardOnly]
    public BBParameter<long> purchaseMultipliedBaseCredit;

    [BlackboardOnly]
    public BBParameter<double> tierMultiplier;

    [BlackboardOnly]
    public BBParameter<double> eventMultiplier;

    [BlackboardOnly]
    public BBParameter<long> totalBaseCredit;

    [BlackboardOnly]
    public BBParameter<long> totalCredit;

    [BlackboardOnly]
    public BBParameter<long> rewardPoint;

    [BlackboardOnly]
    public BBParameter<float> origItemPrice;

    [BlackboardOnly]
    public BBParameter<float> itemPrice;

    [BlackboardOnly]
    public BBParameter<Blackboard> coinItemBB;

    [BlackboardOnly]
    public BBParameter<int> saveAsEventID;

    private bool inited = false;
    private long eventMultiplierNumerator;

    protected override string info
    {
        get { return "Get Purchase Coin Item BB"; }
    }

    protected override void OnExecute()
    {
        var eventType = BlackboardUtils.FindVariable<EventInfoType>(agent, "eventType").value;

        if (inited && eventType != EventInfoType.TIER_UP_SHOP_EVENT_MULTIPLY)
            return;

        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, valueA.value);
        coinItemBB.value = BlackboardQueryUtils.GetItemFromProduct(productBB.value, ItemType.CREDIT);

        itemPrice.value = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "price").value);
        origItemPrice.value = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB.value, "originalPrice").value);

        var rp = BlackboardUtils.FindVariable<long>(coinItemBB.value, "rp");
        var baseCredit = BlackboardUtils.FindVariable<long>(coinItemBB.value, "baseCredit");

        long additionalCreditMultiplierNumerator = BlackboardUtils.FindVariable<long>(coinItemBB.value, "additionalCreditMultiplierNumerator").value;

        purchaseMultiplierPercent.value = System.Convert.ToInt32(additionalCreditMultiplierNumerator / NumberUtils.GetGlobalDenominator() * 100);
        purchaseMultipliedBaseCredit.value = NumberUtils.GetAdditionalMultiplierNumeratorValue(baseCredit.value, additionalCreditMultiplierNumerator);

        int tier = TierUtils.GetMeTier();

        tierMultiplier.value = TierUtils.GetTierMultiplier(tier);
        long baseCoin = TierUtils.GetTierFractionCoin(baseCredit.value, tier);
        baseCoin = LevelUtils.GetLevelMultiplierNumeratorValue(baseCoin, "coin");
        totalBaseCredit.value = NumberUtils.GetAdditionalMultiplierNumeratorValue(baseCoin, additionalCreditMultiplierNumerator);
        rewardPoint.value = rp.value;

        SetTotalCredit();

        EndAction();
    }

    private void SetTotalCredit()
    {
        eventMultiplierNumerator = GetPassiveEventMultiplierNumerator();
        eventMultiplier.value = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);
        totalCredit.value = NumberUtils.GetMultiplierNumeratorValue(totalBaseCredit.value, eventMultiplierNumerator);
    }

    private long GetPassiveEventMultiplierNumerator()
    {
        EventInfo eventInfo = PassiveEventUtils.GetPassiveEvent(ShopType.TIER_UP);
        int eventID = eventInfo == null ? -1 : eventInfo.id;
        bool isEvent = eventInfo != null;

        saveAsEventID.value = eventID;

        if (isEvent)
            return PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);

        return NumberUtils.GetGlobalDenominator();
    }
}
}
