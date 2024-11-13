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
public class UpdatePotOfGoldPopupPOGBEvents : ActionTask<Blackboard> 
{
    public BBParameter<Blackboard> productBB;

    public BBParameter<bool> saveAsIsLoadPOGBoosterAssets;

    private bool isInit = false;

    private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

    private ContextElement pogbAreaElement;
    private ContextElement pogbIconElement;
    private ContextElement pogbEventIconAreaElement;
    private ContextElement withoutTextRemainingTimerElement;

    private ContextElement eventTagTextElement;
    private ContextElement eventTagRemainingTimerElement;

    private Blackboard pogbProductBB;
    private bool isPogbEvent;
    private Blackboard pogbShopBB;

    private Animator jackpotIconAnimator;

    private GameObject eventIconObject;

    private const string TIME_FORMAT_HHMMSS_TOTALHOUR = "TIME_FORMAT_HHMMSS_TOTALHOUR";
    private const string POPUP_POGB_PURCHASE_JACKPOT_NAME = "POGB_JACKPOT_NAME_{0}";

    protected override void OnExecute()
    {
        saveAsIsLoadPOGBoosterAssets.value = false;

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

        ContextElement buyButtonElement = ContextUtils.FindElement(agentElement, "Button Green Pot Of Gold Take", ContextSearchingType.ChildrenSearch);
        pogbAreaElement = ContextUtils.FindElement(buyButtonElement, "Fortune Coins Area", ContextSearchingType.ChildrenSearch);
        pogbIconElement = ContextUtils.FindElement(pogbAreaElement, "Popup Pot Of Gold Booster Button Icon", ContextSearchingType.ChildrenSearch);
        pogbEventIconAreaElement = ContextUtils.FindElement(pogbIconElement, "Event Area", ContextSearchingType.ChildrenSearch);
        eventTagTextElement = ContextUtils.FindElement(pogbIconElement, "Event Tag/Text", ContextSearchingType.FullNameSearch);
        eventTagRemainingTimerElement = ContextUtils.FindElement(pogbIconElement, "Event Tag/Remaining Timer", ContextSearchingType.FullNameSearch);
        withoutTextRemainingTimerElement = ContextUtils.FindElement(pogbIconElement, "Event Tag Without Text/Remaining Timer", ContextSearchingType.FullNameSearch);

        jackpotIconAnimator = pogbIconElement.gameObject.GetComponent<Animator>();

        isInit = true;
    }

    private void UpdateValues()
    {
        double price = productBB.value.GetValue<double>("price");
        pogbProductBB = BlackboardQueryUtils.GetPriceMatchProduct(price, ShopType.POG_BOOSTER, ItemType.POG_BOOSTER, out pogbShopBB);

        if(pogbProductBB != null)
        {
            // Check Passive. 
            saveAsIsLoadPOGBoosterAssets.value = true;
            pogbAreaElement.gameObject.SetActive(true);
            EventInfo eventInfo = GetPassiveEventInfo();

            if(eventInfo != null)
            {
                string assetName = GetEventIconAssetName(eventInfo);

                jackpotIconAnimator.SetBool("IsEvent", true);
                jackpotIconAnimator.SetBool("IsSale", false);
                
                eventIconObject = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, assetName, pogbEventIconAreaElement.transform, "");
                MetaContextElementUtils.SetCommonRemainingTimer(withoutTextRemainingTimerElement, eventInfo.endTimestamp, 0, TIME_FORMAT_HHMMSS_TOTALHOUR, "", "", "Ended", true, null);
            }
            else
            {
                if(isPogbEvent)
                {
                    jackpotIconAnimator.SetBool("IsEvent", false);
                    jackpotIconAnimator.SetBool("IsSale", true);

                    var endTimestamp = pogbShopBB.GetValue<long>("endTimestamp");
                    var tagText = StringTableUtils.GetString(tableType, "TEXT_COMMON_PASSIVE_EVENT_PERCENT_SALE", BlackboardQueryUtils.GetProductSalePercent(pogbProductBB));
                    MetaContextElementUtils.SetText(eventTagTextElement, tagText);

                    MetaContextElementUtils.SetCommonRemainingTimer(eventTagRemainingTimerElement, endTimestamp, 0, TIME_FORMAT_HHMMSS_TOTALHOUR, "", "", "Ended", true, agent.gameObject);
                }
                else
                {
                    jackpotIconAnimator.SetBool("IsEvent", false);
                    jackpotIconAnimator.SetBool("IsSale", false);
                }
            }
        }
        else
        {
            pogbAreaElement.gameObject.SetActive(false);
        }
    }

    private EventInfo GetPassiveEventInfo()
    {
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

    private string GetEventIconAssetName(EventInfo eventInfo)
    {
        switch(eventInfo.type)
        {
            case EventInfoType.POG_BOOSTER_FREE:
                return "Pot Of Gold Booster Button Free";
            case EventInfoType.POG_BOOSTER_MULTIPLY:
                return string.Format("Pot Of Gold Booster Button All Multiplier x{0}", (long)PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
            case EventInfoType.POG_BOOSTER_WITHOUT:
                {
                    int eventIndex = PassiveEventManager.Instance.GetEventInfoIndex(eventInfo);
                    string jackpotName = StringTableUtils.GetString(tableType, string.Format(POPUP_POGB_PURCHASE_JACKPOT_NAME, eventIndex - 1));
                    return string.Format("Pot Of Gold Booster Button Without {0}", jackpotName);
                }
            case EventInfoType.POG_BOOSTER_MIN_MULTIPLIER:
                return string.Format("Pot Of Gold Booster Button Min Multiplier x{0}", (long)PassiveEventManager.Instance.GetEventInfoViewMultiplier(eventInfo));
        }

        return null;
    }
}

}
