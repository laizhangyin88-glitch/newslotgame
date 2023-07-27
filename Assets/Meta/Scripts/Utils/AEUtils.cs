using SlotMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class AEUtils
    {
        public static void SendAE(string name, params (string, object)[] values)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();
            values.ForEach(v => customData[v.Item1] = v.Item2);
            Analytics.CustomEvent(name, customData);
        }

        public static void SendAEStoreOpened(ShopType shopType, int shopId, string contextId, string type = "coin")
        {
            var customData = new Dictionary<string, object>();
            customData["shop_type"] = shopType.ToString();
            customData["shop_id"] = shopId;
            customData["context_id"] = contextId;

            var eventInfo = PassiveEventUtils.GetPassiveEvent(shopType);
            customData["shop_event_flag"] = eventInfo != null;

            customData["open_type"] = "default";
            if (PlayerPrefs.HasKey("SHOP_OPENED_FROM_TYPE"))
            {
                string openType = PlayerPrefs.GetString("SHOP_OPENED_FROM_TYPE");
                if (!string.IsNullOrEmpty(openType))
                    customData["open_type"] = openType;

                PlayerPrefs.DeleteKey("SHOP_OPENED_FROM_TYPE");
            }

            BiEventUtils.AppendLevelMultiplierEventData(customData, type);
            Analytics.CustomEvent("client_store_opened", customData);
            AdjustManager.Instance.SendEvent("store_opened");
        }
    }
}
