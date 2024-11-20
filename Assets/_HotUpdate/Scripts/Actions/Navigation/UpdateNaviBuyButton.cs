using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Meta/Navigation")]
public class UpdateNaviBuyButton : ActionTask<Blackboard> 
{
    public BBParameter<string> saveAsEventText0;
    public BBParameter<string> saveAsEventText1;
    public BBParameter<long> saveAsEventEndTimestamp;
    public BBParameter<bool> saveAsIsEvent;
    public BBParameter<int>  saveAsViewPassiveEventID;
    public BBParameter<long> saveAsNextEventStartTimestamp;
    public BBParameter<long> saveAsNextEventEndTimestamp;

    private bool isInit = false;
    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private Animator rootAnimator;

    protected override void OnExecute()
    {
        InitProperty();
        UpdateVariables();

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();

        // sctionInfoTextElement = ContextUtils.FindElement(agentElement, "Piggy Bank Information/Text", ContextSearchingType.FullNameSearch);
        // fullCoinTextElement = ContextUtils.FindElement(agentElement, "Text Full Amount", ContextSearchingType.ChildrenSearch);

        rootAnimator = agent.gameObject.GetComponent<Animator>();

        isInit = true;
    }

    private void UpdateVariables()
    {
        bool isDailyBoostCollectable = false;
        bool isVipBonusCollectable = false;

        var dailyBoostBB = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "dailyBoost");
        if(dailyBoostBB != null)
        {
            isDailyBoostCollectable = dailyBoostBB.value != null ? dailyBoostBB.value.GetValue<bool>("isCollectable") : false;
        }

        if(BagelCode.AccountUtils.HasAccount())
        {
            long vipDailyBonusTimestamp = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/vipDailybonusTimestamp").value;
            int timeZoneOffset = BagelCode.TimeUtils.GetTimeZoneOffset();
            var vipDailyBonusCooltime = BlackboardUtils.FindVariable<long>( MainBlackboard.Get(), "vipDailyBonusCooltime");
            vipDailyBonusTimestamp += vipDailyBonusCooltime.value;

            long currentTime = TimeUtils.GetTimeStamp();
            if(currentTime >= vipDailyBonusTimestamp)
                isVipBonusCollectable = true;
        }

        rootAnimator.SetBool("IsCoinReward", isDailyBoostCollectable || isVipBonusCollectable);

        var gemBoosterEventShopBB = BlackboardQueryUtils.GetShopBB(ShopType.GEM_BOOSTER);

        var coinBoosterEventShopBB = BlackboardQueryUtils.GetShopBB(ShopType.COIN_BOOSTER);

        saveAsIsEvent.value = true;
        saveAsNextEventStartTimestamp.value = 0L;
        saveAsNextEventEndTimestamp.value = 0L;
        saveAsViewPassiveEventID.value = 0;

        var eventInfo = GetBuyButtonPriorityPassiveEvent();

        if(eventInfo != null && eventInfo.type == EventInfoType.FREE_COIN_BOOSTER)
        {
            saveAsEventEndTimestamp.value = eventInfo.endTimestamp;
            saveAsViewPassiveEventID.value = eventInfo.id;
            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_COIN_BOOSTER_0");
            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_COIN_BOOSTER_1");
        }
        else if(eventInfo != null && eventInfo.type == EventInfoType.FREE_GEM_BOOSTER)
        {
            saveAsEventEndTimestamp.value = eventInfo.endTimestamp;
            saveAsViewPassiveEventID.value = eventInfo.id;
            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_GEM_BOOSTER_0");
            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_GEM_BOOSTER_1");
        }
        else if(eventInfo != null && eventInfo.type == EventInfoType.COIN_SHOP_EVENT_MULTIPLY )
        {
            saveAsEventEndTimestamp.value = eventInfo.endTimestamp;
            saveAsViewPassiveEventID.value = eventInfo.id;
            var eventMultiplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);

            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_0");

            if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.COIN) )
            {
                long viewAddPercent = PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo);
                saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_ADDITIONAL_PERCENT", viewAddPercent);
            }
            else
            {
                saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_1", eventMultiplier);
            }
        }
        else if(eventInfo != null && eventInfo.type == EventInfoType.GEM_SHOP_EVENT_MULTIPLY )
        {
            saveAsEventEndTimestamp.value = eventInfo.endTimestamp;
            saveAsViewPassiveEventID.value = eventInfo.id;
            var eventMultiplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);

            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_0");

            if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.GEM) )
            {
                long viewAddPercent = PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo);
                saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_ADDITIONAL_PERCENT", viewAddPercent);
            }
            else
            {
                saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_GEM_1", eventMultiplier);
            }
        }
        else if(    coinBoosterEventShopBB != null
                 && coinBoosterEventShopBB.GetValue<long>("endTimestamp") > 0
                 && BlackboardQueryUtils.GetFirstProductSalePercent(coinBoosterEventShopBB) > 0)
        {
            saveAsEventEndTimestamp.value = coinBoosterEventShopBB.GetValue<long>("endTimestamp");
            var salePercent = BlackboardQueryUtils.GetFirstProductSalePercent(coinBoosterEventShopBB);

            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_COIN_BOOSTER_SALE_0");
            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_COIN_BOOSTER_SALE_1", salePercent);
        }
        else if(    gemBoosterEventShopBB != null
                 && gemBoosterEventShopBB.GetValue<long>("endTimestamp") > 0
                 && BlackboardQueryUtils.GetFirstProductSalePercent(gemBoosterEventShopBB) > 0)
        {
            saveAsEventEndTimestamp.value = gemBoosterEventShopBB.GetValue<long>("endTimestamp");
            var salePercent = BlackboardQueryUtils.GetFirstProductSalePercent(gemBoosterEventShopBB);

            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_GEM_BOOSTER_SALE_0");
            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_GEM_BOOSTER_SALE_1", salePercent);
        }
        else
        {
            if(eventInfo != null)
            {
                saveAsEventEndTimestamp.value = eventInfo.endTimestamp;
                saveAsViewPassiveEventID.value = eventInfo.id;

                switch(eventInfo.type)
                {
                    // case EventInfoType.FREE_COIN_BOOSTER:
                    //     {
                    //         saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_COIN_BOOSTER_0");
                    //         saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_COIN_BOOSTER_1");
                    //     }
                    //     break;
                    case EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY:
                        {
                            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_ALL_MULTIPLIER_WHEEL_0", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_ALL_MULTIPLIER_WHEEL_1");
                        }
                        break;
                    case EventInfoType.CREDIT_MULTIPLIER_WHEEL:
                        {
                            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_WHEEL_0", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_WHEEL_1");
                        }
                        break;
                    case EventInfoType.GEM_BOOSTER_MULTIPLY:
                        {
                            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_ALL_MULTIPLIER_WHEEL_GEM_0", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_ALL_MULTIPLIER_WHEEL_GEM_1");
                        }
                        break;
                    case EventInfoType.GEM_BOOSTER:
                        {
                            saveAsEventText0.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_WHEEL_GEM_0", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                            saveAsEventText1.value = StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_MULTIPLIER_WHEEL_GEM_1");
                        }
                        break;
                    }
            }
            else
            {
                saveAsEventEndTimestamp.value = 0L;
                saveAsEventText0.value = "";
                saveAsEventText1.value = "";
                saveAsIsEvent.value = false;
                saveAsViewPassiveEventID.value = 0;
            }
        }

        // var coinReserveEventShopBB = BlackboardQueryUtils.GetReserveEventShopBB(ShopType.COIN);
        // var gemReserveEventShopBB = BlackboardQueryUtils.GetReserveEventShopBB(ShopType.GEM);
        // var coinBoosterReserveEventShopBB = BlackboardQueryUtils.GetReserveEventShopBB(ShopType.COIN_BOOSTER);

        // if(coinReserveEventShopBB != null)
        // {
        //     saveAsNextEventStartTimestamp.value = coinReserveEventShopBB.GetValue<long>("startTimestamp");
        //     saveAsNextEventEndTimestamp.value = coinReserveEventShopBB.GetValue<long>("endTimestamp");
        // }
        // else if(gemReserveEventShopBB != null)
        // {
        //     saveAsNextEventStartTimestamp.value = gemReserveEventShopBB.GetValue<long>("startTimestamp");
        //     saveAsNextEventEndTimestamp.value = gemReserveEventShopBB.GetValue<long>("endTimestamp");
        // }
        // else if(coinBoosterReserveEventShopBB != null)
        // {
        //     saveAsNextEventStartTimestamp.value = coinBoosterReserveEventShopBB.GetValue<long>("startTimestamp");
        //     saveAsNextEventEndTimestamp.value = coinBoosterReserveEventShopBB.GetValue<long>("endTimestamp");
        // }

        rootAnimator.SetBool("IsEvent", saveAsIsEvent.value);
    }

    private EventInfo GetBuyButtonPriorityPassiveEvent()
    {
        EventInfo eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.FREE_COIN_BOOSTER);
        if(eventInfo != null) return eventInfo;

        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.FREE_GEM_BOOSTER);
        if(eventInfo != null) return eventInfo;

        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.COIN_SHOP_EVENT_MULTIPLY);
        if(eventInfo != null && !eventInfo.hideBadge) return eventInfo;

        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_SHOP_EVENT_MULTIPLY);
        if(eventInfo != null && !eventInfo.hideBadge) return eventInfo;

        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL_MULTIPLY);
        if(eventInfo != null) return eventInfo;

        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CREDIT_MULTIPLIER_WHEEL);
        if(eventInfo != null) return eventInfo;

        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BOOSTER_MULTIPLY);
        if (eventInfo != null) return eventInfo;

        eventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.GEM_BOOSTER);
        if (eventInfo != null) return eventInfo;

        return null;
    }
}

}
