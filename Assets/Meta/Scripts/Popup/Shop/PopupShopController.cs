using System.Collections.Generic;
using UnityEngine;
using SlotMaker;
using NodeCanvas.Framework;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupShopController : MonoBehaviour
    {
        public Blackboard rootBlackboard;
        public Animator rootAnimator;

        public bool combineApplicationType;
        public string biContextID;

        private ContextElement shopListElement;
        private ContextElement dailyBoostElement;
        private List<ContextElement> shopCoinItemElementList;
        private List<ContextElement> shopGemItemElementList;

        private ContextElement tierIconElement;

        private ContextElement toggleGroupElement;
        private ContextElement shopCoinImageElement;
        private ContextElement shopGemImageElement;

        private ContextElement vegasBucksElement;
        private ContextElement vegasBucksTextElement;
        private ContextElement vegasBucksLoading;

        private bool isInit = false;
        private bool isLoadedCoinShopImage = false;
        private bool isLoadedGemShopImage = false;

        private Blackboard coinShopBB;
        private Blackboard gemShopBB;

        private int meTier;
        private int currentTabIndex = 0;

        public float defaultDelay = 0.5f;
        public float interval = 0.1f;

        private bool coinFlipEffectEnalbed;
        private bool gemFlipEffectEnalbed;
        private bool coinLevelMultiplyFlipEffectEnalbed;
        private bool gemLevelMultiplyFlipEffectEnalbed;

        private Variable<string>[] imageUrlVariables;

        private int coinShopID = 0;
        private int gemShopID = 0;
        private bool isSendCoinStoreBI = false;
        private bool isSendGemStoreBI = false;
        // Bucks
        private bool IsUserBucks { get { return GetUserBucks() > 0L; } }
        private bool IsActiveBucksUI
        {
            get
            {
                bool isEnableUI = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_BUCKS_SHOP_UI")?.value ?? false;
                return isEnableUI && IsUserBucks && GetAvailableBucks();
            }
        }

        private const string SHOP_COIN_FLIP_ENABLED_KEY = "ShopCoinFlipEnabled";
        private const string SHOP_GEM_FLIP_ENABLED_KEY = "ShopGemFlipEnabled";
        private const string SHOP_COIN_LEVEL_MULTIPLY_FLIP_ENABLED_KEY = "ShopCoinLevelMultiplyFlipEnabled";
        private const string SHOP_OPENED_FROM_TYPE = "SHOP_OPENED_FROM_TYPE";
        private const string TYPE_DEFAULT = "default";

        private const int COIN_TAB_INDEX = 0;
        private const int GEM_TAB_INDEX = 1;

        public void OnInitShop(ShopType coinShopType, ShopType gemShopType, int initTabIndex)
        {
            if (isInit) return;

            currentTabIndex = initTabIndex;

            coinShopBB = BlackboardQueryUtils.GetShopBB(coinShopType);
            gemShopBB = BlackboardQueryUtils.GetShopBB(gemShopType);

            if (coinShopBB != null)
                coinShopID = coinShopBB.GetValue<int>("id");

            if (gemShopBB != null)
                gemShopID = gemShopBB.GetValue<int>("id");

            rootBlackboard = gameObject.GetComponent<Blackboard>();
            rootAnimator = gameObject.GetComponent<Animator>();

            // Init context.
            var rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            shopListElement = ContextUtils.FindElement(rootElement, "Shop List", ContextSearchingType.ChildrenSearch);

            var vipBonusElement = ContextUtils.FindElement(rootElement, "VIP Bonus", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetBlackboardValue<GameObject>(vipBonusElement, "rootElement", gameObject);
            MetaContextElementUtils.SetBlackboardValue<string>(vipBonusElement, "_biContextID", GetBIContextID());

            dailyBoostElement = ContextUtils.FindElement(rootElement, "Daily Boost Item", ContextSearchingType.ChildrenSearch);

            var closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);
            MetaContextElementUtils.SetClickable(closeButtonElement, "OnCloseShop", rootElement, null);

            tierIconElement = ContextUtils.FindElement(rootElement, "Tier Bonus Area/Icon Tier Multiplier", ContextSearchingType.FullNameSearch);
            MetaContextElementUtils.SetBlackboardValue(tierIconElement, "multiplierType", TierMultiplierTableType.CoinMultiplier);

            toggleGroupElement = ContextUtils.FindElement(rootElement, "Tab Area", ContextSearchingType.ChildrenSearch);
            IContextClickable tabClickableElement = toggleGroupElement as IContextClickable;
            if (tabClickableElement != null)
            {
                tabClickableElement.RemoveAllListener();
                tabClickableElement.AddListenerOnClick((ContextElement s) =>
               {
                   OnSelectTab(MetaContextElementUtils.GetIntProperty(toggleGroupElement));
               });
            }

            vegasBucksElement = ContextUtils.FindElement(rootElement, "Vegas Bucks", ContextSearchingType.ChildrenSearch);
            vegasBucksTextElement = ContextUtils.FindElement(vegasBucksElement, "Text", ContextSearchingType.ChildrenSearch);
            vegasBucksLoading = ContextUtils.FindElement(vegasBucksElement, "Loading", ContextSearchingType.ChildrenSearch);

            shopCoinImageElement = ContextUtils.FindElement(rootElement, "Shop Image Coin", ContextSearchingType.ChildrenSearch);
            shopGemImageElement = ContextUtils.FindElement(rootElement, "Shop Image Gem", ContextSearchingType.ChildrenSearch);

            shopCoinItemElementList = new List<ContextElement>();
            shopGemItemElementList = new List<ContextElement>();

            //EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.COLLECTING_GAME);
            List<EventInfo> listEventInfo = BlackboardQueryUtils.GetActiveEventInfoList(new List<EventInfoType>(
                new EventInfoType[] { EventInfoType.COLLECTING_GAME, EventInfoType.BOSS_RAIDERS, EventInfoType.CLUB_ARENA }));
            bool isLockedFeature = BlackboardQueryUtils.IsLockedFeature(LockedFeatureType.META_GAME);

            int metaGameEventId = 0;

            if (!isLockedFeature && listEventInfo != null && listEventInfo.Count > 0)
                metaGameEventId = listEventInfo[0].id;
            bool isAvailableBucks = GetAvailableBucks();
            for (int i = 0; i < 6; ++i)
            {
                shopCoinItemElementList.Add(ContextUtils.FindElement(shopListElement, string.Format("Shop Item {0}", i + 1), ContextSearchingType.ChildrenSearch));
                MetaContextElementUtils.SetBlackboardValue<string>(shopCoinItemElementList[i], "_biContextID", GetBIContextID());
                MetaContextElementUtils.SetBlackboardValue<int>(shopCoinItemElementList[i], "_metaGameEventID", metaGameEventId);
                MetaContextElementUtils.SetBlackboardValue<bool>(shopCoinItemElementList[i], "_isAvailableBucks", isAvailableBucks);
                shopCoinItemElementList[i].gameObject.SetActive(false);

                // Gem Item Is Generate
                if (gemShopBB != null)
                {
                    var gemObj = MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Shop Item Gem", shopCoinItemElementList[i].transform.parent, null, string.Format("Shop Item Gem {0}", i + 1));
                    MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Image Wheel Shop Item", gemObj.transform, "Anchor/Wheel", null);
                    MetaObjectUtils.MakePrefab(MetaStringDefine.LOBBY_BUNDLE_NAME, "Button Shop", gemObj.transform, "Anchor/Button Area", "Buy");
                    shopGemItemElementList.Add(gemObj.GetComponent<ContextElement>());

                    MetaContextElementUtils.SetBlackboardValue<string>(shopGemItemElementList[i], "_biContextID", GetBIContextID());
                    MetaContextElementUtils.SetBlackboardValue<int>(shopGemItemElementList[i], "_metaGameEventID", metaGameEventId);
                    MetaContextElementUtils.SetBlackboardValue<bool>(shopGemItemElementList[i], "_isAvailableBucks", isAvailableBucks);

                    shopGemItemElementList[i].gameObject.SetActive(false);
                }
            }

            MetaContextElementUtils.SetBlackboardValue<string>(dailyBoostElement, "_biContextID", GetBIContextID());
            MetaContextElementUtils.SetBlackboardValue<int>(dailyBoostElement, "_metaGameEventID", metaGameEventId);

            var coinFlipEffect = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), SHOP_COIN_FLIP_ENABLED_KEY);
            if (coinFlipEffect == null)
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), SHOP_COIN_FLIP_ENABLED_KEY, true);

            coinFlipEffectEnalbed = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), SHOP_COIN_FLIP_ENABLED_KEY).value;

            var gemFlipEffect = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), SHOP_GEM_FLIP_ENABLED_KEY);
            if (gemFlipEffect == null)
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), SHOP_GEM_FLIP_ENABLED_KEY, true);

            gemFlipEffectEnalbed = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), SHOP_GEM_FLIP_ENABLED_KEY).value;

            ContextElement gemTab = ContextUtils.FindElement(rootElement, "Tab Area/Tab Gem", ContextSearchingType.FullNameSearch);
            ContextElement gemTabComingSoon = ContextUtils.FindElement(rootElement, "Tab Area/Tab Gem Dummy", ContextSearchingType.FullNameSearch);

            gemTabComingSoon.gameObject.SetActive(gemShopBB == null);
            gemTab.gameObject.SetActive(gemShopBB != null);

            OnInitLevelMultiply();

            isInit = true;
        }

        public void OnUpdateVariables()
        {
            OnSelectTab(currentTabIndex);
            AssignImageUrlVariables();

            MetaContextElementUtils.SetBlackboardValue(tierIconElement, "isRefresh", true);

            if (!isLoadedCoinShopImage)
            {
                if (imageUrlVariables[COIN_TAB_INDEX] != null && !string.IsNullOrEmpty(imageUrlVariables[COIN_TAB_INDEX].value))
                    LoadImageFromURL(COIN_TAB_INDEX);
            }

            if (!isLoadedGemShopImage)
            {
                if (imageUrlVariables[GEM_TAB_INDEX] != null && !string.IsNullOrEmpty(imageUrlVariables[GEM_TAB_INDEX].value))
                    LoadImageFromURL(GEM_TAB_INDEX);
            }
            UpdateVegasBucks();
        }

        private void AssignImageUrlVariables()
        {
            if (imageUrlVariables == null)
            {
                imageUrlVariables = new Variable<string>[2];
                imageUrlVariables[COIN_TAB_INDEX] = coinShopBB.GetVariable<string>("imageUrl");
                if (gemShopBB != null)
                {
                    imageUrlVariables[GEM_TAB_INDEX] = gemShopBB.GetVariable<string>("imageUrl");
                }
            }
        }

        private void LoadImageFromURL(int tabIndex)
        {
            string imageURL = (tabIndex == COIN_TAB_INDEX) ? coinShopBB.GetValue<string>("imageUrl") : gemShopBB.GetValue<string>("imageUrl");
            var imageElement = (tabIndex == COIN_TAB_INDEX) ? shopCoinImageElement as IContextImage : shopGemImageElement as IContextImage;

            System.Action<string> loadSuccessOperation = CoinShopImageLoadSuccess;
            if (tabIndex == GEM_TAB_INDEX)
                loadSuccessOperation = GemShopImageLoadSuccess;

            imageElement.SetHash(imageURL.GetHashCode().ToString());

            WebImageDownloader.Instance.LoadWebImage(
                imageURL,
                CacheType.FileCache,
                true,
                loadSuccessOperation,
                delegate (Sprite img)
                {
                    if(imageElement == null) return;

                    if (img != null)
                    {
                        if (imageElement.CheckHash(imageURL.GetHashCode().ToString()))
                        {
                            imageElement.SetSprite(img);

                            MetaContextElementUtils.SetActive(imageElement as ContextElement, currentTabIndex == tabIndex && !IsActiveBucksUI);
                        }
                    }
                },
                null,
                ShopImageLoadFailed
            );
        }

        private void UpdateCoinShop()
        {
            if (coinShopBB != null)
            {
                var productGroupList = coinShopBB.GetValue<List<Blackboard>>("productGroupList");

                double lowPrice = double.MaxValue;
                ContextElement tierUpElement = null;

                var meAccRP = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/accRp");
                meTier = TierUtils.GetTier(meAccRP.value);
                int minCount = Mathf.Min(shopCoinItemElementList.Count, productGroupList.Count);
                for (int i = 0; i < minCount;++i) //shopCoinItemElementList.Count; ++i)
                {
                    shopCoinItemElementList[i].gameObject.SetActive(true);

                    var productList = BlackboardQueryUtils.GetProductList(productGroupList[i]);
                    MetaContextElementUtils.SetBlackboardValue<List<Blackboard>>(shopCoinItemElementList[i], "productList", productList);

                    MetaContextElementUtils.SetBlackboardValue<GameObject>(shopCoinItemElementList[i], "parentShop", gameObject);

                    MetaContextElementUtils.SetBlackboardValue<bool>(shopCoinItemElementList[i], "isTierUp", false);
                    // MetaContextElementUtils.SetBlackboardValue<bool>(shopCoinItemElementList[i], "isEffect", isCoinShopEvent);

                    MetaContextElementUtils.SetBlackboardValue<bool>(shopCoinItemElementList[i], "showFlipEffect", coinFlipEffectEnalbed);
                    MetaContextElementUtils.SetBlackboardValue<float>(shopCoinItemElementList[i], "flipEffectInterval", defaultDelay + (interval * i));
                    MetaContextElementUtils.SetBlackboardValue<bool>(shopCoinItemElementList[i], "showLevelMultiplyFlipEffect", coinLevelMultiplyFlipEffectEnalbed);

                    if (IsTierUp(productList[0], meTier, ItemType.CREDIT))
                    {
                        var price = productList[0].GetValue<double>("price");
                        if (price < lowPrice)
                        {
                            lowPrice = price;
                            tierUpElement = shopCoinItemElementList[i];
                        }
                    }

                    if (!shopCoinItemElementList[i].gameObject.GetComponent<GraphOwner>().isRunning)
                        shopCoinItemElementList[i].gameObject.GetComponent<GraphOwner>().StartBehaviour();
                    MetaContextElementUtils.SendEvent(shopCoinItemElementList[i], "OnUpdate", null, null);
                }

                if (tierUpElement != null)
                    MetaContextElementUtils.SetBlackboardValue<bool>(tierUpElement, "isTierUp", true);
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), SHOP_COIN_FLIP_ENABLED_KEY, false);
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), SHOP_COIN_LEVEL_MULTIPLY_FLIP_ENABLED_KEY, false);

                if (shopCoinImageElement != null)
                {
                    MetaContextElementUtils.SetActive(shopCoinImageElement as ContextElement, !IsActiveBucksUI);
                    UpdateVegasBucks();
                }
                if (shopGemImageElement != null)
                    MetaContextElementUtils.SetActive(shopGemImageElement as ContextElement, false);
            }

            for (int i = 0; i < shopGemItemElementList.Count; ++i)
            {
                shopGemItemElementList[i].gameObject.SetActive(false);
            }
        }

        private void UpdateGemShop()
        {
            if (gemShopBB != null)
            {
                var productGroupList = gemShopBB.GetValue<List<Blackboard>>("productGroupList");

                double lowPrice = double.MaxValue;
                ContextElement tierUpElement = null;

                var meAccRP = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/accRp");
                meTier = TierUtils.GetTier(meAccRP.value);

                for (int i = 0; i < shopGemItemElementList.Count; ++i)
                {
                    shopGemItemElementList[i].gameObject.SetActive(true);

                    var productList = BlackboardQueryUtils.GetProductList(productGroupList[i]);
                    MetaContextElementUtils.SetBlackboardValue<List<Blackboard>>(shopGemItemElementList[i], "productList", productList);

                    MetaContextElementUtils.SetBlackboardValue<GameObject>(shopGemItemElementList[i], "parentShop", gameObject);

                    MetaContextElementUtils.SetBlackboardValue<bool>(shopGemItemElementList[i], "isTierUp", false);

                    MetaContextElementUtils.SetBlackboardValue<bool>(shopGemItemElementList[i], "showFlipEffect", gemFlipEffectEnalbed);
                    MetaContextElementUtils.SetBlackboardValue<float>(shopGemItemElementList[i], "flipEffectInterval", defaultDelay + (interval * i));
                    MetaContextElementUtils.SetBlackboardValue<bool>(shopGemItemElementList[i], "showLevelMultiplyFlipEffect", gemLevelMultiplyFlipEffectEnalbed);

                    if (IsTierUp(productList[0], meTier, ItemType.GEM))
                    {
                        var price = productList[0].GetValue<double>("price");
                        if (price < lowPrice)
                        {
                            lowPrice = price;
                            tierUpElement = shopGemItemElementList[i];
                        }
                    }

                    if (!shopCoinItemElementList[i].gameObject.GetComponent<GraphOwner>().isRunning)
                        shopGemItemElementList[i].gameObject.GetComponent<GraphOwner>().StartBehaviour();
                    MetaContextElementUtils.SendEvent(shopGemItemElementList[i], "OnUpdate", null, null);

                    if (shopCoinImageElement != null)
                        MetaContextElementUtils.SetActive(shopCoinImageElement as ContextElement, false);
                    if (shopGemImageElement != null)
                    {
                        MetaContextElementUtils.SetActive(shopGemImageElement as ContextElement, !IsActiveBucksUI);
                        UpdateVegasBucks();
                    }
                }

                if (tierUpElement != null)
                    MetaContextElementUtils.SetBlackboardValue<bool>(tierUpElement, "isTierUp", true);

                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), SHOP_GEM_FLIP_ENABLED_KEY, false);
            }

            for (int i = 0; i < shopCoinItemElementList.Count; ++i)
            {
                shopCoinItemElementList[i].gameObject.SetActive(false);
            }
        }

        private bool IsTierUp(Blackboard productBB, int targetTier, ItemType itemType)
        {
            var itemBB = BlackboardQueryUtils.GetItemFromProduct(productBB, itemType);

            if (itemBB != null)
            {
                var rp = itemBB.GetValue<long>("rp");
                var meAccRP = BlackboardUtils.FindVariable<long>(MainBlackboard.Get(), "me/accRp");
                long expectAccRP = meAccRP.value + rp;
                int expectTier = TierUtils.GetTier(expectAccRP);

                return expectTier > targetTier;
            }

            return false;
        }

        public string InitBIContextID(string contextID)
        {
            if (string.IsNullOrEmpty(contextID))
            {
                if (string.IsNullOrEmpty(BlackboardQueryUtils.GetShopBiContextId()))
                    contextID = BiEventUtils.GenerateContextID();
                else
                    contextID = BlackboardQueryUtils.GetShopBiContextId();
            }
            BlackboardQueryUtils.SetShopBiContextId("");

            biContextID = contextID;

            return biContextID;
        }

        private string GetBIContextID()
        {
            if (string.IsNullOrEmpty(biContextID))
                biContextID = BiEventUtils.GenerateContextID();

            return biContextID;
        }

        public void OnSelectTab(int tabIndex)
        {
            if (gemShopBB == null)
                tabIndex = 0;

            MetaContextElementUtils.SetIntProperty(toggleGroupElement, tabIndex);
            currentTabIndex = tabIndex;

            if (tabIndex == 1)
            {
                if (!isSendGemStoreBI)
                {
                    BI_client_store_opened(ShopType.GEM, gemShopID);
                    isSendGemStoreBI = true;
                }
                UpdateGemShop();
            }
            else
            {
                if (!isSendCoinStoreBI)
                {
                    BI_client_store_opened(ShopType.COIN, coinShopID);
                    isSendCoinStoreBI = true;
                }
                UpdateCoinShop();
            }
        }

        public void OnRefreshTier()
        {

        }

        public void OnStartPassiveEvent()
        {

        }

        public void OnRefreshPassiveEvent()
        {

        }

        private void CoinShopImageLoadSuccess(string url)
        {
            // loading False.
            // Set Image.
            isLoadedCoinShopImage = true;
        }

        private void GemShopImageLoadSuccess(string url)
        {
            // loading False.
            // Set Image.
            isLoadedGemShopImage = true;
        }

        private void ShopImageLoadFailed(WebImageDownloader.WebImageDownloadError error)
        {
            // Refresh??
        }

        private void BI_client_store_opened(ShopType shopType, int shopID)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["shop_type"] = shopType.ToString();
            customData["shop_id"] = shopID;
            customData["context_id"] = GetBIContextID();

            var eventInfo = PassiveEventUtils.GetPassiveEvent(shopType);
            customData["shop_event_flag"] = eventInfo == null ? false : true;

            if (PlayerPrefs.HasKey(SHOP_OPENED_FROM_TYPE))
            {
                string openType = PlayerPrefs.GetString(SHOP_OPENED_FROM_TYPE);

                if (!string.IsNullOrEmpty(openType))
                    customData["open_type"] = openType;
                else
                    customData["open_type"] = TYPE_DEFAULT;

                PlayerPrefs.DeleteKey(SHOP_OPENED_FROM_TYPE);
            }
            else
            {
                customData["open_type"] = TYPE_DEFAULT;
            }
            BiEventUtils.AppendLevelMultiplierEventData(customData, "coin");
            Analytics.CustomEvent("client_store_opened", customData);
            AdjustManager.Instance.SendEvent("store_opened");
        }

        public bool OnCheckHasPreBbbReward()
        {
            var isHasPreBbbReward = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), "hasPreBbbReward");

            OnCreateHasPreBbbRewardPopup(isHasPreBbbReward.value);

            return isHasPreBbbReward.value;
        }

        public void OnCreateHasPreBbbRewardPopup(bool isHasPreBbbReward)
        {
            if (!isHasPreBbbReward)
                return;

            Transform parent = GameObject.Find("Popup Manager/Area").transform;
            ParadoxNotion.Services.MonoManager.current.StartCoroutine(SceneUtils.LoadSceneAsync(MetaStringDefine.LOBBY_BUNDLE_NAME, "Popup Pre Whale Bonus Scene", parent, true,
                (result) =>
                {
                    PopupManager.Instance.Open(result);
                    GameObject saveAs = result;

                    saveAs.gameObject.SetActive(true);
                }
            ));
        }

        private void OnInitLevelMultiply()
        {
            bool isLevelMultiplier = LevelUtils.GetCurrentLevelMultiplierNumerator("coin") > 0L;

            var coinLevelMultiplyFlipEffect = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), SHOP_COIN_LEVEL_MULTIPLY_FLIP_ENABLED_KEY);
            if (coinLevelMultiplyFlipEffect == null)
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), SHOP_COIN_LEVEL_MULTIPLY_FLIP_ENABLED_KEY, isLevelMultiplier);
            coinLevelMultiplyFlipEffectEnalbed = BlackboardUtils.GetOrCreateVariable<bool>(MainBlackboard.Get(), SHOP_COIN_LEVEL_MULTIPLY_FLIP_ENABLED_KEY).value;
        }
        // Vegas Bucks
        private long GetUserBucks()
        {
            return BlackboardQueryUtils.GetUserTotalBucks();
        }

        private bool GetAvailableBucks()
        {
            if (rootBlackboard == null)
                return false;
            return BlackboardUtils.FindVariable<bool>(rootBlackboard, "_isAvailableBucks")?.value ?? false;
        }

        public void UpdateVegasBucks()
        {
            if (vegasBucksElement == null) return;
            long userBucks = GetUserBucks();

            bool isActive = IsActiveBucksUI;
            vegasBucksElement.gameObject.SetActive(isActive);
            shopCoinImageElement?.gameObject.SetActive(!isActive && currentTabIndex == COIN_TAB_INDEX);
            shopGemImageElement?.gameObject.SetActive(!isActive && currentTabIndex == GEM_TAB_INDEX);
            if (vegasBucksTextElement != null)
                MetaContextElementUtils.SetText(vegasBucksTextElement, string.Format("{0:#,##0.00}", userBucks * 0.01f));
        }

        public System.Collections.IEnumerator RequestBucksAmount()
        {
            if (GetAvailableBucks())
            {
                bool isSuccessOrFailure = false;
                BagelCodeClientAPI.RequestBucksAmount(
                    (response) =>
                    {
                        BlackboardQueryUtils.UpdateUserBucks(response.userBucks);
                        isSuccessOrFailure = true;
                    },
                    (error) =>
                    {
                        Debug.Log("Error : " + error.errorCode.ToString());
                        isSuccessOrFailure = true;
                    });
                yield return new WaitUntil(() => isSuccessOrFailure);
                UpdateVegasBucks();
            }
        }
    }
}
