using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupPotOfGoldController : MonoBehaviour
    {
        private Blackboard rootBB;
        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement potElement;
        private ContextElement purchaseButtonElement;
        private ContextElement okButtonElement;
        private ContextElement closeButtonElement;

        private ContextElement contentsTextElement;
        private ContextElement eventBadgeAreaElement;
        private Transform      eventBadgeObject;
        private ContextElement inflationBadgeElement;
        private ContextElement buyButtonTextElement;
        private ContextElement rpTextElement;
        private ContextElement saleTagAreaElement;
        private ContextElement bottomDescElement;

        private ContextElement shopImageElement;

        private Blackboard productBB;
        private Blackboard itemBB;

        private bool skipEffect;

        private long inflationNumerator;
        private bool isInflation;
        private bool showInflationEffect;

        private long baseCoin;
        private long inflationCoin;
        private long eventCoin;

        private float changeInterval = 1f;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string EVENT_CLOSE_BUTTON = "ClosePotOfGold";
        private const string EVENT_PURCHASE_BUTTON = "BuyPotOfGold";

        private bool isInit = false;

        private EventInfo multiplierEventInfo = null;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void InitProperty()
        {
            if(isInit) return;

            inflationNumerator = NumberUtils.GetShopInflationNumerator(ShopType.PIGGY_BANK);
            // inflationNumerator = BlackboardUtils.FindVariable<long>(null, "/values/misc/INFLATION_NUMERATOR_POG").value;
            isInflation = inflationNumerator > NumberUtils.GetGlobalDenominator();
            showInflationEffect = isInflation;

            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            potElement = ContextUtils.FindElement(rootElement, "Pot", ContextSearchingType.ChildrenSearch);

            purchaseButtonElement = ContextUtils.FindElement(rootElement, "Button Green Pot Of Gold Take", ContextSearchingType.ChildrenSearch);
            okButtonElement = ContextUtils.FindElement(rootElement, "Button Ok", ContextSearchingType.ChildrenSearch);
            closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            //////////////////

            contentsTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            eventBadgeAreaElement = ContextUtils.FindElement(rootElement, "Pot/Badge Area", ContextSearchingType.FullNameSearch);
            eventBadgeObject = eventBadgeAreaElement.transform.Find("Badge Event");
            inflationBadgeElement = ContextUtils.FindElement(eventBadgeAreaElement, "Badge Inflation", ContextSearchingType.FullNameSearch);

            ContextElement buyButtonElement = ContextUtils.FindElement(rootElement, "Button Green Pot Of Gold Take", ContextSearchingType.ChildrenSearch);
            buyButtonTextElement = ContextUtils.FindElement(buyButtonElement, "Text", ContextSearchingType.ChildrenSearch);
            saleTagAreaElement = ContextUtils.FindElement(buyButtonElement, "Sale Tag", ContextSearchingType.ChildrenSearch);
            // jackpotIconelement = ContextUtils.FindElement(buyButtonElement, "Fortune Coins Area", ContextSearchingType.ChildrenSearch);

            rpTextElement = ContextUtils.FindElement(rootElement, "Text Vip Point", ContextSearchingType.ChildrenSearch);

            bottomDescElement = ContextUtils.FindElement(rootElement, "Popup Bottom Description", ContextSearchingType.ChildrenSearch);

            shopImageElement = ContextUtils.FindElement(rootElement, "Event Header Banner/Image", ContextSearchingType.FullNameSearch);

            isInit = true;
        }

        private void CalculateCoins()
        {
            long mePogCoins = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/piggyCredit").value;
            int meTier = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "me/tier").value;
            baseCoin = TierUtils.GetTierFractionCoin(mePogCoins, meTier);
            baseCoin = LevelUtils.GetLevelMultiplierNumeratorValue(baseCoin, "pog");
            inflationCoin = baseCoin;

            if(isInflation)
                baseCoin = NumberUtils.GetDevideNumeratorValue(baseCoin, inflationNumerator);
        }

        public void UpdateStaticValues(Blackboard productBB, bool isNew)
        {
            InitProperty();

            skipEffect = isNew;
            this.productBB = productBB;

            purchaseButtonElement.gameObject.SetActive(!isNew);
            okButtonElement.gameObject.SetActive(isNew);

            Animator potAnimator = potElement.gameObject.GetComponent<Animator>();
            
            rootAnimator.SetBool("New", isNew);
            rootAnimator.SetBool("Old", !isNew);
            rootAnimator.SetBool("Active", true);

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                EVENT_CLOSE_BUTTON,
                rootElement,
                null
            );

            CalculateCoins();

            if(isNew)
            {
                MetaContextElementUtils.SimpleSetText(rootElement, "Title Area/Text", StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_TITLE_2"), ContextSearchingType.FullNameSearch);
                MetaContextElementUtils.SimpleSetText(potElement, "Text Progress", StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_SAVED_PERCENT", 0), ContextSearchingType.ChildrenSearch);
                potAnimator.SetInteger("Level", 1);
                potAnimator.SetBool("IsFull", false);

                MetaContextElementUtils.SimpleSetText(okButtonElement, "Text", StringTableUtils.GetString(tableType, "BUTTON_OK"), ContextSearchingType.ChildrenSearch);
                MetaContextElementUtils.SetClickable(
                    okButtonElement,
                    EVENT_CLOSE_BUTTON,
                    rootElement,
                    null
                );
            }
            else
            {
                Blackboard pogInfoBB = BlackboardQueryUtils.GetItemFromProduct(productBB, ItemType.PIGGY_BANK);
                long mePogCoins      = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/piggyCredit").value;
                long minCoins        = pogInfoBB.GetValue<long>("minCredit");
                long maxCoins        = pogInfoBB.GetValue<long>("maxCredit");

                var sectionList      = BlackboardUtils.FindVariable<List<double>>(MainBlackboard.Get(), "values/misc/PIGGY_BANK_INFO_APPEAR_SECTION_LIST").value;

                float proportion = (float)(mePogCoins - minCoins) / (float)(maxCoins - minCoins);
                proportion = Mathf.Clamp(proportion, 0f, 1f);

                int pogLevel = 0;
                for(int i=0; i<sectionList.Count; ++i)
                {
                    if((float)sectionList[i] <= proportion)
                        pogLevel = i + 1;
                    else
                        break;
                }

                MetaContextElementUtils.SimpleSetText(rootElement, "Title Area/Text", StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_TITLE_1"), ContextSearchingType.FullNameSearch);
                MetaContextElementUtils.SimpleSetText(potElement, "Text Progress", StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_SAVED_PERCENT", (int)(proportion *= 100f)), ContextSearchingType.ChildrenSearch);

                potAnimator.SetInteger("Level", pogLevel);
                potAnimator.SetBool("IsFull", sectionList.Count == pogLevel);

                MetaContextElementUtils.SetClickable(
                    purchaseButtonElement,
                    EVENT_PURCHASE_BUTTON,
                    rootElement,
                    null
                );
            }

            // Set Tier Icon
            ContextElement iconTierElement = ContextUtils.FindElement(rootElement, "Icon Tier Multiplier", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetBlackboardValue(iconTierElement, "multiplierType", TierMultiplierTableType.CoinMultiplier);
            MetaContextElementUtils.SetBlackboardValue(iconTierElement, "isRefresh", true);

            var pogShopBB = BlackboardQueryUtils.GetShopBB(ShopType.PIGGY_BANK);
            if(pogShopBB != null)
            {
                var shopImageURL = pogShopBB.GetValue<string>("imageUrl");
                if(!string.IsNullOrEmpty(shopImageURL))
                    MetaContextElementUtils.SetWebImage(shopImageElement, shopImageURL, CacheType.FileCache, false, null);
            }
        }

        public void UpdateDynamicValues(Blackboard productBB, Blackboard itemInfo)
        {
            this.productBB = productBB;
            this.itemBB = itemInfo;

            var saveAsMultiplierEventID = BlackboardUtils.GetOrCreateVariable<int>(rootBB, "_piggyEventID");
            var saveAsSaleEventID = BlackboardUtils.GetOrCreateVariable<int>(rootBB, "_piggyEventSaleID");
            var saveAsEventEndTimestamp = BlackboardUtils.GetOrCreateVariable<long>(rootBB, "targetTimestamp");

            saveAsMultiplierEventID.value = -1;
            saveAsSaleEventID.value = -1;
            saveAsEventEndTimestamp.value = 0;

            multiplierEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_MULTIPLY);

            CalculateCoins();

            bottomDescElement.gameObject.SetActive(multiplierEventInfo != null);

            if(isInflation)
            {
                MetaContextElementUtils.SimpleSetText(inflationBadgeElement, "Text", StringTableUtils.GetString(tableType, "TEXT_POT_OF_GOLD_INFLATION_MULTIPLIER", NumberUtils.GetMultiplierFromNumerator(inflationNumerator)) );
            }

            if(multiplierEventInfo != null)
            {
                long pogEventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(multiplierEventInfo);
                eventCoin = NumberUtils.GetMultiplierNumeratorValue(inflationCoin, pogEventMultiplierNumerator);

                if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.PIGGY_BANK) )
                {
                    long viewAddPercent = PassiveEventManager.Instance.GetEventInfoViewAddPercent(multiplierEventInfo);
                    MetaContextElementUtils.SimpleSetText(eventBadgeAreaElement, "Text", StringTableUtils.GetString(tableType, "TEXT_POT_OF_GOLD_ADDTIONAL_PERCENT", viewAddPercent));
                }
                else
                {
                    double pogEventMultiplier = NumberUtils.GetMultiplierFromNumerator(pogEventMultiplierNumerator);
                    MetaContextElementUtils.SimpleSetText(eventBadgeAreaElement, "Text", StringTableUtils.GetString(tableType, "TEXT_POT_OF_GOLD_MULTIPLIER", pogEventMultiplier));
                }

                saveAsMultiplierEventID.value = multiplierEventInfo.id;
                saveAsEventEndTimestamp.value = multiplierEventInfo.endTimestamp;
            }

            double price = productBB.GetValue<double>("price");
            double originalPrice = productBB.GetValue<double>("originalPrice");
            EventInfo saleEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.PIGGY_BANK_SALE);

            if(saleEventInfo != null)
            {
                int productIndex = PassiveEventManager.Instance.GetEventInfoIndex(saleEventInfo);
                int salePercent = BlackboardQueryUtils.GetPotOfGoldSalePercent(productIndex);

                MetaContextElementUtils.SetText(buyButtonTextElement, StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_BUTTON_SALE_BUY", price, originalPrice));
                MetaContextElementUtils.SimpleSetText(saleTagAreaElement, "Text", StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_SAVED_PERCENT", salePercent));

                saleTagAreaElement.gameObject.SetActive(true);

                saveAsSaleEventID.value = saleEventInfo.id;
                if(saveAsEventEndTimestamp.value == 0 || saveAsEventEndTimestamp.value > saleEventInfo.endTimestamp)
                    saveAsEventEndTimestamp.value = saleEventInfo.endTimestamp;
            }
            else
            {
                MetaContextElementUtils.SetText(buyButtonTextElement, StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_BUTTON_BUY", price));
                saleTagAreaElement.gameObject.SetActive(false);
            }

            long rp = itemInfo.GetValue<long>("rp");
            MetaContextElementUtils.SetText(rpTextElement, StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_RP_TEXT", rp));

            StopCoroutine("ChangeEffect");
            StartCoroutine("ChangeEffect");
        }

        private void UpdateBase()
        {
            rootAnimator.SetBool("isBadgeActive", false);
            MetaContextElementUtils.SetText(contentsTextElement, StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_CONTENTS_COIN_TEXT_DEFAULT", baseCoin));
            eventBadgeAreaElement.gameObject.SetActive(false);
        }

        private void UpdateInflation()
        {
            rootAnimator.SetBool("isBadgeActive", true);
            MetaContextElementUtils.SetText(contentsTextElement, StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_CONTENTS_COIN_TEXT_EVENT", inflationCoin, baseCoin));
            inflationBadgeElement.gameObject.SetActive(true);
            eventBadgeAreaElement.gameObject.SetActive(true);
            eventBadgeObject.gameObject.SetActive(false);
        }

        private void UpdateEvent()
        {
            rootAnimator.SetBool("isBadgeActive", true);
            MetaContextElementUtils.SetText(contentsTextElement, StringTableUtils.GetString(tableType, "POPUP_POT_OF_GOLD_CONTENTS_COIN_TEXT_EVENT", eventCoin, inflationCoin));
            inflationBadgeElement.gameObject.SetActive(false);
            eventBadgeAreaElement.gameObject.SetActive(true);
            eventBadgeObject.gameObject.SetActive(true);
        }

        private IEnumerator ChangeEffect()
        {
            if(skipEffect)
            {
                if(multiplierEventInfo != null)
                {
                    UpdateEvent();
                }
                else if(showInflationEffect)
                {
                    UpdateInflation();
                }
                else
                {
                    UpdateBase();
                }
            }
            else
            {
                if(showInflationEffect)
                {
                    UpdateBase();
                    showInflationEffect = false;
                    yield return new WaitForSeconds(changeInterval);
                    rootAnimator.SetBool("isEvent", false);
                    yield return null;
                    rootAnimator.SetBool("isEvent", true);
                    yield return new WaitForSeconds(0.4f);
                    UpdateInflation();
                }
                else
                {
                    UpdateBase();
                }

                if(multiplierEventInfo != null)
                {
                    yield return new WaitForSeconds(changeInterval);
                    rootAnimator.SetBool("isEvent", false);
                    yield return null;
                    rootAnimator.SetBool("isEvent", true);
                    yield return new WaitForSeconds(0.4f);
                    UpdateEvent();
                }
            }
        }
    }
}
