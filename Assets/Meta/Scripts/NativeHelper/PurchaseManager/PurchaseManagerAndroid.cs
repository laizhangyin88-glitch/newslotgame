#if UNITY_ANDROID && !PLATFORM_AMAZON && !UNITY_EDITOR

using System.Runtime.InteropServices;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker.Json;

namespace BagelCode
{

public class PurchaseManagerAndroid : IPurchaseManager
{
    private AndroidJavaClass ajc = new AndroidJavaClass("com.bagelcode.v3.UnityPlayerActivity");

    public void Initialize(string name)
    {
        //ajc.CallStatic("Initialize", name);
    }

    public void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback)
    {
        ajc.CallStatic("purchase", productId, sku, isSubscription, purchaseId, callback);
    }

    public void ConsumeUnclaimedPurchase(System.Action<string> callback, string callbackName)
    {
        ajc.CallStatic("consumeUnclaimedPurchase", callbackName);
    }

    public void RestorePurchasedProducts()
    {
        // Nothing to do
    }

    public PurchaseResult ParsePurchaseResult(string encodedJson)
    {
        string receipt = "";
        string signature = "";
        string currencyCode = "";
        string localPrice = "";
        long purchaseProgressId = 0L;

        try
        {
            byte[] decodedData = Convert.FromBase64String(encodedJson);
            string decodedJson = System.Text.Encoding.UTF8.GetString(decodedData);

            var jsonObj = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(decodedJson);
            receipt      = jsonObj["receipt"].ToString();
            signature    = jsonObj["signature"].ToString();
            if(jsonObj.ContainsKey("currencyCode"))
                currencyCode = jsonObj["currencyCode"].ToString();
            if(jsonObj.ContainsKey("localPrice"))
                localPrice = jsonObj["localPrice"].ToString();
            if(jsonObj.ContainsKey("purchaseProgressId"))
                purchaseProgressId = System.Convert.ToInt64(jsonObj["purchaseProgressId"]);
        }
        catch (Exception ex)
        {
            if(string.IsNullOrEmpty(encodedJson))
                throw new Exception("Purcase Result: encodedJson is null");
            else
                throw new Exception( string.Format("Purcase Result: {0}", encodedJson) );

            return null;
        }

        return new PurchaseResult(receipt, signature, currencyCode, localPrice, purchaseProgressId);
    }

    public void ReportPurchaseFulfillment(string receipt, string signature, System.Action<string> callback, string callbackName)
    {
        // remove original consume process and put that here
        ajc.CallStatic("reportPurchaseFulfillment", callbackName);
    }

    public void GetAndUpdateMicrosoftStoreIdKey(string azureAdCollectionsToken, string userId, System.Action<string> callback, string callbackName)
    {
        // Nothing to do
        callback("");
    }
}

}

#endif
