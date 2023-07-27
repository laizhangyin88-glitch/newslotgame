using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Meta/POG")]
public class UpdatePOGBoosterPurchaseValues : ActionTask<Blackboard> 
{
    public BBParameter<int>  saveAsTargetPurhcaseID;
    public BBParameter<Blackboard> saveAsProductBB;
    public BBParameter<int>  saveAsBoosterPassiveEventID;
    public BBParameter<long> saveAsEventEndtimestamp;

    private bool isInit = false;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private List<ContextElement> jackpotElementList;
    private List<Animator> jackpotAnimator;

    private ContextElement purchasedTextElement;
    private ContextElement boostMultiplierTextElement;
    private ContextElement coinTextElement;

    private ContextElement closeButtonElement;
    private ContextElement purchaseButtonElement;
    private ContextElement freeButtonElement;
    private ContextElement rpTextElement;

    private ContextElement eventTextElement;
    private ContextElement eventTimerElement;
    private ContextElement pogbEventIconAreaElement;

    private ContextElement saleTagElement;
    private ContextElement saleTagTextElement;

    private Animator rootAnimator;

    private Blackboard itemBB;
    private List<double> jackpotViewMultiplierList;
    private List<long> jackpotMultiplierNumeratorList;
    private long prevEarnCoins;
    private long rp;
    private double price;
    private double originalPrice;
    private bool isSale;

    private GameObject eventIconObject;

    private const string JACKPOT_TEXT_FORMAT_KEY = "TEXT_POG_BOOSTER_JACKPOT_MULTIPLIER";
    private const string POPUP_POGB_JACKPOT_ORIG_MULTIPLIER = "POPUP_POGB_PURCHASE_JACKPOT_ORIG_MULTIPLIER";
    private const string POPUP_POGB_PURHCASE_COIN_TEXT = "POPUP_POGB_PURHCASE_COIN_TEXT";
    private const string POPUP_POGB_PURHCASE_MULTIPLIER_TEXT = "POPUP_POGB_PURHCASE_MULTIPLIER_TEXT";
    private const string POPUP_POGB_PURCHASE_TOTAL_COIN_TEXT = "POPUP_POGB_PURCHASE_TOTAL_COIN_TEXT";
    private const string POPUP_POGB_PURCHASE_BUY_BUTTON_TEXT = "POPUP_POGB_PURCHASE_BUY_BUTTON_TEXT";
    private const string POPUP_POGB_PURCHASE_BUY_WITH_SALE_BUTTON_TEXT = "POPUP_POGB_PURCHASE_BUY_WITH_SALE_BUTTON_TEXT";
    private const string POPUP_POGB_PURCHASE_OK_BUTTON_TEXT = "POPUP_POGB_PURCHASE_OK_BUTTON_TEXT";
    private const string POPUP_POGB_PURCHASE_RP_TEXT = "POPUP_POGB_PURCHASE_RP_TEXT";
    private const string POPUP_POGB_PURCHASE_WAS_PRICE_TEXT = "POPUP_POGB_PURCHASE_WAS_PRICE_TEXT";
    private const string POPUP_POGB_PURCHASE_TOP_DESC_ALL_EVENT = "POPUP_POGB_PURCHASE_TOP_DESC_ALL_EVENT";
    private const string POPUP_POGB_PURCHASE_TOP_DESC_MIN_EVENT = "POPUP_POGB_PURCHASE_TOP_DESC_MIN_EVENT";
    private const string POPUP_POGB_PURCHASE_CENTER_DESC_WITH_OUT_EVENT = "POPUP_POGB_PURCHASE_CENTER_DESC_WITH_OUT_EVENT";
    private const string POPUP_POGB_PURCHASE_JACKPOT_NAME = "POGB_JACKPOT_NAME_{0}";

    private const string EVENT_CLOSE_BUTTON = "OnClose";
    private const string EVENT_PURCHASE_BUTTON = "OnPurchase";
    private const string EVENT_REDEEM_BUTTON = "OnRedeem";

    private const string POGB_BUNDLE_NAME = "metapogbooster";

    protected override void OnExecute()
    {
        saveAsBoosterPassiveEventID.value = 0;
        if(eventIconObject != null)
            GameObject.Destroy(eventIconObject);

        InitProperty();
        UpdateValues();

        EndAction();
    }

    private void InitProperty()
    {
        if(isInit) return;

        ContextElement agentElement = agent.gameObject.GetComponent<ContextElement>();
        agentElement.UpdateContext(false);

        purchasedTextElement        = ContextUtils.FindElement(agentElement, "Text Purchased", ContextSearchingType.ChildrenSearch);
        boostMultiplierTextElement  = ContextUtils.FindElement(agentElement, "Text Multiplier", ContextSearchingType.ChildrenSearch);
        coinTextElement             = ContextUtils.FindElement(agentElement, "Text Coin", ContextSearchingType.ChildrenSearch);

        purchaseButtonElement       = ContextUtils.FindElement(agentElement, "Button Boost", ContextSearchingType.ChildrenSearch);
        freeButtonElement           = ContextUtils.FindElement(agentElement, "Button Free", ContextSearchingType.ChildrenSearch);
        rpTextElement               = ContextUtils.FindElement(agentElement, "VIP Point Area/Text Vip Point", ContextSearchingType.FullNameSearch);

        eventTextElement            = ContextUtils.FindElement(agentElement, "Event Text/Text Event", ContextSearchingType.FullNameSearch);
        eventTimerElement           = ContextUtils.FindElement(agentElement, "Timer/Remaining Timer", ContextSearchingType.FullNameSearch);
        pogbEventIconAreaElement    = ContextUtils.FindElement(agentElement, "Fortune Coins Area/Event Area", ContextSearchingType.FullNameSearch);

        saleTagElement              = ContextUtils.FindElement(agentElement, "Button Boost/Sale Tag", ContextSearchingType.FullNameSearch);
        saleTagTextElement          = ContextUtils.FindElement(agentElement, "Button Boost/Sale Tag/Text", ContextSearchingType.FullNameSearch);

        jackpotElementList = new List<ContextElement>();
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Mini", ContextSearchingType.ChildrenSearch));
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Minor", ContextSearchingType.ChildrenSearch));
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Major", ContextSearchingType.ChildrenSearch));
        jackpotElementList.Add(ContextUtils.FindElement(agentElement, "Grand", ContextSearchingType.ChildrenSearch));

        jackpotAnimator = new List<Animator>();
        for(int i=0; i<jackpotElementList.Count; ++i)
        {
            jackpotAnimator.Add(jackpotElementList[i].gameObject.GetComponent<Animator>());
        }

        saveAsTargetPurhcaseID.value = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "purchaseResponse/purchaseId").value;
        var purchasePrice = BlackboardUtils.FindVariable<double>(MainBlackboard.Get(), "purchaseResponse/price");

        var itemUseResultList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "/purchaseResponse/itemUseResultList").value;
        prevEarnCoins = itemUseResultList[0].GetValue<long>("earnCredit");
        

        saveAsProductBB.value = BlackboardQueryUtils.GetPriceMatchProduct(purchasePrice.value, ShopType.POG_BOOSTER, ItemType.POG_BOOSTER);
        itemBB = BlackboardQueryUtils.GetItemFromProduct(saveAsProductBB.value, ItemType.POG_BOOSTER);

        jackpotMultiplierNumeratorList = new List<long>();
        jackpotViewMultiplierList = new List<double>();
        List<Blackboard> jackpotMultiplierListBB = itemBB.GetValue<List<Blackboard>>("setting");

        for(int i=0; i<jackpotMultiplierListBB.Count; ++i)
        {
            long numerator = jackpotMultiplierListBB[i].GetValue<long>("multiplierNumerator");
            jackpotMultiplierNumeratorList.Add( numerator );
            jackpotViewMultiplierList.Add( NumberUtils.GetMultiplierFromNumerator(numerator) );
        }

        price = saveAsProductBB.value.GetValue<double>("price");
        originalPrice = saveAsProductBB.value.GetValue<double>("originalPrice");
        isSale = price < originalPrice;
        rp = itemBB.GetValue<long>("rp");

        rootAnimator = agent.gameObject.GetComponent<Animator>();

        closeButtonElement = ContextUtils.FindElement(agentElement, "Button Close", ContextSearchingType.ChildrenSearch);
        MetaContextElementUtils.SetClickable(
            closeButtonElement,
            EVENT_CLOSE_BUTTON,
            false,
            false,
            SendEvent,
            ownerSystem
        );

        MetaContextElementUtils.SetClickable(
            purchaseButtonElement,
            EVENT_PURCHASE_BUTTON,
            false,
            false,
            SendEvent,
            ownerSystem
        );

        MetaContextElementUtils.SetClickable(
            freeButtonElement,
            EVENT_REDEEM_BUTTON,
            false,
            false,
            SendEvent,
            ownerSystem
        );

        isInit = true;
    }

    private void UpdateValues()
    {
        bool isFreeEvent = false;
        bool isEvent = false;
        long maxEventMultiplierNumerator = NumberUtils.GetGlobalDenominator();
        
        EventInfo eventInfo = GetPassiveEventInfo();
        if(eventInfo != null)
        {
            saveAsBoosterPassiveEventID.value = eventInfo.id;
            saveAsEventEndtimestamp.value = eventInfo.endTimestamp;

            switch(eventInfo.type)
            {
                case EventInfoType.POG_BOOSTER_MULTIPLY:
                    {
                        SetAllMultiplierValues(eventInfo);
                        maxEventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                        isEvent = true;
                    }
                    break;
                case EventInfoType.POG_BOOSTER_MIN_MULTIPLIER:
                    {
                        SetMinMultiplierValues(eventInfo);
                        isEvent = true;
                    }
                    break;
                case EventInfoType.POG_BOOSTER_FREE:
                    SetMultiplierList(jackpotViewMultiplierList);
                    isFreeEvent = true;
                    break;
                case EventInfoType.POG_BOOSTER_WITHOUT:
                    {
                        SetWithoutMultiplierValues(eventInfo);
                        SetMultiplierList(jackpotViewMultiplierList);
                        isEvent = true;
                    }
                    break;
            }

            MetaContextElementUtils.SetCommonRemainingTimer(eventTimerElement,
                                    eventInfo.endTimestamp,
                                    0,
                                    "TIME_FORMAT_HHMMSS_TOTALHOUR",
                                    "POPUP_POGB_PURCHASE_TIMER_OUTPUT_FORMAT",
                                    null,
                                    "Ended",
                                    true,
                                    null
                                    );

            string iconAssetName = GetEventIconAssetName(eventInfo);

            eventIconObject = MetaObjectUtils.MakePrefab(POGB_BUNDLE_NAME, iconAssetName, pogbEventIconAreaElement.transform, "");
        }
        else
        {
            SetMultiplierList(jackpotViewMultiplierList);
        }

        rootAnimator.SetBool("IsEvent", isEvent);
        
        long maxMultiplierNumerator = jackpotMultiplierNumeratorList[3];
        double maxMultiplier = jackpotViewMultiplierList[3];
        double maxEventMultiplier = 1.0;
        long totalCoin = NumberUtils.GetMultiplierNumeratorValue(prevEarnCoins, maxMultiplierNumerator);
        if(maxEventMultiplierNumerator > NumberUtils.GetGlobalDenominator())
        {
            maxEventMultiplier = NumberUtils.GetMultiplierFromNumerator(maxEventMultiplierNumerator);
            totalCoin = NumberUtils.GetMultiplierNumeratorValue(totalCoin, maxEventMultiplierNumerator);
        }

        MetaContextElementUtils.SetText(purchasedTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURHCASE_COIN_TEXT, prevEarnCoins));
        MetaContextElementUtils.SetText(boostMultiplierTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURHCASE_MULTIPLIER_TEXT, maxMultiplier * maxEventMultiplier));
        MetaContextElementUtils.SetText(coinTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_TOTAL_COIN_TEXT, totalCoin));

        MetaContextElementUtils.SimpleSetText(freeButtonElement, "Text", StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_OK_BUTTON_TEXT));

        purchaseButtonElement.gameObject.SetActive(!isFreeEvent);
        freeButtonElement.gameObject.SetActive(isFreeEvent);
        closeButtonElement.gameObject.SetActive(!isFreeEvent);
        
        GraphOwner owner = agent.GetComponent<GraphOwner>();
        MetaSystem.SubscribeBackButton(owner.GetHashCode(), () => { owner.SendEvent(isFreeEvent ? "CloseLocked" : EVENT_CLOSE_BUTTON); });

        if(isFreeEvent)
        {
            MetaContextElementUtils.SetText(rpTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_WAS_PRICE_TEXT, saveAsProductBB.value.GetValue<double>("originalPrice") ));
        }
        else
        {
            saleTagElement.gameObject.SetActive(isSale);
            if(isSale)
            {
                string eventTag = StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_PERCENT_SALE", BlackboardQueryUtils.GetProductSalePercent(saveAsProductBB.value));
                MetaContextElementUtils.SetText( saleTagTextElement, eventTag);
                MetaContextElementUtils.SimpleSetText(purchaseButtonElement, "Text", StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_BUY_WITH_SALE_BUTTON_TEXT, price, originalPrice));
            }
            else
            {
                MetaContextElementUtils.SimpleSetText(purchaseButtonElement, "Text", StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_BUY_BUTTON_TEXT, price));
            }

            MetaContextElementUtils.SetText(rpTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_RP_TEXT, rp));
        }
    }

    private void SetAllMultiplierValues(EventInfo eventInfo)
    {
        double eventMultiplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
        List<double> eventMultiplierList = new List<double>(jackpotViewMultiplierList);
        for(int i=0; i<eventMultiplierList.Count; ++i)
        {
            eventMultiplierList[i] *= eventMultiplier;
            jackpotAnimator[i].SetTrigger("Multiplier");
        }

        SetMultiplierList(eventMultiplierList);
        SetEventMultiplierList(jackpotViewMultiplierList);

        MetaContextElementUtils.SetText(eventTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_TOP_DESC_ALL_EVENT, eventMultiplier));
    }

    private void SetMinMultiplierValues(EventInfo eventInfo)
    {
        double eventMultiplier = PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo);
        List<double> eventMultiplierList = new List<double>(jackpotViewMultiplierList);
        for(int i=0; i<eventMultiplierList.Count; ++i)
        {
            if(eventMultiplierList[i] < eventMultiplier)
            {
                eventMultiplierList[i] = eventMultiplier;
                jackpotAnimator[i].SetTrigger("Multiplier");
            }
        }

        SetMultiplierList(eventMultiplierList);
        SetEventMultiplierList(jackpotViewMultiplierList);

        MetaContextElementUtils.SetText(eventTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_TOP_DESC_MIN_EVENT, eventMultiplier));
    }

    private void SetWithoutMultiplierValues(EventInfo eventInfo)
    {
        int eventIndex = PassiveEventManager.Instance.GetEventInfoIndex(eventInfo);
        List<double> eventMultiplierList = new List<double>(jackpotViewMultiplierList);
        for(int i=0; i<eventMultiplierList.Count; ++i)
        {
            if(eventIndex > i)
                jackpotAnimator[i].SetTrigger("Without");
        }
        
        string jackpotName = StringTableUtils.GetString(tableType, string.Format(POPUP_POGB_PURCHASE_JACKPOT_NAME, eventIndex - 1));
        MetaContextElementUtils.SetText(eventTextElement, StringTableUtils.GetString(tableType, POPUP_POGB_PURCHASE_CENTER_DESC_WITH_OUT_EVENT, jackpotName));
    }

    private void SetMultiplierList(List<double> multiplierList)
    {
        MetaContextElementUtils.SimpleSetText(jackpotElementList[0], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[0]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[1], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[1]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[2], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[2]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[3], "Text Jackpot", StringTableUtils.GetString(tableType, JACKPOT_TEXT_FORMAT_KEY, multiplierList[3]));
    }

    private void SetEventMultiplierList(List<double> multiplierList)
    {
        MetaContextElementUtils.SimpleSetText(jackpotElementList[0], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[0]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[1], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[1]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[2], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[2]));
        MetaContextElementUtils.SimpleSetText(jackpotElementList[3], "Text Jackpot Event", StringTableUtils.GetString(tableType, POPUP_POGB_JACKPOT_ORIG_MULTIPLIER, multiplierList[3]));
    }

    private EventInfo GetPassiveEventInfo()
    {
        EventInfo freeEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_FREE);
        if(freeEventInfo != null) return freeEventInfo;

        EventInfo allMultiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_MULTIPLY);
        if(allMultiplierEventInfo != null) return allMultiplierEventInfo;

        EventInfo withoutEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_WITHOUT);
        if(withoutEventInfo != null) return withoutEventInfo;

        EventInfo minMultiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.POG_BOOSTER_MIN_MULTIPLIER);
        if(minMultiplierEventInfo != null) return minMultiplierEventInfo;

        return null;
    }

    private string GetEventIconAssetName(EventInfo eventInfo)
    {
        switch(eventInfo.type)
        {
            case EventInfoType.POG_BOOSTER_FREE:
                {
                    return "Pot Of Gold Booster Free";
                }
                break;
            case EventInfoType.POG_BOOSTER_MULTIPLY:
                {
                    return string.Format("Pot Of Gold Booster All Multiplier x{0}", (int)PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                }
                break;
            case EventInfoType.POG_BOOSTER_WITHOUT:
                {
                    int eventIndex = PassiveEventManager.Instance.GetEventInfoIndex(eventInfo);
                    string jackpotName = StringTableUtils.GetString(tableType, string.Format(POPUP_POGB_PURCHASE_JACKPOT_NAME, eventIndex - 1));
                    return string.Format("Pot Of Gold Booster Without {0}", jackpotName);
                }
                break;
            case EventInfoType.POG_BOOSTER_MIN_MULTIPLIER:
                {
                    return string.Format("Pot Of Gold Booster Min Multiplier x{0}", (int)PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
                }
                break;
        }

        return null;
    }
}

}

