#if UNITY_WEBGL && !UNITY_EDITOR

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SlotMaker.Json;

namespace BagelCode
{

public class PurchaseManagerCanvas : IPurchaseManager
{
    private string purchaseManagerUnityObject;
    private string SUCCESS_RESPONSE = "Success";
    private string ERROR_RESPONSE = "Error";
    private string purchaseHistoryKey = "purchaseHistory";

    public void Initialize(string name)
    {
        purchaseManagerUnityObject = name;
    }

    [DllImport("__Internal")]
    private static extern void PayWithProductIdFacebook(string callbackObjectName, string callbackMethodName, int serverProductId, string sku, bool isSubscription);
    public void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback)
    {
        PayWithProductIdFacebook(purchaseManagerUnityObject, callback, productId, sku, isSubscription);
    }

    [DllImport("__Internal")]
    private static extern void ConsumeUnclaimedPurchaseFacebook(string callbackObjectName, string callbackMethodName);
    public void ConsumeUnclaimedPurchase(System.Action<string> callback, string callbackName)
    {
        ConsumeUnclaimedPurchaseFacebook(purchaseManagerUnityObject, callbackName);
    }

    public void RestorePurchasedProducts()
    {
        // Nothing to do
    }

    public PurchaseResult ParsePurchaseResult(string encodedJson)
    {
        string jsonString = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(encodedJson));
        Dictionary<string, object> purchase = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(jsonString);
        string receipt = purchase["receipt"].ToString();
        string signature = purchase["signature"].ToString();
        return new PurchaseResult(receipt, signature);
    }

    [DllImport("__Internal")]
    private static extern void ConsumePurchase(string callbackObjectName, string callbackMethodName, string decodedSignedRequest);
    public void ReportPurchaseFulfillment(string receipt, string signature, System.Action<string> callback, string callbackName)
    {
        if (!string.IsNullOrEmpty(receipt))
        {
            try
            {
                string[] signAndSignedRequest = receipt.Split('.');
                string signedRequest = signAndSignedRequest[1];
                string nonURLSafeBase64SignedRequest = signedRequest.Replace('_', '/').Replace('-', '+');
                switch(nonURLSafeBase64SignedRequest.Length % 4)
                {
                    case 2: nonURLSafeBase64SignedRequest += "=="; break;
                    case 3: nonURLSafeBase64SignedRequest += "="; break;
                }
                string decodedSignedRequest = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(nonURLSafeBase64SignedRequest));
                ConsumePurchase(purchaseManagerUnityObject, callbackName, decodedSignedRequest);
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

    public void GetAndUpdateMicrosoftStoreIdKey(string azureAdCollectionsToken, string userId, System.Action<string> callback, string callbackName)
    {
        // Nothing to do
        callback("");
    }
}

}

#endif
