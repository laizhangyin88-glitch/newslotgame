using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{

public class PurchaseManagerUnity : IPurchaseManager
{
    public void Initialize(string name)
    {
        // Nothing to do
    }

    public void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback)
    {
        // Nothing to do
    }

    public void ConsumeUnclaimedPurchase(System.Action<string> callback, string callbackName)
    {
        callback(null);
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
        // Nothing to do
        callback("Success");
    }

    public void GetAndUpdateMicrosoftStoreIdKey(string azureAdCollectionsToken, string userId, System.Action<string> callback, string callbackName)
    {
        // Nothing to do
        callback("");
    }
}

}
