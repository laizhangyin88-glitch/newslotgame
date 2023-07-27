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
    public class PopupShopItemGemController : PopupShopItemControllerBase
    {
        private List<Blackboard> productList;
        private Blackboard productBB;
        private bool isEvent;
        private bool isTierUp;
        private List<Blackboard> gemWheelProductList;
        private bool showFlipEffect;
        private float flipEffectInterval;

        private long inflationNumerator;
        private bool isInflation;
        private float inflationEffectInterval;

        private string baseItemText;

        private string inflationItemText;
        private string inflationItemWasText;
        private string inflationEventText;

        private string totalItemText;
        private string totalItemWasText;
        private string totalEventText;

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
            if(isInit) return;

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            inflationNumerator = NumberUtils.GetShopInflationNumerator(ShopType.GEM);
            isInflation = inflationNumerator > NumberUtils.GetGlobalDenominator();
            inflationEffectInterval = 1f;

            var eventInfo = GetPassiveEventMultiplierNumerator();
            eventID = eventInfo == null ? -1 : eventInfo.id;

            this.productList = productList;
            this.flipEffectInterval = flipEffectInterval;
            this.showFlipEffect = isFlipEffect && (isInflation || eventInfo != null);

            itemTextElement = ContextUtils.FindElement(rootElement, "Text", ContextSearchingType.ChildrenSearch);
            itemWasTextElement = ContextUtils.FindElement(rootElement, "Text Was", ContextSearchingType.ChildrenSearch);
            rpTextElement = ContextUtils.FindElement(rootElement, "Vip Point", ContextSearchingType.ChildrenSearch);
            eventMultiplierTextElement = ContextUtils.FindElement(rootElement, "Sale Text", ContextSearchingType.ChildrenSearch);
            inflationMultiplierTextElement = ContextUtils.FindElement(rootElement, "Inflation Text", ContextSearchingType.ChildrenSearch);
            buyButtonElement = ContextUtils.FindElement(rootElement, "Buy", ContextSearchingType.ChildrenSearch);
            wheelElement = ContextUtils.FindElement(rootElement, "Wheel", ContextSearchingType.ChildrenSearch);

            rewardIconAreaList = new List<ContextElement>();
            for(int i=0; i<3; ++i)
            {
                var areaElement = ContextUtils.FindElement(rootElement, string.Format("Reward Item Area {0}", i+1), ContextSearchingType.ChildrenSearch);

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
                "OnShowGemBoosterInformation",
                rootElement,
                null
            );

            productBB = productList[0];
            var price = productBB.GetValue<double>("price");

            gemWheelProductList = new List<Blackboard>();

            Blackboard wheelShopBB;
            var gemWheelProductBB = BlackboardQueryUtils.GetPriceMatchProduct(price, ShopType.GEM_BOOSTER, ItemType.GEM_BOOSTER, out wheelShopBB);
            if (gemWheelProductBB != null)
                gemWheelProductList.Add(gemWheelProductBB);

            isSilentTierShop = false;

            if (gemWheelProductList != null && gemWheelProductList.Count > 0)
            {
                isSilentTierShop = true;
                BlackboardUtils.SetOrCreateValue<List<Blackboard>>(rootBlackboard, "gemWheelProductList", gemWheelProductList);

                var wheelIconBB = wheelElement.gameObject.GetComponent<Blackboard>();
                BlackboardUtils.SetOrCreateValue<Blackboard>(wheelIconBB, "product", gemWheelProductList[0]);
            }

            BlackboardUtils.SetOrCreateValue<Blackboard>( rootBlackboard, "product", productBB);
            BlackboardUtils.SetOrCreateValue<bool>(rootBlackboard, "silentTierShop", isSilentTierShop);

            // Make Gem Icon
            MakeItemIconObj(price);

            isInit = true;
        }

        // Call from ShopController, PassiveBehavuour.
        public void UpdateValues()
        {
            if(!isInit) return;
            if(updateMutex) return;
            updateMutex = true;

            StopCoroutine("FlipEffectDealy");
            UpdateStaticValues();
            if(showFlipEffect)
            {
                // Start Coroutine
                StartCoroutine("FlipEffectDealy");
            }
            else
            {
                rootAnimator.SetBool("IsEventSkip", true);

                if(isEvent)
                {
                    rootAnimator.SetBool("IsInflation", false);
                    rootAnimator.SetBool("IsEvent", true);

                    UpdateEventValues();
                }
                else if(isInflation)
                {
                    rootAnimator.SetBool("IsInflation", true);
                    rootAnimator.SetBool("IsEvent", false);

                    UpdateInflationValues();
                }

                UpdateTags();

                updateMutex = false;
            }

            showFlipEffect = false;
        }

        private void UpdateStaticValues()
        {
            int tier = TierUtils.GetMeTier();
            double itemPrice = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB, "price").value);
            double origItemPrice    = System.Convert.ToSingle(BlackboardUtils.FindVariable<double>(productBB, "originalPrice").value);

            var gemItemBB = BlackboardQueryUtils.GetItemFromProduct(productBB, ItemType.GEM);

            long baseGem = gemItemBB.GetValue<long>("gem");
            long rp  = gemItemBB.GetValue<long>("rp");

            long totalBaseGem = TierUtils.GetTierFractionCoin(baseGem, tier);

            isTierUp = BlackboardUtils.GetOrCreateVariable<bool>(rootBlackboard, "isTierUp").value;

            var eventInfo = GetPassiveEventMultiplierNumerator();
            eventID = eventInfo == null ? -1 : eventInfo.id;
            isEvent = eventInfo != null;

            BlackboardUtils.SetOrCreateValue<int>( rootBlackboard, "_gemShopMultiplierEventID", eventInfo == null ? 0 : eventInfo.id);
            BlackboardUtils.SetOrCreateValue<long>(rootBlackboard, "_totalBaseGem", totalBaseGem);

            // Init Animator
            rootAnimator.SetBool("TierUp", false);
            rootAnimator.SetBool("MostPopular", false);
            rootAnimator.SetBool("BestValue", false);
            rootAnimator.SetBool("BaseEvent", false);
            rootAnimator.SetBool("IsEvent", false);
            rootAnimator.SetBool("IsEventSkip", !showFlipEffect);

            if(isInflation)
            {
                long inflationBaseGem = NumberUtils.GetDevideNumeratorValue(totalBaseGem, inflationNumerator);
                baseItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_GEMS", inflationBaseGem);

                inflationItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_GEMS", totalBaseGem);
                inflationItemWasText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_WAS_GEMS", inflationBaseGem);

                inflationEventText = StringTableUtils.GetString(tableType, "SHOP_ITEM_INFLATION_MULTIPLIER", NumberUtils.GetMultiplierFromNumerator(inflationNumerator));
            }
            else
            {
                baseItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_GEMS", totalBaseGem);
                inflationItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_GEMS", totalBaseGem);
                inflationItemWasText = "";
                inflationEventText = "";
            }

            if(isEvent)
            {
                long eventMultiplierNumerator = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(eventInfo); 
                long totalGem = NumberUtils.GetMultiplierNumeratorValue(totalBaseGem, eventMultiplierNumerator);
                totalItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_GEMS", totalGem);
                totalItemWasText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_WAS_GEMS", totalBaseGem);

                if( BlackboardQueryUtils.IsShopEventPercentText(ShopType.GEM) )
                {
                    long viewAddPercent = PassiveEventManager.Instance.GetEventInfoViewAddPercent(eventInfo);
                    totalEventText = StringTableUtils.GetString(tableType, "SHOP_ITEM_EVENT_PERCENT", viewAddPercent);
                }
                else
                {
                    double eventMultiplier   = NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);
                    totalEventText = StringTableUtils.GetString(tableType, "SHOP_ITEM_EVENT_MULTIPLIER", eventMultiplier);
                }
            }
            else
            {
                totalItemText = StringTableUtils.GetString(tableType, "SHOP_ITEM_TOTAL_GEMS", totalBaseGem);
                totalItemWasText = "";
                totalEventText = "";
            }

            itemWasTextElement.gameObject.SetActive(false);

            MetaContextElementUtils.SetText(itemTextElement, baseItemText);
            MetaContextElementUtils.SetText(itemWasTextElement, "");
            MetaContextElementUtils.SetText(rpTextElement, StringTableUtils.GetString(tableType, "TEXT_COMMA_NUMBER", rp));
            MetaContextElementUtils.SimpleSetText(buyButtonElement, "Text", StringTableUtils.GetString(tableType, "SHOP_BUY_BUTTON", itemPrice));

            rootAnimator.SetBool("UserWheel", isSilentTierShop);
        }

        public void UpdateRewards()
        {
            if(rewardIconList != null && rewardIconList.Count > 0) return;

            var productBB = productList[0];
            var rewardListVisualizeInfoBB = BlackboardUtils.FindVariable<Blackboard>(productBB, "rewardListVisualizeInfo");

            if(rewardListVisualizeInfoBB != null && rewardListVisualizeInfoBB.value != null)
            {
                var useRewardPackageIcon = rewardListVisualizeInfoBB.value.GetValue<bool>("useRewardPackageIcon");
                if(useRewardPackageIcon)
                {
                    var iconURL = rewardListVisualizeInfoBB.value.GetValue<string>("rewardPackageIconUrl");
                    if(!string.IsNullOrEmpty(iconURL))
                    {
                        // Make Web Image Icon. & Set Image. 
                        rewardIconList = new List<GameObject>();

                        var rewardPackageIconSize = rewardListVisualizeInfoBB.value.GetValue<RewardPackageIconSize>("rewardPackageIconSize");

                        GameObject go = null;

                        switch(rewardPackageIconSize)
                        {
                            case RewardPackageIconSize.ONE_BY_ONE:
                                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Shop Rewards Item Web Image", rewardIconAreaList[0].transform, null);
                                break;
                            case RewardPackageIconSize.ONE_BY_TWO:
                                go = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Shop Rewards Item Web Image", rewardIconAreaList[2].transform, null);
                                break;
                        }

                        if(go != null)
                        {
                            ContextElement element = go.GetComponent<ContextElement>();
                            IContextImage imageElement = element as IContextImage;

                            imageElement.SetHash(iconURL.GetHashCode().ToString());

                            WebImageDownloader.Instance.LoadWebImage(
                                iconURL,
                                CacheType.FileCache,
                                true,
                                RewardImageLoadSuccess,
                                delegate(Sprite img)
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
                    if(rewardViewInfoListBB != null && rewardViewInfoListBB.Count > 0)
                    {
                        rewardIconList = new List<GameObject>();
                        var rewardListBB = productBB.GetValue<List<Blackboard>>("rewardList");

                        int makeCount = 0;

                        for(int i=0; i<rewardViewInfoListBB.Count; ++i)
                        {
                            var rewardIndex = rewardViewInfoListBB[i].GetValue<int>("rewardIndex");
                            var quantity = rewardViewInfoListBB[i].GetValue<long>("quantity");

                            var go = MakeRewardIcon(rewardListBB[rewardIndex], rewardIconAreaList[makeCount].transform, null, i);

                            if(go != null)
                            {
                                SetIconValues(go, rewardListBB[rewardIndex]);
                                SetQuantity(go, quantity);

                                rewardIconList.Add(go);
                                makeCount++;
                                if(makeCount >= 2)
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
            var priceSectionList = BlackboardUtils.FindVariable<List<double>>(MainBlackboard.Get(), "values/misc/SHOP_GEM_ICON_SECTION_LIST")?.value;
            int grade = priceSectionList.IndexOfRange(price) + 1;
            string asset = "Shop Icon Gem Lv " + grade.ToString();

            var imageAnchor = ContextUtils.FindElement(rootElement, "Image Anchor", ContextSearchingType.FullNameSearch);
            Transform parent = imageAnchor.transform;

            MetaObjectUtils.MakePrefab(bundle, asset, parent);
        }

        private void UpdateInflationValues()
        {
            itemWasTextElement.gameObject.SetActive(true);
            MetaContextElementUtils.SetText(itemTextElement, inflationItemText);
            MetaContextElementUtils.SetText(itemWasTextElement, inflationItemWasText);
            MetaContextElementUtils.SetText(inflationMultiplierTextElement, inflationEventText);
        }

        private void UpdateEventValues()
        {
            itemWasTextElement.gameObject.SetActive(true);
            MetaContextElementUtils.SetText(itemTextElement, totalItemText);
            MetaContextElementUtils.SetText(itemWasTextElement, totalItemWasText);
            MetaContextElementUtils.SetText(eventMultiplierTextElement, totalEventText);
        }

        private void UpdateTags()
        {
            // Update Tag.
            rootAnimator.SetBool("TierUp", isTierUp && !showFlipEffect);
            rootAnimator.SetBool("MostPopular", false);
            rootAnimator.SetBool("BestValue", false);
            rootAnimator.SetBool("BaseEvent", false);

            if(!isTierUp && !showFlipEffect)
            {
                ProductTagType tagType = productBB.GetValue<ProductTagType>("tagType");

                rootAnimator.SetBool("MostPopular", tagType == ProductTagType.POPULAR);
                rootAnimator.SetBool("BestValue", tagType == ProductTagType.BEST);
                rootAnimator.SetBool("BaseEvent", (tagType != ProductTagType.NONE && tagType != ProductTagType.UNKNOWN) );
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
            if(isInflation)
            {
                yield return new WaitForSeconds(flipEffectInterval);

                rootAnimator.SetBool("IsInflation", true);
                rootAnimator.SetBool("IsEvent", false);
                rootAnimator.SetBool("IsEventSkip", false);

                yield return new WaitForSeconds(0.2f);

                UpdateInflationValues();
            }

            if(isEvent)
            {
                if(isInflation)
                    yield return new WaitForSeconds(inflationEffectInterval);
                else
                    yield return new WaitForSeconds(flipEffectInterval);

                rootAnimator.SetBool("IsInflation", false);
                rootAnimator.SetBool("IsEvent", true);
                rootAnimator.SetBool("IsEventSkip", false);

                yield return new WaitForSeconds(0.2f);

                UpdateEventValues();
            }

            UpdateTags();

            updateMutex = false;
        }

        private EventInfo GetPassiveEventMultiplierNumerator()
        {
            return PassiveEventUtils.GetPassiveEvent(ShopType.GEM);
        }
        
        public void StartPassiveEvent()
        {
            if(updateMutex) return;

            var eventInfo = GetPassiveEventMultiplierNumerator();
            if(eventInfo != null && eventInfo.id != eventID)
            {
                eventID = eventInfo.id;
                showFlipEffect = true;
                UpdateValues();
            }
        }
    }

}

