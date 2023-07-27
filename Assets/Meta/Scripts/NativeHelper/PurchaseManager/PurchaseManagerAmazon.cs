#if UNITY_ANDROID && PLATFORM_AMAZON && !UNITY_EDITOR

using System.Runtime.InteropServices;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker.Json;

namespace BagelCode
{

public class PurchaseManagerAmazon : IPurchaseManager
{
    private AndroidJavaClass ajc = new AndroidJavaClass("com.bagelcode.v3.UnityPlayerActivity");

    public void Initialize(string name)
    {
        //ajc.CallStatic("Initialize", name);
    }

    public void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback)
    {
        ajc.CallStatic("purchase", productId, sku, callback);
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
        return new PurchaseResult(encodedJson, "");
    }

    public void ReportPurchaseFulfillment(string receipt, string signature, System.Action<string> callback, string callbackName)
    {
        // TODO: remove original consume process and put that here
        callback("Success");
    }

    public void GetAndUpdateMicrosoftStoreIdKey(string azureAdCollectionsToken, string userId, System.Action<string> callback, string callbackName)
    {
        // Nothing to do
        callback("");
    }
}

}

#endif
