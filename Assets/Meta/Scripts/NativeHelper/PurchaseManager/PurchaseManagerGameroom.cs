#if UNITY_STANDALONE_WIN && !UNITY_EDITOR

using System.Runtime.InteropServices;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using Facebook.MiniJSON;

namespace BagelCode
{

public class PurchaseManagerGameroom : IPurchaseManager
{
    private string purchaseManagerUnityObject;
    private string SUCCESS_RESPONSE = "Success";
    private string ERROR_RESPONSE = "Error";
    private string purchaseHistoryFile = System.IO.Path.Combine(Application.persistentDataPath, "purchaseHistory.txt");

    public void Initialize(string name)
    {
        // initializePurchase(name);
        Debug.Log("[DEBUG] purchaseHistoryFile = " + purchaseHistoryFile);
        purchaseManagerUnityObject = name;
    }

    private void PurchaseCallback(IPayResult result, int serverProductId, string callback)
    {
        if (result.Cancelled || !String.IsNullOrEmpty(result.Error))
        {
            Debug.Log("[DEBUG] Purchase Error: " + result.Error + " with error code " + result.ErrorCode);
            NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callback, ERROR_RESPONSE);
            return;
        }
        Dictionary<string, object> recordDictionary = new Dictionary<string, object>(result.ResultDictionary);
        recordDictionary["server_product_id"] = serverProductId;
        Debug.Log("[DEBUG] purcahse result (record ver) = " + Json.Serialize(recordDictionary));
        try
        {
            System.IO.File.AppendAllText(purchaseHistoryFile, Json.Serialize(recordDictionary) + System.Environment.NewLine);
            Debug.Log("[DEBUG] successfully written in " + purchaseHistoryFile);
        }
        catch (Exception e)
        {
            Debug.Log("[DEBUG] error while writing: " + e.ToString());
        }
        Debug.Log("[DEBUG] Successfully Puchased... send receipt");

        // pass purchase token and access token to signature param not to add additional params
        var outputDict = new Dictionary<string, object>();
        var extraDataDict = new Dictionary<string, object>();
        extraDataDict["purchase_token"] = result.ResultDictionary["purchase_token"];
        extraDataDict["access_token"] = AccessToken.CurrentAccessToken.TokenString;
        outputDict["receipt"] = result.ResultDictionary["signed_request"];
        outputDict["signature"] = Json.Serialize(extraDataDict);
        string rawJsonString = Json.Serialize(outputDict);
        byte[] rawJsonBytes = System.Text.Encoding.ASCII.GetBytes(rawJsonString);
        string outputString = Convert.ToBase64String(rawJsonBytes);
        NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callback, outputString);
    }

    public void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback)
    {
        Debug.Log("[DEBUG] Purchase paramter = " + productId + " / " + sku);
        FB.Canvas.PayWithProductId(sku, "purchaseiap", 1, null, null, null, null, null, result => PurchaseCallback(result, productId, callback));
    }

    public void ConsumeUnclaimedPurchase(System.Action<string> callback, string callbackName)
    {
        FB.API("/app/purchases", HttpMethod.GET, purchasesResult =>
        {
            if (purchasesResult.Cancelled || !String.IsNullOrEmpty(purchasesResult.Error))
            {
                Debug.Log("[DEBUG] Failed to get unconsumed purchases: " + purchasesResult.Error);
                NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callbackName, "NO_UNCLAIMED_PURCHASE");
                return;
            }
            Debug.Log("[DEBUG] successful /app/purchases call: start consume process");
            var unconsumedPurchasesList = (List<object>)(purchasesResult.ResultDictionary["data"]);
            if (unconsumedPurchasesList.Count == 0)
            {
                Debug.Log("[DEBUG] no unconsumed purchases found");
                NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callbackName, "NO_UNCLAIMED_PURCHASE");
                return;
            }
            var purchaseTokenList = new List<string>();
            foreach(Dictionary<string, object> unconsumedPurchase in unconsumedPurchasesList)
            {
                purchaseTokenList.Add((string)unconsumedPurchase["purchase_token"]);
                Debug.Log("[DEBUG] purchase_token = " + (string)unconsumedPurchase["purchase_token"]);
            }

            // process only one unconsumed puchase for each call
            string targetPurchaseToken = purchaseTokenList[0];
            FB.API("/"+targetPurchaseToken+"/consume", HttpMethod.POST, consumeResult =>
            {
                if (consumeResult.Cancelled || !String.IsNullOrEmpty(consumeResult.Error))
                {
                    Debug.Log("[DEBUG] failed to consume purcahse: " + Json.Serialize(consumeResult.ResultDictionary));
                    // no handler for ERROR_RESPONSE exists. use NO_UNCLAIMED_PURCHASE instead.
                    NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callbackName, "NO_UNCLAIMED_PURCHASE");
                    return;
                }
                Debug.Log("[DEBUG] successfully consumed: " + purchaseTokenList[0] + " / " + Json.Serialize(consumeResult.ResultDictionary));
                if (!System.IO.File.Exists(purchaseHistoryFile))
                {
                    Debug.Log("[DEBUG] history not found");
                    NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callbackName, "NO_MATCHING_UNCLAIMED_PURCHASE");
                    return;
                }
                Debug.Log("[DEBUG] history file found. check reclaim record");
                string line = null;
                string serverProductId = null;
                string signedRequest = null;
                System.IO.StreamReader file = null;
                try
                {
                    file = new System.IO.StreamReader(purchaseHistoryFile);
                    while((line = file.ReadLine()) != null)
                    {
                        Debug.Log("[DEBUG] read line = " + line);
                        Dictionary<string, object> record = (Dictionary<string, object>)Json.Deserialize(line);
                        string recordedServerProductId = record["server_product_id"].ToString();
                        string recordedPurchaseToken = record["purchase_token"].ToString();
                        string recordedSignedRequest = record["signed_request"].ToString();
                        if (purchaseTokenList[0] == recordedPurchaseToken)
                        {
                            serverProductId = recordedServerProductId;
                            signedRequest = recordedSignedRequest;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("[DEBUG] error while reading history file: " + e.ToString());
                    NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callbackName, "NO_MATCHING_UNCLAIMED_PURCHASE");
                    return;
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
                if (serverProductId == null)
                {
                    Debug.Log("[DEBUG] unconsumed purchase not found in history. just consume and stop");
                    NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callbackName, "NO_MATCHING_UNCLAIMED_PURCHASE");
                    return;
                }
                Debug.Log("[DEBUG] found reclaim record. preparing unconsumed purchase claim...");
                var outputDict = new Dictionary<string, object>();
                var extraDataDict = new Dictionary<string, object>();
                extraDataDict["purchase_token"] = targetPurchaseToken;
                extraDataDict["access_token"] = AccessToken.CurrentAccessToken.TokenString;
                outputDict["serverProductId"] = serverProductId;
                outputDict["receipt"] = signedRequest;
                outputDict["signature"] = Json.Serialize(extraDataDict);
                string rawJsonString = Json.Serialize(outputDict);
                Debug.Log("[DEBUG] rawJsonString to send = " + rawJsonString);
                byte[] rawJsonBytes = System.Text.Encoding.ASCII.GetBytes(rawJsonString);
                string outputString = Convert.ToBase64String(rawJsonBytes);
                NativeHelperGameroom.UnitySendMessageWrapper(purchaseManagerUnityObject, callbackName, outputString);
            });
        });
    }

    public void RestorePurchasedProducts()
    {
        // Nothing to do
    }

    public PurchaseResult ParsePurchaseResult(string encodedJson)
    {
        string jsonString = System.Text.Encoding.ASCII.GetString(Convert.FromBase64String(encodedJson));
        Dictionary<string, object> purchase = (Dictionary<string, object>)Json.Deserialize(jsonString);
        string receipt = purchase["receipt"].ToString();
        string signature = purchase["signature"].ToString();
        return new PurchaseResult(receipt, signature);
    }

    public void ReportPurchaseFulfillment(string receipt, string signature, System.Action<string> callback, string callbackName)
    {
        // TODO: remove server-side consume, then put consume process here
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
