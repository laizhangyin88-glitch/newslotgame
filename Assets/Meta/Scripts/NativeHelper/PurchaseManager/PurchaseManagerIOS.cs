#if UNITY_IPHONE && !UNITY_EDITOR

using System.Runtime.InteropServices;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BagelCode
{

public class PurchaseManagerIOS : IPurchaseManager
{
    [DllImport ("__Internal")]
    private static extern void _initializeBagelcodeIAP(string name);

    public void Initialize(string name)
    {
        _initializeBagelcodeIAP(name);
    }

    [DllImport("__Internal")]
    private static extern void _paymentRequestWithProductIdentifiers(string productIdentifier, string callback);

    public void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback)
    {
        _paymentRequestWithProductIdentifiers(sku, callback);
    }

    public void ConsumeUnclaimedPurchase(System.Action<string> callback, string callbackName)
    {
        //Nothing to do
        callback(null);
    }

    [DllImport("__Internal")]
    private static extern void _restorePurchasedProducts();

    public void RestorePurchasedProducts()
    {
        _restorePurchasedProducts();
    }

    public PurchaseResult ParsePurchaseResult(string encodedJson)
    {
        // encodedJson = receipt | currencyCode
        string[] splitedData = encodedJson.Split('|');
        string receipt = splitedData[0];
        string currencyCode = splitedData.Length >= 2 ? splitedData[1] : "";
        string localPrice = splitedData.Length >= 3 ? splitedData[2] : "";

        return new PurchaseResult(receipt, "", currencyCode, localPrice);
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

#endif
