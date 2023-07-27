using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupShopItemCoinController : PopupShopItemControllerBase
    {
        private List<Blackboard> productList;
        private Blackboard productBB;
        private bool isEvent;
        private bool isTierUp;
        private List<Blackboard> coinWheelProductList;
        private bool showFlipEffect;
        private float flipEffectInterval;

        private long shopInflationNumerator;
        private bool isShopInflation;
        private float nextInflationEffectInterval;

        private long levelMultiplierInflationNumerator;
        private bool isLevelMultiplierInflation;

        private string baseItemText;

        private string shopInflationItemText;
        private string shopInflationItemWasText;
        private string shopInflationEventText;

        private string totalItemText;
        private string totalItemWasText;
        private string totalEventText;

        private int currentItemTextIndex = 0;
        private List<string> totalItemTextList = new List<string>();
        private List<string> wasItemTextList = new List<string>();
        private List<string> eventMultiplierTextList = new List<string>();
        private List<bool> isActiveWasList = new List<bool>();
        private List<bool> useShopInflationTextList = new List<bool>();

        private bool isInit = false;
        private bool updateMutex = false;
        private int eventID = -1;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private Blackboard rootBlackboard;
        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement itemTextElement;
        private ContextElement itemWasTextElement;
        private ContextElement rpTextElement;
        private ContextElement eventMultiplierTextElement;
        private ContextElement inflationMultiplierTextElement;
        private ContextElement buyButtonElement;
        private ContextElement wheelElement;
        private List<ContextElement> rewardIconAreaList;

        private List<GameObject> rewardIconList;

        private bool isSilentTierShop;

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            updateMutex = false;
            StopAllCoroutines();
        }

        public void InitProperty(List<Blackboard> productList, bool isFlipEffect, float flipEffectInterval)
        {
            if (isInit) return;

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            shopInflationNumerator = NumberUtils.GetShopInflationNumerator(ShopType.COIN);
            isShopInflation = shopInflationNumerator > NumberUtils.GetGlobalDenominator();
            nextInflationEffectInterval = 1f;

            levelMultiplierInflationNumerator = LevelUtils.GetLevelMultiplierNumerator("coin");
            isLevelMultiplierInflation = levelMultiplierInflationNumerator > 0L && LevelUtils.CheckPrefsLevelMultiplier();

            bool isLevelMultiplyFlipEffect = BlackboardUtils.FindVariable<bool>(rootBlackboard, "showLevelMultiplyFlipEffect")?.value ?? false;

            var eventInfo = GetPassiveEventMultiplierNumerator();
            eventID = eventInfo == null ? -1 : eventInfo.id;

            this.productList = productList;
            this.flipEffectInterval = flipEffectInterval;
            this.showFlipEffect = (isFlipEffect || isLevelMultiplyFlipEffect) && (isShopInflation || eventInfo != null || isLevelMultiplierInflation);

            itemTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            itemWasTextElement = ContextUtils.FindElement(rootElement, "Text Was", ContextSearchingType.ChildrenSearch);
            rpTextElement = ContextUtils.FindElement(rootElement, "Vip Point", ContextSearchingType.ChildrenSearch);
            eventMultiplierTextElement = ContextUtils.FindElement(rootElement, "Sale Text", ContextSearchingType.ChildrenSearch);
            inflationMultiplierTextElement = ContextUtils.FindElement(rootElement, "Inflation Text", ContextSearchingType.ChildrenSearch);
            buyButtonElement = ContextUtils.FindElement(rootElement, "Buy", ContextSearchingType.ChildrenSearch);
            wheelElement = ContextUtils.FindElement(rootElement, "Wheel", ContextSearchingType.ChildrenSearch);

            rewardIconAreaList = new List<ContextElement>();
            for (int i = 0; i < 3; ++i)
            {
                var areaElement = ContextUtils.FindElement(rootElement, string.Format("Reward Item Area {0}", i + 1), ContextSearchingType.ChildrenSearch);

                rewardIconAreaList.Add(areaElement);
            }

            MetaContextElementUtils.SetClickable(
                buyButtonElement,
                "OnBuyItem",
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                wheelElement,
                "OnShowCoinBoosterInformation",
                rootElement,
                null
            );

            productBB = productList[0];
            var price = productBB.GetValue<double>("price");

            coinWheelProductList = new List<Blackboard>();

            Blackboard wheelShopBB;
            var coinWheelProductBB = BlackboardQueryUtils.GetPriceMatchProduct(price, ShopType.COIN_BOOSTER, ItemType.CREDIT_MULTIPLIER_WHEEL, out wheelShopBB);
            if (coinWheelProductBB != null)
                coinWheelProductList.Add(coinWheelProductBB);

            isSilentTierShop = false;

            if (coinWheelProductList != null && coinWheelProductList.Count > 0)
            {
                isSilentTierShop = true;
                BlackboardUtils.SetOrCreateValue<List<Blackboard>>(rootBlackboard, "coinWheelProductList", coinWheelProductList);

                var wheelIconBB = wheelElement.gameObject.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue<Blackboard>(wheelIconBB, "product", coinWheelProductList[0]);
            }

            BlackboardUtils.SetOrCreateValue<Blackboard>(rootBlackboard, "product", productBB);
            BlackboardUtils.SetOrCreateValue<bool>(rootBlackboard, "silentTierShop", isSilentTierShop);

            // Make Coin Icon
            MakeItemIconObj(price);

            isInit = true;
        }

        // Call from ShopController, PassiveBehavuour.
        public void UpdateValues()
        {
            if (!isInit) return;
            if (updateMutex) return;
            updateMutex = true;

            StopCoroutine("FlipEffectDealy");
            UpdateStaticValues();
            if (showFlipEffect)
            {
                // Start Coroutine
                StartCoroutine("FlipEffectDealy");
            }
            else
            {
                rootAnimator.SetBool("IsEventSkip", true);

                if (isEvent || isLevelMultiplierInflation)
                {
                    rootAnimator.SetBool("IsInflation", false);
                    rootAnimator.SetBool("IsEvent", true);
                }
                else if (isShopInflation)
                {
                    rootAnimator.SetBool("IsInflation", true);
                    rootAnimator.SetBool("IsEvent", false);
                }

                UpdateLastTextValues();
                UpdateTags();

                updateMutex = false;
            }

            showFlipEffect = false;
        }

        private void UpdateStaticValues()
        {
            int tier = TierUtils.GetMeTier();

            double itemPrice = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB, "price").value);
            double origItemPrice = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB, "originalPrice").value);

            var coinItemBB = BlackboardQueryUtils.GetItemFromProduct(productBB, ItemType.CREDIT);
            long rp = coinItemBB.GetValue<long>("rp");
            long baseCredit = coinItemBB.GetValue<long>("baseCredit");

            long additionalCreditMultiplierNumerator = coinItemBB.GetValue<long>("additionalCreditMultiplierNumerator");

            double tierMultiplier = TierUtils.GetTierMultiplier(tier);
            long baseProductCoins = TierUtils.GetTierFractionCoin(baseCredit, tier);
            baseProductCoins = NumberUtils.GetAdditionalMultiplierNumeratorValue(baseProductCoins, additionalCreditMultiplierNumerator);

            long totalCoins = baseProductCoins;
            long totalLevelMultiplierCoin = LevelUtils.GetLevelMultiplierNumeratorValue(baseProductCoins, "coin");

            isTierUp = BlackboardUtils.GetOrCreateVariable<bool>(rootBlackboard, "isTierUp").value;

            var eventInfo = GetPassiveEventMultiplierNumerator();
            eventID = eventInfo == null ? -1 : eventInfo.id;
            isEvent = eventInfo != null;

            BlackboardUtils.SetOrCreateValue<int>(rootBlackboard, "_coinShopMultiplierEventID", eventInfo == null ? 0 : eventInfo.id);

            // Init Animator
            rootAnimator.SetBool("TierUp", false);
            rootAnimator.SetBool("MostPopular", false);
            rootAnimator.SetBool("BestValue", false);
            rootAnimator.SetBool("BaseEvent", false);
            rootAnimator.SetBool("IsEvent", false);
            rootAnimator.SetBool("IsEventSkip", !showFlipEffect);

            // Clear Item text list
            ClearItemTextInfos();
            currentItemTextIndex = 0;
            baseItemText = "";

            // flip effect priority.
            // Shop Multiply Event > LM Inflation > Shop Inflation
            if (isEvent)
            {
                long eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo);
                long totalEventCoins = NumberUtils.GetMultiplierNumeratorValue(totalLevelMultiplierCoin, eventMultiplierNumerator);

                baseItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_COINS", totalLevelMultiplierCoin);
                totalItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_COINS", totalEventCoins);
                totalItemWasText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_WAS_COINS", totalLevelMultiplierCoin);

                if (BlackboardQueryUtils.IsShopEventPercentText(ShopType.COIN))
                {
                    long viewAddPercent = PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo);
                    totalEventText = StringTableUtils.GetString(tableType, "SHOP_ITEM_EVENT_PERCENT", viewAddPercent);
                }
                else
                {
                    double eventMultiplier = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);
                    totalEventText = StringTableUtils.GetString(tableType, "SHOP_ITEM_EVENT_MULTIPLIER", eventMultiplier);
                }

                // base
                AddItemTextInfos(baseItemText, "", "", false, false);

                // total
                AddItemTextInfos(totalItemText, totalItemWasText, totalEventText, true, false);
            }
            else if (isLevelMultiplierInflation)
            {
                long prevSectionLevelMultiplierNumerator = LevelUtils.GetPriviousLevelMultiplierNumerator("coin");
                long prevSectionLevelMultiplierCoin = NumberUtils.GetMultiplierNumeratorValue(baseProductCoins, prevSectionLevelMultiplierNumerator);

                string levelMultiplierinflationText = LevelUtils.GetLevelMultiplierStringFromPreviousSection("coin");

                totalItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_COINS", totalLevelMultiplierCoin);
                totalItemWasText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_WAS_COINS", prevSectionLevelMultiplierCoin);
                totalEventText = StringTableUtils.GetString(tableType, "SHOP_ITEM_EVENT_MULTIPLIER", levelMultiplierinflationText);

                // base
                AddItemTextInfos(totalItemWasText, "", "", false, false);

                // total
                AddItemTextInfos(totalItemText, totalItemWasText, totalEventText, true, false);
            }
            else if (isShopInflation)
            {
                long inflationBaseCoin = NumberUtils.GetDevideNumeratorValue(totalLevelMultiplierCoin, shopInflationNumerator);
                baseItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_COINS", inflationBaseCoin);

                shopInflationItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_COINS", totalLevelMultiplierCoin);
                shopInflationItemWasText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_WAS_COINS", inflationBaseCoin);
                shopInflationEventText = StringTableUtils.GetString(tableType, "SHOP_ITEM_INFLATION_MULTIPLIER", NumberUtils.GetMultiplierFromNumerator(shopInflationNumerator));

                // base
                AddItemTextInfos(baseItemText, "", "", false, false);

                // total
                AddItemTextInfos(shopInflationItemText, shopInflationItemWasText, shopInflationEventText, true, true);
            }
            else
            {
                baseItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_COINS", totalLevelMultiplierCoin);
                totalItemWasText = "";
                totalEventText = "";

                // base
                AddItemTextInfos(baseItemText, "", "", false, false);
            }

            // Update Base item text
            UpdateTextValues(currentItemTextIndex);

            // itemWasTextElement.gameObject.SetActive(false);

            // MetaContextElementUtils.SetText(itemTextElement, baseItemText);
            // MetaContextElementUtils.SetText(itemWasTextElement, "");
            MetaContextElementUtils.SetText(rpTextElement, StringTableUtils.GetString(tableType, "TEXT_COMMA_NUMBER", rp));
            MetaContextElementUtils.SimpleSetText(buyButtonElement, "Text", StringTableUtils.GetString(tableType, "SHOP_BUY_BUTTON", itemPrice));

            rootAnimator.SetBool("UserWheel", isSilentTierShop);
        }

        public void UpdateRewards()
        {
            if (rewardIconList != null && rewardIconList.Count > 0) return;

            var productBB = productList[0];
            var rewardListVisualizeInfoBB = BlackboardUtils.FindVariable<Blackboard>(productBB, "rewardListVisualizeInfo");

            if (rewardListVisualizeInfoBB != null && rewardListVisualizeInfoBB.value != null)
            {
                var useRewardPackageIcon = rewardListVisualizeInfoBB.value.GetValue<bool>("useRewardPackageIcon");
                if (useRewardPackageIcon)
                {
                    var iconURL = rewardListVisualizeInfoBB.value.GetValue<string>("rewardPackageIconUrl");
                    if (!string.IsNullOrEmpty(iconURL))
                    {
                        // Make Web Image Icon. & Set Image.
                        rewardIconList = new List<GameObject>();

                        var rewardPackageIconSize = rewardListVisualizeInfoBB.value.GetValue<RewardPackageIconSize>("rewardPackageIconSize");

                        GameObject go = null;

                        switch (rewardPackageIconSize)
                        {
                            case RewardPackageIconSize.ONE_BY_ONE:
                                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Shop Rewards Item Web Image", rewardIconAreaList[0].transform, null);
                                break;
                            case RewardPackageIconSize.ONE_BY_TWO:
                                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Shop Rewards Item Web Image", rewardIconAreaList[2].transform, null);
                                break;
                        }

                        if (go != null)
                        {
                            ContextElement element = go.GetComponent<ContextElement>();
                            IContextImage imageElement = element as IContextImage;

                            imageElement.SetHash(iconURL.GetHashCode().ToString());

                            WebImageDownloader.Instance.LoadWebImage(
                                iconURL,
                                CacheType.FileCache,
                                true,
                                RewardImageLoadSuccess,
                                delegate (Sprite img)
                                {
                                    if (img != null)
                                    {
                                        if (imageElement.CheckHash(iconURL.GetHashCode().ToString()))
                                        {
                                            imageElement.SetSprite(img);
                                        }
                                    }
                                },
                                null,
                                RewardImageLoadFailed
                            );
                        }
                    }
                }
                else
                {
                    var rewardViewInfoListBB = rewardListVisualizeInfoBB.value.GetValue<List<Blackboard>>("rewardViewInfoList");
                    if (rewardViewInfoListBB != null && rewardViewInfoListBB.Count > 0)
                    {
                        rewardIconList = new List<GameObject>();
                        var rewardListBB = productBB.GetValue<List<Blackboard>>("rewardList");

                        int makeCount = 0;

                        for (int i = 0; i < rewardViewInfoListBB.Count; ++i)
                        {
                            var rewardIndex = rewardViewInfoListBB[i].GetValue<int>("rewardIndex");
                            var quantity = rewardViewInfoListBB[i].GetValue<long>("quantity");

                            var go = MakeRewardIcon(rewardListBB[rewardIndex], rewardIconAreaList[makeCount].transform, null, i);

                            if (go != null)
                            {
                                SetIconValues(go, rewardListBB[rewardIndex]);
                                SetQuantity(go, quantity);

                                rewardIconList.Add(go);
                                makeCount++;
                                if (makeCount >= 2)
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void MakeItemIconObj(double price)
        {
            string bundle = MetaStringDefine.LOBBY_BUNDLE_NAME;

            var priceSectionList = BlackboardUtils.FindVariable<List<double>>(MainBlackboard.Get(), "values/misc/SHOP_COIN_ICON_SECTION_LIST")?.value;
            int grade = priceSectionList.IndexOfRange(price) + 1;
            string asset = "Shop Icon Coin Lv " + grade.ToString();

            var imageAnchor = ContextUtils.FindElement(rootElement, "Image Anchor", ContextSearchingType.FullNameSearch);
            Transform parent = imageAnchor.transform;

            MetaObjectUtils.MakePrefab(bundle, asset, parent);
        }

        private void AddItemTextInfos(string totalItemText, string wasItemText, string eventMultiplierText, bool isActiveWas, bool useShopInflationText)
        {
            totalItemTextList.Add(totalItemText);
            wasItemTextList.Add(wasItemText);
            eventMultiplierTextList.Add(eventMultiplierText);
            isActiveWasList.Add(isActiveWas);
            useShopInflationTextList.Add(useShopInflationText);
        }

        private void ClearItemTextInfos()
        {
            totalItemTextList.Clear();
            wasItemTextList.Clear();
            eventMultiplierTextList.Clear();
            isActiveWasList.Clear();
            useShopInflationTextList.Clear();
        }

        private void UpdateLastTextValues()
        {
            if(totalItemTextList == null) return;
            UpdateTextValues(totalItemTextList.Count - 1);
        }

        private void UpdateTextValues(int index)
        {
            if(index < 0 || totalItemTextList.Count <= index) return;

            itemWasTextElement.gameObject.SetActive(isActiveWasList[index]);

            MetaContextElementUtils.SetText(itemTextElement, totalItemTextList[index]);
            MetaContextElementUtils.SetText(itemWasTextElement, wasItemTextList[index]);

            if(useShopInflationTextList[index])
                MetaContextElementUtils.SetText(inflationMultiplierTextElement, eventMultiplierTextList[index]);
            else
                MetaContextElementUtils.SetText(eventMultiplierTextElement, eventMultiplierTextList[index]);
        }

        private void UpdateTags()
        {
            // Update Tag.
            rootAnimator.SetBool("TierUp", isTierUp && !showFlipEffect);
            rootAnimator.SetBool("MostPopular", false);
            rootAnimator.SetBool("BestValue", false);
            rootAnimator.SetBool("BaseEvent", false);

            if (!isTierUp && !showFlipEffect)
            {
                ProductTagType tagType = productBB.GetValue<ProductTagType>("tagType");

                rootAnimator.SetBool("MostPopular", tagType == ProductTagType.POPULAR);
                rootAnimator.SetBool("BestValue", tagType == ProductTagType.BEST);
                rootAnimator.SetBool("BaseEvent", (tagType != ProductTagType.NONE && tagType != ProductTagType.UNKNOWN));
            }
        }

        private void RewardImageLoadSuccess(string url)
        {
            // Close Loading.
        }

        private void RewardImageLoadFailed(WebImageDownloader.WebImageDownloadError error)
        {
            // Retry??
        }

        private IEnumerator FlipEffectDealy()
        {
            if (isEvent || isLevelMultiplierInflation)
            {
                // if (isShopInflation)
                //     yield return new WaitForSeconds(nextInflationEffectInterval);
                // else
                yield return new WaitForSeconds(flipEffectInterval);

                rootAnimator.SetBool("IsInflation", false);
                rootAnimator.SetBool("IsEvent", true);
                rootAnimator.SetBool("IsEventSkip", false);

                yield return new WaitForSeconds(0.2f);

                UpdateTextValues(++currentItemTextIndex);
            }
            else if (isShopInflation)
            {
                yield return new WaitForSeconds(flipEffectInterval);

                rootAnimator.SetBool("IsInflation", true);
                rootAnimator.SetBool("IsEvent", false);
                rootAnimator.SetBool("IsEventSkip", false);

                yield return new WaitForSeconds(0.2f);

                UpdateTextValues(++currentItemTextIndex);
            }

            UpdateTags();

            updateMutex = false;
        }

        private EventInfo GetPassiveEventMultiplierNumerator()
        {
            return PassiveEventUtils.GetPassiveEvent(ShopType.COIN);
        }

        public void StartPassiveEvent()
        {
            if (updateMutex) return;

            var eventInfo = GetPassiveEventMultiplierNumerator();
            if (eventInfo != null && eventInfo.id != eventID)
            {
                eventID = eventInfo.id;
                showFlipEffect = true;
                UpdateValues();
            }
        }
    }
}
