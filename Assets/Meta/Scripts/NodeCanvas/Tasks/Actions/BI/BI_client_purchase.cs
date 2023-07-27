using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_purchase : ActionTask<Blackboard>
{
    public BBParameter<bool> isEvent;

    private object IAMType;
    private object IAMId;
    private object isActionIAM;

    private Blackboard product;
    private Blackboard purchaseResponse;
    private List<Blackboard> itemUseResultList = new List<Blackboard>();
    private ItemType itemType;

    private bool isExist = true;
    private bool isSendPurchaseCount = true;

    protected override void OnExecute()
    {
        isExist = true;
        isSendPurchaseCount = true;

        product = BlackboardUtils.FindVariable<Blackboard>(agent, "product").value;
        purchaseResponse = BlackboardUtils.FindVariable<Blackboard>(null, "/purchaseResponse").value;
        itemUseResultList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/purchaseResponse/itemUseResultList").value;
        itemType = itemUseResultList[0].GetValue<ItemType>("itemType");

        BiEventUtils.GetIAMData(agent, out IAMId, out IAMType, out isActionIAM);

        Dictionary<string, object> customData = new Dictionary<string, object>();

        switch(itemType)
        {
            case ItemType.CREDIT:
                AddCoinEventData(customData);
                break;
            case ItemType.DAILY_BOOST:
                AddDailyBoostEventData(customData);
                BISendDailyBoost();
                break;
            // case ItemType.TIER_BOOST:
            //     break;
            case ItemType.CREDIT_POT_OF_GOLD:
                AddCoinPOGEventData(customData);
                break;
            case ItemType.CREDIT_MULTIPLIER_WHEEL:
                AddCoinMultiplierWheelEventData(customData);
                break;
            case ItemType.DAILY_BONUS_WHEEL:
                AddDailySpinEventData(customData);
                break;
            case ItemType.PIGGY_BANK:
                AddPOGEventData(customData);
                break;
            case ItemType.EARLY_ACCESS:
                AddEarlyAccessEventData(customData);
                break;
            case ItemType.CREDIT_WHEEL:
                AddCoinWheelEventData(customData);
                break;
            case ItemType.TICKETED_BONUS_TICKET:
                AddTicketedBonusEventData(customData);
                break;
            case ItemType.DAILY_MEGA_WHEEL:
                AddMegaWheelEventData(customData);
                break;
            case ItemType.POG_BOOSTER:
                AdPOGBEventData(customData);
                break;
            case ItemType.GEM:
                AddGemEventData(customData);
                break;
            case ItemType.GEM_BOOSTER:
                AddGemMultiplierWheelEventData(customData);
                break;
            case ItemType.EPIC_PASS:
                AddEpicPassEventData(customData);
                break;
            case ItemType.SPIN_BOOST:
                AddSpinBoosterEventData(customData);
                break;
            case ItemType.TICKETED_BONUS_BOOSTER:
                AddTicketedBonusBoosterEventData(customData);
                break;
            default:
                isExist = false;
                break;
        }

        if(isExist)
        {
            customData["shop_event_flag"] = isEvent == null ? false : isEvent.value;
            var lmTypeValue = BlackboardUtils.FindVariable<string>(agent, "_levelMultiplierType");
            BiEventUtils.AppendLevelMultiplierEventData(customData, lmTypeValue == null ? BiEventUtils.GetLevelMultiplierFromItemType(product) : lmTypeValue.value);

            Analytics.CustomEvent("client_purchase", customData);
        }

        ThirdPartyAnalyticsManager.SendPurchaseEvent(product, purchaseResponse);

        if(isSendPurchaseCount && purchaseResponse.GetValue<int>("purchaseCount") == 1)
        {
            AdjustManager.Instance.SendEvent("first_purchase");
        }

        EndAction();
    }

    private void AddCoinEventData(Dictionary<string, object> customData)
    {
        long eventMultiplierNumerator = itemUseResultList[0].GetValue<long>("eventMultiplierNumerator");
        double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);

        if(eventMultiplier > 1)
            customData["event_multiplier"] = eventMultiplier;

        customData["type"] = "coin";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_coin"] = itemUseResultList[0].GetValue<long>("earnCredit");
    }

    private void AddGemEventData(Dictionary<string, object> customData)
    {
        long eventMultiplierNumerator = itemUseResultList[0].GetValue<long>("eventMultiplierNumerator");
        double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);

        if(eventMultiplier > 1)
            customData["event_multiplier"] = eventMultiplier;

        customData["type"] = "gem";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_gem"] = itemUseResultList[0].GetValue<long>("earnGem");
    }

    private void AddDailyBoostEventData(Dictionary<string, object> customData)
    {
        var dailyBoost = BlackboardUtils.FindVariable<Blackboard>(agent, "/dailyBoost").value;
        customData["type"] = "daily_boost";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["coin_per_day"] = dailyBoost.GetValue<long>("credit");
        customData["gem_per_day"] = dailyBoost.GetValue<long>("gem");
        customData["total_day_count"] = dailyBoost.GetValue<int>("totalCount");
    }

    private void AddDailySpinEventData(Dictionary<string, object> customData)
    {
        var total_wheel_count = itemUseResultList[0].GetValue<int>("totalSpinCount") - itemUseResultList[0].GetValue<int>("addedSpinCount");

        customData["type"] = "wheel";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["added_wheel_count"] = itemUseResultList[0].GetValue<int>("addedSpinCount");
        customData["total_wheel_count"] = total_wheel_count;
    }

    private void AddPOGEventData(Dictionary<string, object> customData)
    {
        var infoList = BlackboardUtils.FindVariable<List<Blackboard>>(product, "itemList").value;

        var tier = BlackboardUtils.FindVariable<int>(agent, "/me/tier").value;
        var real_max_coin = TierUtils.GetTierFractionCoin(infoList[0].GetValue<long>("maxCredit"), tier);
        real_max_coin = LevelUtils.GetLevelMultiplierNumeratorValue(real_max_coin, "pog");

        var eventMultiplier = BlackboardUtils.FindVariable<double>( itemUseResultList[0], "multiplier");

        var piggyMultiplierEvent = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "BI_POG_MULTIPLIER_EVENT_ID");
        var piggySaleEvent = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "BI_POG_SALE_EVENT_ID");

        bool isMultiplierPersonalEvent = false;
        bool isSalePersonalEvent = false;
        var PersonalPassiveEventIDOffset = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "values/misc/PERSONAL_PASSIVE_EVENT_ID_OFFSET");

        if(piggyMultiplierEvent != null && piggyMultiplierEvent.value >= PersonalPassiveEventIDOffset.value)
            isMultiplierPersonalEvent = true;

        if(piggySaleEvent != null && piggySaleEvent.value >= PersonalPassiveEventIDOffset.value)
            isSalePersonalEvent = true;

        customData["type"] = "pot_of_gold";

        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, isMultiplierPersonalEvent, isSalePersonalEvent);
        customData["purchased_coin"] = itemUseResultList[0].GetValue<long>("earnCredit");
        customData["max_coin"] = System.Convert.ToInt64(real_max_coin);

        if(eventMultiplier != null && eventMultiplier.value > 1)
            customData["event_multiplier"] = eventMultiplier.value;
    }

    private void AddCoinPOGEventData(Dictionary<string, object> customData)
    {
        var infoList = BlackboardUtils.FindVariable<List<Blackboard>>(product, "itemList").value;

        customData["type"] = "coin_pot_of_gold";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_coin"] = itemUseResultList[0].GetValue<long>("earnCredit");
    }

    private void AddCoinMultiplierWheelEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "coin_booster";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_coin"] = itemUseResultList[0].GetValue<long>("earnCredit");
        customData["total_coin"] = itemUseResultList[0].GetValue<long>("earnCredit") + itemUseResultList[0].GetValue<long>("origEarnCredit");
        customData["multiplier"] = itemUseResultList[0].GetValue<double>("multiplier");
    }

    private void AddGemMultiplierWheelEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "gem_booster";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_gem"] = itemUseResultList[0].GetValue<long>("earnGem");
        customData["total_gem"] = itemUseResultList[0].GetValue<long>("earnGem") + itemUseResultList[0].GetValue<long>("origEarnGem");
        customData["multiplier"] = itemUseResultList[0].GetValue<double>("multiplier");
    }

    private void AddEpicPassEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "epic_pass";
    }

    private void AddEarlyAccessEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "early_access";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);

        var offerFreeTrial = BlackboardUtils.FindVariable<bool>(product, "offerFreeTrial").value;

        if(offerFreeTrial)
            isSendPurchaseCount = false;
    }

    private void AddCoinWheelEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "coin_wheel";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_coin"] = itemUseResultList[0].GetValue<long>("earnCredit");
    }

    private void AddTicketedBonusEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "buy_bonus";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
    }

    private void AddMegaWheelEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "mega_wheel";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
    }

    private void AdPOGBEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "pot_of_gold_booster";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_coin"] = itemUseResultList[0].GetValue<long>("earnCredit");
        customData["total_coin"] = itemUseResultList[0].GetValue<long>("earnCredit") + itemUseResultList[0].GetValue<long>("origEarnCredit");
        customData["multiplier"] = itemUseResultList[0].GetValue<double>("multiplier");
    }

    private void BISendDailyBoost()
    {
        var dailyBoost = BlackboardUtils.FindVariable<Blackboard>(agent, "/dailyBoost").value;
        var tier = BlackboardUtils.FindVariable<int>(agent, "/me/tier").value;

        var earn_coin = TierUtils.GetTierFractionCoin(dailyBoost.GetValue<long>("credit"), tier);
        earn_coin = LevelUtils.GetLevelMultiplierNumeratorValue(earn_coin, "dailyBoost");
        var earn_gem = TierUtils.GetTierFractionCoin(dailyBoost.GetValue<long>("gem"), tier);

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["earn_coin"] = System.Convert.ToInt64(earn_coin);//coin
        customData["earn_gem"] = System.Convert.ToInt64(earn_gem);//gem
        customData["collected_count"] = dailyBoost.GetValue<int>("collectCount");//collected_count
        customData["nth_day"] = dailyBoost.GetValue<int>("totalCount") - dailyBoost.GetValue<int>("leftCollectDayCount");//nth_day
        customData["total_day_count"] = dailyBoost.GetValue<int>("totalCount");//total_day_count

        BiEventUtils.AppendLevelMultiplierEventData(customData, "dailyBoost");

        Analytics.CustomEvent("client_daily_boost", customData);
    }

    private void AddSpinBoosterEventData(Dictionary<string, object> customData)
    {
        customData["type"] = "spin_deal_booster";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_coin"] = itemUseResultList[0].GetValue<long>("earnCredit");
        customData["total_coin"] = itemUseResultList[0].GetValue<long>("earnCredit") + itemUseResultList[0].GetValue<long>("origEarnCredit");
        customData["multiplier"] = itemUseResultList[0].GetValue<double>("multiplier");
    }

    private void AddTicketedBonusBoosterEventData(Dictionary<string, object> customData)
    {
        long earnCredit = itemUseResultList[0].GetValue<long>("earnCredit");
        long totalEarnCredit = earnCredit + itemUseResultList[0].GetValue<long>("origEarnCredit");

        customData["type"] = "ticketed_bonus_booster";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        customData["purchased_coin"] = earnCredit;
        customData["total_coin"] = totalEarnCredit;
        customData["multiplier"] = NumberUtils.GetMultiplierFromNumerator(itemUseResultList[0].GetValue<long>("multiplierNumerator"));
        }
    }

}
