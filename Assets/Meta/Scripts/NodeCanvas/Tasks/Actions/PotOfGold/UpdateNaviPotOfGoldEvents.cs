using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Meta/POG")]
public class UpdateNaviPotOfGoldEvents : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> pogProductBB;
    public BBParameter<float> tagChangeDelay;
    public BBParameter<float> tagChangeSpd;
    
    public BBParameter<int> saveAsEventID;

    private bool isInit = false;
    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private ContextElement pogEventTagElement;
    private ContextElement eventLongTimerElement;

    private Blackboard pogbProductBB;
    private bool isPogbEvent;
    private Blackboard pogbShopBB;

    private const string TIME_FORMAT_HHMMSS_TOTALHOUR = "TIME_FORMAT_HHMMSS_TOTALHOUR";
    private const string POPUP_POGB_PURCHASE_JACKPOT_NAME = "POGB_JACKPOT_NAME_{0}";

    protected override void OnExecute()
    {
        InitProperty();

        // POGB Shop Event
        var price = pogProductBB.value.GetValue<double>("price");
        pogbProductBB = BlackboardQueryUtils.GetPriceMatchProduct(price, ShopType.POG_BOOSTER, ItemType.POG_BOOSTER, out pogbShopBB);

        pogEventTagElement.gameObject.SetActive(false);
        eventLongTimerElement.gameObject.SetActive(false);

        EventInfo eventInfo = GetPassiveEventInfo();

        if(eventInfo == null)
        {
            saveAsEventID.value = 0;

            // Check POGB Sale
            if(isPogbEvent)
                SetPOGBSaleEvent();
        }
        else
        {
            saveAsEventID.value = eventInfo.id;

            List<string> eventTagList = new List<string>();

            switch(eventInfo.type)
            {
                case EventInfoType.PIGGY_BANK_SALE:
                    {
                        var productIndex = PassiveEventManager.Instance.GetEventInfoIndex(eventInfo);
                        string eventTag = StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_PERCENT_SALE", BlackboardQueryUtils.GetPotOfGoldSalePercent(productIndex));
                        SetPOGEvent(eventInfo, eventTag);
                    }
                    break;
                case EventInfoType.PIGGY_BANK_MULTIPLY:
                    {
                        if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.PIGGY_BANK) )
                        {
                            eventTagList.Add(StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_ADDITIONAL_PERCENT", PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo)));
                            SetPOGBEvent(eventInfo, eventTagList);
                        }
                        else
                        {
                            string eventTag = StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_MULTIPLIER", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                            SetPOGEvent(eventInfo, eventTag);
                        }
                    }
                    break;
                case EventInfoType.POG_BOOSTER_FREE:
                    {
                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_1"));
                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_2"));
                        SetPOGBEvent(eventInfo, eventTagList);
                    }
                    break;
                case EventInfoType.POG_BOOSTER_MULTIPLY:
                    {
                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_1"));
                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_3", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo)));
                        SetPOGBEvent(eventInfo, eventTagList);
                    }
                    break;
                case EventInfoType.POG_BOOSTER_WITHOUT:
                    {
                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_1"));

                        int eventIndex = PassiveEventManager.Instance.GetEventInfoIndex(eventInfo);
                        string jackpotName = StringTableUtils.GetString(tableType, string.Format(POPUP_POGB_PURCHASE_JACKPOT_NAME, eventIndex - 1));

                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_4", jackpotName));
                        SetPOGBEvent(eventInfo, eventTagList);
                    }
                    break;
                case EventInfoType.POG_BOOSTER_MIN_MULTIPLIER:
                    {
                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_1"));
                        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_5", PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo)));
                        SetPOGBEvent(eventInfo, eventTagList);
                    }
                    break;
            }
        }

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
        agentElement.UpdateContext(false);

        pogEventTagElement = ContextUtils.FindElement(agentElement, "Event Tag", ContextSearchingType.ChildrenSearch);
        eventLongTimerElement = ContextUtils.FindElement(agentElement, "Event Tag Long", ContextSearchingType.ChildrenSearch);

        isInit = true;
    }

    private EventInfo GetPassiveEventInfo()
    {
        EventInfo pogSaleEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_SALE);
        if(pogSaleEventInfo != null) return pogSaleEventInfo;

        EventInfo pogMultiplyEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_MULTIPLY);
        if(pogMultiplyEventInfo != null && !pogMultiplyEventInfo.hideBadge) return pogMultiplyEventInfo;

        EventInfo pogbFreeEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_FREE);
        if(pogbFreeEventInfo != null) return pogbFreeEventInfo;

        // Product Sale Event
        if(BlackboardQueryUtils.GetProductSalePercent(pogbProductBB) > 0 && pogbShopBB != null && pogbShopBB.GetValue<long>("endTimestamp") > 0)
        {
            isPogbEvent = true;
            return null;
        }

        EventInfo pogbAllMultiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_MULTIPLY);
        if(pogbAllMultiplierEventInfo != null) return pogbAllMultiplierEventInfo;

        EventInfo pogbWithoutEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_WITHOUT);
        if(pogbWithoutEventInfo != null) return pogbWithoutEventInfo;

        EventInfo pogbMinMultiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_MIN_MULTIPLIER);
        if(pogbMinMultiplierEventInfo != null) return pogbMinMultiplierEventInfo;

        return null;
    }

    private void SetPOGEvent(EventInfo eventInfo, string tagText)
    {
        pogEventTagElement.gameObject.SetActive(true);
        eventLongTimerElement.gameObject.SetActive(false);

        // MetaContextElementUtils.SimpleSetText(pogEventTagElement, "Text", tagText);
        // MetaContextElementUtils.SetCommonRemainingTimer(pogNormalTimerElement, , 0, TIME_FORMAT_HHMMSS_TOTALHOUR, "", "", "Ended", true, null);

        EventTagController eventController = pogEventTagElement.gameObject.GetComponent<EventTagController>();
        eventController.Initialize( eventInfo.endTimestamp, 
                                    TIME_FORMAT_HHMMSS_TOTALHOUR,
                                    "",
                                    "Ended",
                                    true,
                                    agent.gameObject,
                                    tagText
                                );
    }

    private void SetPOGBEvent(EventInfo eventInfo, List<string> tagList)
    {
        pogEventTagElement.gameObject.SetActive(false);
        eventLongTimerElement.gameObject.SetActive(true);

        // Blackboard eventTagBB = eventLongTimerElement.gameObject.GetComponent<Blackboard>();
        // BlackboardUtils.SetOrCreateValue<List<string>>(eventTagBB, "eventTextList", tagList);
        // BlackboardUtils.SetOrCreateValue<float>(eventTagBB, "changeDelay", tagChangeDelay.value);
        // BlackboardUtils.SetOrCreateValue<float>(eventTagBB, "changeSpd", tagChangeSpd.value);

        // MetaContextElementUtils.SetCommonRemainingTimer(eventLongTimerElement, eventInfo.endTimestamp, 0, TIME_FORMAT_HHMMSS_TOTALHOUR, "", "", "Ended", true, null);

        EventTagController eventController = eventLongTimerElement.gameObject.GetComponent<EventTagController>();
        eventController.Initialize( eventInfo.endTimestamp, 
                                    TIME_FORMAT_HHMMSS_TOTALHOUR,
                                    "",
                                    "Ended",
                                    true,
                                    null,
                                    tagList,
                                    tagChangeDelay.value,
                                    tagChangeSpd.value
                                );
    }

    private void SetPOGBSaleEvent()
    {
        List<string> eventTagList = new List<string>();
        eventTagList.Add(StringTableUtils.GetString(tableType, "NAVI_SHOP_EVENT_POGB_1"));
        eventTagList.Add(StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_PERCENT_SALE", BlackboardQueryUtils.GetProductSalePercent(pogbProductBB)));

        pogEventTagElement.gameObject.SetActive(false);
        eventLongTimerElement.gameObject.SetActive(true);

        // Blackboard eventTagBB = eventLongTimerElement.gameObject.GetComponent<Blackboard>();
        // BlackboardUtils.SetOrCreateValue<List<string>>(eventTagBB, "eventTextList", eventTagList);
        // BlackboardUtils.SetOrCreateValue<float>(eventTagBB, "changeDelay", tagChangeDelay.value);
        // BlackboardUtils.SetOrCreateValue<float>(eventTagBB, "changeSpd", tagChangeSpd.value);

        var eventEndTimestamp = pogbShopBB.GetValue<long>("endTimestamp");

        // MetaContextElementUtils.SetCommonRemainingTimer(eventLongTimerElement, eventEndTimestamp, 0, TIME_FORMAT_HHMMSS_TOTALHOUR, "", "", "Ended", true, agent.gameObject);

        EventTagController eventController = eventLongTimerElement.gameObject.GetComponent<EventTagController>();
        eventController.Initialize( eventEndTimestamp, 
                                    TIME_FORMAT_HHMMSS_TOTALHOUR,
                                    "",
                                    "Ended",
                                    true,
                                    agent.gameObject,
                                    eventTagList,
                                    tagChangeDelay.value,
                                    tagChangeSpd.value
                                );
    }
}

}
