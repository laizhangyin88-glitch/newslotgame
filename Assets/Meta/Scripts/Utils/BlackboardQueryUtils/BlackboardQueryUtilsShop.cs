using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{

    public static partial class BlackboardQueryUtils
    {
        private static Blackboard GetShopLogic(ShopType targetShopType)
        {
            var shopList = BlackboardUtils.FindVariable<List<Blackboard>>(MainBlackboard.Get(), "shopList");
            if (shopList.value != null)
            {
                Blackboard defaultShopBB = null;

                long activeShopEndTimestamp = System.Int64.MaxValue;
                Blackboard activeShopBB = null;

                long currentTimestamp = TimeUtils.GetTimeStamp();

                for (int i = 0; i < shopList.value.Count; ++i)
                {
                    ShopType shopType = BlackboardUtils.FindVariable<ShopType>(shopList.value[i], "type").value;

                    if (shopType == targetShopType)
                    {
                        bool isDefault = shopList.value[i].GetValue<bool>("isDefault");
                        if (isDefault)
                        {
                            defaultShopBB = shopList.value[i];
                        }
                        else
                        {
                            long startTimestamp = shopList.value[i].GetValue<long>("startTimestamp");
                            long endTimestamp = shopList.value[i].GetValue<long>("endTimestamp");

                            if (startTimestamp <= currentTimestamp && currentTimestamp < endTimestamp)
                            {
                                if (activeShopEndTimestamp > endTimestamp)
                                {
                                    activeShopEndTimestamp = endTimestamp;
                                    activeShopBB = shopList.value[i];
                                }
                            }
                        }
                    }
                }

                if (activeShopBB != null) return activeShopBB;
                if (defaultShopBB != null) return defaultShopBB;
            }

            return null;
        }

        public static Blackboard GetShopBB(ShopType targetShopType)
        {
            Blackboard preferredShopBB = null;

            switch(targetShopType)
            {
                case ShopType.COIN_WITH_BOSS_RAIDERS:
                case ShopType.COIN_WITH_CLUB_ARENA:
                case ShopType.COIN_WITH_HIDDEN_UNIVERSE:
                case ShopType.COIN_WITH_META_GAME:
                case ShopType.COIN_WITH_BUILD_DREAM:
                    // return GetShopLogic(targetShopType) ?? GetShopBB(ShopType.COIN);
                    preferredShopBB = GetShopLogic(targetShopType);
                    if(preferredShopBB != null) return preferredShopBB;
                    else return GetShopBB(ShopType.COIN);

                case ShopType.GEM_WITH_BOSS_RAIDERS:
                case ShopType.GEM_WITH_CLUB_ARENA:
                case ShopType.GEM_WITH_HIDDEN_UNIVERSE:
                case ShopType.GEM_WITH_META_GAME:
                case ShopType.GEM_WITH_BUILD_DREAM:
                    // return GetShopLogic(targetShopType) ?? GetShopBB(ShopType.GEM);
                    preferredShopBB = GetShopLogic(targetShopType);
                    if (preferredShopBB != null) return preferredShopBB;
                    else return GetShopBB(ShopType.GEM);

                case ShopType.COIN: // Check Meta Shop
                    if(PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.COLLECTING_GAME) != null)
                    {
                        preferredShopBB = GetShopLogic(ShopType.COIN_WITH_META_GAME);
                    }
                    else if(PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BOSS_RAIDERS) != null)
                    {
                        preferredShopBB = GetShopLogic(ShopType.COIN_WITH_BOSS_RAIDERS);
                    }
                    else if(PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CLUB_ARENA) != null)
                    {
                        preferredShopBB = GetShopLogic(ShopType.COIN_WITH_CLUB_ARENA);
                    }
                    if (preferredShopBB != null) return preferredShopBB;
                    else return GetShopLogic(ShopType.COIN);

                case ShopType.GEM: // Check Meta Shop
                    if (PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.COLLECTING_GAME) != null)
                    {
                        preferredShopBB = GetShopLogic(ShopType.GEM_WITH_META_GAME);
                    }
                    else if (PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.BOSS_RAIDERS) != null)
                    {
                        preferredShopBB = GetShopLogic(ShopType.GEM_WITH_BOSS_RAIDERS);
                    }
                    else if (PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.CLUB_ARENA) != null)
                    {
                        preferredShopBB = GetShopLogic(ShopType.GEM_WITH_CLUB_ARENA);
                    }
                    if (preferredShopBB != null) return preferredShopBB;
                    else return GetShopLogic(ShopType.GEM);
            }

            return GetShopLogic(targetShopType);
        }

        // Shop Product Item List..
        public static List<Blackboard> GetShopProductGroups(ShopType targetShopType)
        {
            Blackboard targetShopBB = GetShopBB(targetShopType);

            return GetShopProductGroups(targetShopBB);
        }

        public static List<Blackboard> GetShopProductGroups(Blackboard targetShopBB)
        {
            if (targetShopBB != null)
            {
                return BlackboardUtils.FindVariable<List<Blackboard>>(targetShopBB, "productGroupList").value;
            }

            return null;
        }

        // Product List.
        public static List<Blackboard> GetProductList(Blackboard productGroup)
        {
            if (productGroup != null)
            {
                return BlackboardUtils.FindVariable<List<Blackboard>>(productGroup, "productList").value;
            }

            return null;
        }

        // First Target Item. 
        public static Blackboard GetItemFromProduct(Blackboard product, ItemType targetItemType)
        {
            var itemList = BlackboardUtils.FindVariable<List<Blackboard>>(product, "itemList");

            if (itemList.value != null)
            {
                for (int i = 0; i < itemList.value.Count; ++i)
                {
                    ItemType itemType = BlackboardUtils.FindVariable<ItemType>(itemList.value[i], "itemType").value;

                    if (itemType == targetItemType)
                    {
                        return itemList.value[i];
                    }
                }
            }

            return null;
        }

        public static Blackboard GetItemFromProduct(Blackboard product, int index)
        {
            var itemList = BlackboardUtils.FindVariable<List<Blackboard>>(product, "itemList");

            if (itemList.value != null)
            {
                if (itemList.value.Count > index)
                {
                    return itemList.value[index];
                }
            }

            return null;
        }

        public static int GetProductSalePercent(Blackboard productBB)
        {
            if (productBB != null)
            {
                double price = BlackboardUtils.FindVariable<double>(productBB, "price").value;
                double origPrice = BlackboardUtils.FindVariable<double>(productBB, "originalPrice").value;

                if (price < origPrice)
                    return System.Convert.ToInt32(((1.0 - (price / origPrice)) * 100.0));
            }

            return 0;
        }

        public static bool CheckPurchaseProhibitedRegion()
        {
            var isPurchaseProhibitedRegion = BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "isPurchaseProhibitedRegion");

            if (isPurchaseProhibitedRegion != null && isPurchaseProhibitedRegion.value == true)
            {
                ErrorPopupInfo info = new ErrorPopupInfo();

                string supportUrl = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "values/misc/SUPPORT_PAGE_URL").value;

                bool stringError = false;
                info.type = ErrorPopupType.OK;
                info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_PURCHASE_PROHIBITED_REGION", supportUrl, out stringError);
                info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                ErrorPopupHandler.Instance.OpenError(info);

                Analytics.CustomEvent("client_purchase_prohibited_region", null);
                return true;
            }
            return false;
        }

        public static Blackboard GetPriceMatchProduct(double targetPrice, ShopType shopType, ItemType itemType)
        {
            Blackboard shopBB = GetShopBB(shopType);

            if (shopBB != null)
            {
                List<Blackboard> productGroupList = shopBB.GetValue<List<Blackboard>>("productGroupList");

                if (productGroupList != null)
                {
                    for (int i = 0; i < productGroupList.Count; ++i)
                    {
                        List<Blackboard> productList = GetProductList(productGroupList[i]);
                        for (int k = 0; k < productList.Count; ++k)
                        {
                            double originalPrice = productList[k].GetValue<double>("originalPrice");

                            if ((int)originalPrice == (int)targetPrice)
                            {
                                var itemBB = GetItemFromProduct(productList[k], itemType);
                                if (itemBB != null)
                                    return productList[k];
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static Blackboard GetPriceMatchProduct(double targetPrice, ShopType shopType, ItemType itemType, out Blackboard outShopBB)
        {
            outShopBB = GetShopBB(shopType);

            if (outShopBB != null)
            {
                List<Blackboard> productGroupList = outShopBB.GetValue<List<Blackboard>>("productGroupList");

                if (productGroupList != null)
                {
                    for (int i = 0; i < productGroupList.Count; ++i)
                    {
                        List<Blackboard> productList = GetProductList(productGroupList[i]);
                        for (int k = 0; k < productList.Count; ++k)
                        {
                            double originalPrice = productList[k].GetValue<double>("originalPrice");

                            if ((int)originalPrice == (int)targetPrice)
                            {
                                var itemBB = GetItemFromProduct(productList[k], itemType);
                                if (itemBB != null)
                                    return productList[k];
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static Blackboard GetProductFromID(int targetProductID, ShopType type = ShopType.UNKNOWN)
        {
            if (type == ShopType.UNKNOWN)
            {
                var enumNumbers = System.Enum.GetValues(typeof(ShopType));
                foreach (int t in enumNumbers)
                {
                    var bb = GetProductFromID(targetProductID, (ShopType)t);
                    if (bb != null)
                        return bb;
                }
            }
            else
            {
                var productGroups = GetShopProductGroups(type);
                if (productGroups != null)
                {
                    for (int i = 0; i < productGroups.Count; ++i)
                    {
                        var productList = GetProductList(productGroups[i]);
                        if (productList != null)
                        {
                            for (int k = 0; k < productList.Count; ++k)
                            {
                                var productID = productList[k].GetValue<int>("id");
                                if (productID == targetProductID)
                                    return productList[k];
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static void GetCoinProductPredictCoin(Blackboard productBB, out long baseCoin, out long totalItemCoins, out long totalEventCoins)
        {
            //long eventMultiplierNumerator = productBB.GetValue<long>("eventMultiplierNumerator");

            var coinItemBB = GetItemFromProduct(productBB, ItemType.CREDIT);
            long baseCredit = coinItemBB.GetValue<long>("baseCredit");

            long additionalCreditMultiplierNumerator = coinItemBB.GetValue<long>("additionalCreditMultiplierNumerator");

            int purchaseMultiplierPercent = System.Convert.ToInt32(additionalCreditMultiplierNumerator / NumberUtils.GetGlobalDenominator() * 100L);
            long purchaseMultipliedBaseCredit = NumberUtils.GetAdditionalMultiplierNumeratorValue(baseCredit, additionalCreditMultiplierNumerator);

            int tier = TierUtils.GetMeTier();
            double tierMultiplier = TierUtils.GetTierMultiplier(tier);
            baseCoin = TierUtils.GetTierFractionCoin(baseCredit, tier);
            baseCoin = LevelUtils.GetLevelMultiplierNumeratorValue(baseCoin, "coin");
            totalItemCoins = NumberUtils.GetAdditionalMultiplierNumeratorValue(baseCoin, additionalCreditMultiplierNumerator);
            totalEventCoins = totalItemCoins;
        }

        public static double GetFirstProductEventMultiplier(Blackboard targetShopBB)
        {
            var productGroupList = GetShopProductGroups(targetShopBB);
            var productList = GetProductList(productGroupList[0]);
            var product = productList[0];

            var eventMultiplierNumerator = product.GetValue<long>("eventMultiplierNumerator");
            return NumberUtils.GetMultiplierFromNumerator(eventMultiplierNumerator);
        }

        public static int GetFirstProductSalePercent(Blackboard targetShopBB)
        {
            var productGroupList = GetShopProductGroups(targetShopBB);
            var productList = GetProductList(productGroupList[0]);
            var product = productList[0];

            return GetProductSalePercent(product);
        }

        public static bool IsShopEventPercentText(ShopType targetShopType)
        {
            // 0 : Multiplier, 1 : Percent
            switch (targetShopType)
            {
                case ShopType.COIN:
                    return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_SHOW_PERCENTAGE_COIN_SHOP").value;
                case ShopType.PIGGY_BANK:
                    return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_SHOW_PERCENTAGE_POG").value;
                case ShopType.GEM:
                    return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_SHOW_PERCENTAGE_GEM_SHOP").value;
                case ShopType.DAILY_BONUS:
                    return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_SHOW_PERCENTAGE_WHEEL").value;
                case ShopType.GEM_BAB_PROMOTION:
                    return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_SHOW_PERCENTAGE_GEM_BAB_PROMOTION_SHOP").value;
                case ShopType.GEM_JACKPOT:
                    return BlackboardUtils.FindVariable<bool>(MainBlackboard.Get(), "values/misc/ENABLE_SHOW_PERCENTAGE_GEM_JACKPOT").value;
            }

            return false;
        }

        public static void SetShopBiContextId(string contextId)
        {
            BlackboardUtils.SetOrCreateValue<string>(MainBlackboard.Get(), "shopBiContextId", contextId);
        }

        public static string GetShopBiContextId()
        {
            return BlackboardUtils.GetOrCreateVariable<string>(MainBlackboard.Get(), "shopBiContextId")?.value ?? "";
        }
    }
}