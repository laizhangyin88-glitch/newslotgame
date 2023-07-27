#if UNITY_WSA && !UNITY_EDITOR

using System.Runtime.InteropServices;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SlotMaker.Json;

namespace BagelCode
{

public class PurchaseManagerWindows : IPurchaseManager
{
    [DllImport ("__Internal")]
    private static extern void initializePurchase([MarshalAs(UnmanagedType.LPWStr)]string name);
    public void Initialize(string name)
    {
        initializePurchase(name);
    }

    [DllImport("__Internal")]
    private static extern void purchase([MarshalAs(UnmanagedType.LPWStr)]string storeId, int productId, [MarshalAs(UnmanagedType.LPWStr)]string callback);
    public void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback)
    {
        purchase(sku, productId, callback);
    }

    [DllImport("__Internal")]
    private static extern void consumeUnclaimedPurchase([MarshalAs(UnmanagedType.LPWStr)]string callbackName);
    public void ConsumeUnclaimedPurchase(System.Action<string> callback, string callbackName)
    {
        consumeUnclaimedPurchase(callbackName);
    }

    public void RestorePurchasedProducts()
    {
        // Nothing to do
    }

    public PurchaseResult ParsePurchaseResult(string purchaseResult)
    {
        string[] splitedData = purchaseResult.Split('|');
        string storeID = splitedData[0];
        string msStoreIDKey = splitedData[1];
        string currencyCode = splitedData[2];
        string localPrice = splitedData[3];

        return new PurchaseResult(storeID + "|" + msStoreIDKey, "", currencyCode, localPrice);
    }

    [DllImport("__Internal")]
    private static extern void reportConsumableProductsAsFulfilled([MarshalAs(UnmanagedType.LPWStr)]string storeId, [MarshalAs(UnmanagedType.LPWStr)]string callbackName);
    public void ReportPurchaseFulfillment(string receipt, string signature, System.Action<string> callback, string callbackName)
    {
        if (!string.IsNullOrEmpty(receipt))
        {
            try
            {
                // receipt = sku | storeID | receipt
                string[] splitedStoreIdAndIdKey = receipt.Split('|');
                string addOnStoreId = splitedStoreIdAndIdKey[1];
                reportConsumableProductsAsFulfilled(addOnStoreId, callbackName);
            }
            catch(Exception ex)
            {
                callback("Success");
            }
        }
        else
        {
            callback("Success");
        }
    }

    [DllImport("__Internal")]
    private static extern void getAndUpdateMicrosoftStoreIdKey([MarshalAs(UnmanagedType.LPWStr)]string azureAdCollectionsToken, [MarshalAs(UnmanagedType.LPWStr)]string userId, [MarshalAs(UnmanagedType.LPWStr)]string callbackName);
    public void GetAndUpdateMicrosoftStoreIdKey(string azureAdCollectionsToken, string userId, System.Action<string> callback, string callbackName)
    {
        getAndUpdateMicrosoftStoreIdKey(azureAdCollectionsToken, userId, callbackName);
    }
}

}

#endif
