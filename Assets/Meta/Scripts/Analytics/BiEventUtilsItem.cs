using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode
{
    public static partial class BiEventUtils
    {
        public static void ItemClick(Blackboard productBB, string contextID, bool isEvent, string levelMultiplierType = "")
        {
            if (string.IsNullOrEmpty(contextID))
                contextID = GenerateContextID();

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextID;
            customData["shop_event_flag"] = isEvent;
            AppendCommonProductEventData(customData, productBB);
            AppendLevelMultiplierEventData(customData, string.IsNullOrEmpty(levelMultiplierType) ? GetLevelMultiplierFromItemType(productBB) : levelMultiplierType);
            Analytics.CustomEvent("client_item_click", customData);
            AdjustManager.Instance.SendEvent("item_click");
        }

        public static void IAPTransaction(Blackboard productBB, string contextID, bool isSuccess, bool isEvent, string errorDesc, string levelMultiplierType = "")
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextID;
            customData["status"] = isSuccess ? "success" : "fail";
            customData["shop_event_flag"] = isEvent;
            customData["error_description"] = isSuccess ? null : errorDesc;

            AppendCommonProductEventData(customData, productBB);

            if(customData.ContainsKey("gem_price"))
                customData.Remove("gem_price");

            AppendLevelMultiplierEventData(customData, string.IsNullOrEmpty(levelMultiplierType) ? GetLevelMultiplierFromItemType(productBB) : levelMultiplierType);

            Analytics.CustomEvent("client_iap_transaction", customData);
        }

        public static void GemTransaction(Blackboard productBB, string contextID, bool isSuccess, bool isEvent)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextID;
            AppendCommonProductEventData(customData, productBB);
            customData["status"] = isSuccess ? "success" : "fail";
            customData["shop_event_flag"] = isEvent;

            AppendLevelMultiplierEventData(customData, GetLevelMultiplierFromItemType(productBB));

            Analytics.CustomEvent("client_gem_transaction", customData);
        }

        public static void ItemAcquired(Blackboard productBB, string contextID, bool isEvent, string levelMultiplierType = "")
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextID;
            customData["shop_event_flag"] = isEvent;
            AppendCommonProductEventData(customData, productBB);
            AppendLevelMultiplierEventData(customData, string.IsNullOrEmpty(levelMultiplierType) ? GetLevelMultiplierFromItemType(productBB) : levelMultiplierType);

            Analytics.CustomEvent("client_item_acquired", customData);
        }

        public static void AppendLevelMultiplierEventData(Dictionary<string, object> eventData, string typeValue)
        {
            eventData["level_multiplier"] = NumberUtils.GetMultiplierFromNumerator(LevelUtils.GetLevelMultiplierNumerator(typeValue));
        }

        public static void AppendFreebieLevelMultiplierEventData(Dictionary<string, object> eventData, FreebieLevelUtils.FreebieType type)
        {
            eventData["level_multiplier"] = NumberUtils.GetMultiplierFromNumerator(FreebieLevelUtils.GetLevelMultiplierNumerator(type));
        }

        public static string GetLevelMultiplierFromItemType(Blackboard productBB)
        {
            var itemList = BlackboardUtils.FindVariable<List<Blackboard>>(productBB, "itemList");
            if (itemList != null)
            {
                var itemType = BlackboardUtils.FindVariable<ItemType>(itemList.value[0], "itemType");
                if (itemType != null)
                {
                    switch (itemType.value)
                    {
                        case ItemType.CREDIT:
                        case ItemType.CREDIT_MULTIPLIER_WHEEL:
                        case ItemType.CREDIT_WHEEL:
                        case ItemType.CODA_SHOP_CREDIT:
                            return "coin";
                        case ItemType.DAILY_BOOST:
                            return "dailyBoost";
                        case ItemType.CREDIT_POT_OF_GOLD:
                        case ItemType.PIGGY_BANK:
                        case ItemType.POG_BOOSTER:
                            return "pog";
                        case ItemType.DAILY_BONUS_WHEEL:
                        case ItemType.DAILY_MEGA_WHEEL:
                            return "wheel";
                        case ItemType.TICKETED_BONUS_TICKET:
                        case ItemType.TICKETED_BONUS_BOOSTER:
                            return "ticketedBonus";
                        case ItemType.SPIN_BOOST:
                            return "spinDeal";
                        case ItemType.EARLY_ACCESS:
                            return "earlyAccess";
                        // check Item Type => default "coin"
                        //case ItemType.TIER_BOOST:
                        //case ItemType.GEM:
                        //case ItemType.GEM_BOOSTER:
                        //case ItemType.EPIC_PASS:
                        //case ItemType.CODA_SHOP_GEM:
                        //case ItemType.HIDDEN_UNIVERSE_FINDER:
                        default:
                            return "coin";
                    }
                }
            }
            return "";
        }
    }
}
