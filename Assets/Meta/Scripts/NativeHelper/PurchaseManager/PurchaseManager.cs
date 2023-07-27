using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SlotMaker;

namespace BagelCode
{

public class PurchaseManager: SlotMaker.MonoWeakSingleton<PurchaseManager>
{
    private IPurchaseManager delegator;

    // Local recover save keys.
    public static string LOCAL_LAST_PURCHASE_SKU = "LAST_PURCHASE_SKU";
    public static string LOCAL_LAST_PURCHASE_RECEIPT = "LAST_PURCHASE_RECEIPT";
    public static string LOCAL_LAST_PURCHASE_SIGNATURE = "LAST_PURCHASE_SIGNATURE";
    public static string LOCAL_LAST_PURCHASE_CURRENCY_CODE = "LAST_PURCHASE_CURRENCY_CODE";
    public static string LOCAL_LAST_PURCHASE_LOCAL_PRICE = "LAST_PURCHASE_LOCAL_PRICE";
    public static string LOCAL_LAST_PURCHASE_PROGRESS_ID = "LAST_PURCHASE_PROGRESS_ID";

    public void Awake()
    {
        //#if UNITY_IPHONE && !UNITY_EDITOR
        //delegator = new PurchaseManagerIOS();
        //#elif UNITY_ANDROID && PLATFORM_AMAZON && !UNITY_EDITOR
        //delegator = new PurchaseManagerAmazon();
        //#elif UNITY_ANDROID && !UNITY_EDITOR
        //delegator = new PurchaseManagerAndroid();
        //#elif UNITY_WSA && !UNITY_EDITOR
        //delegator = new PurchaseManagerWindows();
        //#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        //delegator = new PurchaseManagerGameroom();
        //#elif UNITY_WEBGL && !UNITY_EDITOR
        //delegator = new PurchaseManagerCanvas();
        //#else
        delegator = new PurchaseManagerUnity();
        //#endif
    }

    public void Initialize()
    {
        Debug.Log("PurchaseManager Initialize");
        delegator.Initialize(gameObject.name);
    }

    #region Purchase
    private System.Action<PurchaseResult> purchaseManagerSuccessCallback;
    private System.Action<string> purchaseManagerFailureCallback;
    private System.Action<string> purchaseManagerPendingCallback;
    private string ERROR = "Error";
    private string PENDING = "Pending";
    public void Purchase(
            int productId,
            string sku,
            bool isSubscription,
            long purchaseProgressId,
            System.Action<PurchaseResult> successCallback,
            System.Action<string> failureCallback,
            System.Action<string> pendingCallback
        )
    {
        purchaseManagerSuccessCallback = successCallback;
        purchaseManagerFailureCallback = failureCallback;
        purchaseManagerPendingCallback = pendingCallback;

        delegator.Purchase(productId, sku, isSubscription, purchaseProgressId, "OnPurchaseManagerCallback");
    }

    private void OnPurchaseManagerCallback(string encodedJson)
    {
        if (ApplicationSettings.LogTest())
            Debug.Log("OnPurchaseManagerCallback : " + encodedJson);

        if (purchaseManagerSuccessCallback == null || purchaseManagerFailureCallback == null)
        {
            return;
        }

        if (encodedJson.Length >= ERROR.Length && encodedJson.Substring(0, ERROR.Length).Equals(ERROR))
        {
            purchaseManagerFailureCallback(encodedJson.Substring(ERROR.Length));
        }
        else if (encodedJson.Length >= PENDING.Length && encodedJson.Substring(0, PENDING.Length).Equals(PENDING))
        {
            purchaseManagerPendingCallback(encodedJson.Substring(PENDING.Length));
            return;
        }
        else
        {
            purchaseManagerSuccessCallback(delegator.ParsePurchaseResult(encodedJson));
        }

        EndPurchase();
    }

    public void EndPurchase()
    {
        purchaseManagerFailureCallback = null;
        purchaseManagerSuccessCallback = null;
        purchaseManagerPendingCallback = null;
    }
    #endregion

    #region ConsumeUnclaimedPurchase
    private System.Action<string> consumeManagerCallback;
    public void ConsumeUnclaimedPurchase(System.Action<string> callback)
    {
        consumeManagerCallback = callback;
        delegator.ConsumeUnclaimedPurchase(callback, "OnConsumeUnclaimedPurchaseCallback");
    }

    private void OnConsumeUnclaimedPurchaseCallback(string encodedJson)
    {
        if (ApplicationSettings.LogTest())
            Debug.Log("OnConsumeUnclaimedPurchaseCallback : " + encodedJson);

        if (consumeManagerCallback == null)
        {
            return;
        }
        consumeManagerCallback(encodedJson);
        consumeManagerCallback = null;
    }
    #endregion

    public void RestorePurchasedProducts()
    {
        delegator.RestorePurchasedProducts();
    }

    private System.Action<string> reportPurchaseFulfillmentCallback;
    public void ReportPurchaseFulfillment(string receipt, string signature, System.Action<string> callback)
    {
        reportPurchaseFulfillmentCallback = callback;
        delegator.ReportPurchaseFulfillment(receipt, signature, callback, "OnReportPurchaseFulfillmentCallback");
    }

    private void OnReportPurchaseFulfillmentCallback(string success) {
        if (ApplicationSettings.LogTest())
            Debug.Log("OnConsumeUnclaimedPurchaseCallback : " + success);
        reportPurchaseFulfillmentCallback(success);
        reportPurchaseFulfillmentCallback = null;
    }

    private System.Action<string> getAndUpdateMicrosoftStoreIdKeyCallback;
    public void GetAndUpdateMicrosoftStoreIdKey(string azureAdCollectionsToken, string userId, System.Action<string> callback)
    {
        getAndUpdateMicrosoftStoreIdKeyCallback = callback;
        delegator.GetAndUpdateMicrosoftStoreIdKey(azureAdCollectionsToken, userId, callback, "OnGetAndUpdateMicrosoftStoreIdKeyCallback");
    }

    private void OnGetAndUpdateMicrosoftStoreIdKeyCallback(string microsoftStoreIdKey) {
        if (ApplicationSettings.LogTest())
            Debug.Log("OnGetAndUpdateMicrosoftStoreIdKeyCallback : " + microsoftStoreIdKey);
        getAndUpdateMicrosoftStoreIdKeyCallback(microsoftStoreIdKey);
        getAndUpdateMicrosoftStoreIdKeyCallback = null;
    }

    public void LocalConsumeUnclaimedPurchase(System.Action<string> callback)
    {
        // Use Local Consume logic by all decive.
// #if DEV
//         bool isDebugCrash = (PlayerPrefs.GetInt("DEBUG_PURCHASE_CRASH", 1) == 0);
//         if(isDebugCrash)
//         {
            callback(GetLastPurchaseReceiptEncodedJson());
//         }
//         else
// #endif
//         {
//             ConsumeUnclaimedPurchase(callback);
//         }
    }

    public void SetLastPurchaseReceipt(string sku, string receipt, string signature, string currencyCode, string localPrice, long purchaseProgressId)
    {
        PlayerPrefs.SetString(LOCAL_LAST_PURCHASE_SKU, sku);
        PlayerPrefs.SetString(LOCAL_LAST_PURCHASE_RECEIPT, receipt);
        PlayerPrefs.SetString(LOCAL_LAST_PURCHASE_SIGNATURE, signature);
        PlayerPrefs.SetString(LOCAL_LAST_PURCHASE_CURRENCY_CODE, currencyCode);
        PlayerPrefs.SetString(LOCAL_LAST_PURCHASE_LOCAL_PRICE, localPrice);
        PlayerPrefsUtils.SetInt64(LOCAL_LAST_PURCHASE_PROGRESS_ID, purchaseProgressId);

        PlayerPrefs.Save();
    }

    public string GetLastPurchaseReceiptEncodedJson()
    {
        string sku          = PlayerPrefs.GetString(LOCAL_LAST_PURCHASE_SKU, "");
        string receipt      = PlayerPrefs.GetString(LOCAL_LAST_PURCHASE_RECEIPT, "");
        string signature    = PlayerPrefs.GetString(LOCAL_LAST_PURCHASE_SIGNATURE, "");
        string currencyCode = PlayerPrefs.GetString(LOCAL_LAST_PURCHASE_CURRENCY_CODE, "");
        string localPrice   = PlayerPrefs.GetString(LOCAL_LAST_PURCHASE_LOCAL_PRICE, "");
        long purchaseProgressId = PlayerPrefsUtils.GetInt64(LOCAL_LAST_PURCHASE_PROGRESS_ID, 0L);

        if (ApplicationSettings.LogSystem() && !string.IsNullOrEmpty(receipt))
                Debug.LogError(string.Format("GetLastPurchaseReceiptEncodedJson : {0}, {1}, {2}", sku, receipt, signature, currencyCode, localPrice));
#if DEV
        if(!string.IsNullOrEmpty(sku))
#else
        if(!string.IsNullOrEmpty(sku) && !string.IsNullOrEmpty(receipt))
#endif
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("sku", sku);
            data.Add("receipt", string.IsNullOrEmpty(receipt) ? "" : receipt);
            data.Add("signature", string.IsNullOrEmpty(signature) ? "" : signature);
            data.Add("currencyCode", string.IsNullOrEmpty(currencyCode) ? "" : currencyCode);
            data.Add("localPrice", string.IsNullOrEmpty(localPrice) ? "" : localPrice);
            data.Add("purchaseProgressId", purchaseProgressId);

            string jsonData = SlotMaker.Json.SlotSimpleJson.SerializeObject(data);
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonData));
        }

        return null;
    }

    public void ClearLastPurchaseReceipt()
    {
        if(PlayerPrefs.HasKey(LOCAL_LAST_PURCHASE_SKU))
            PlayerPrefs.DeleteKey(LOCAL_LAST_PURCHASE_SKU);
        if(PlayerPrefs.HasKey(LOCAL_LAST_PURCHASE_RECEIPT))
            PlayerPrefs.DeleteKey(LOCAL_LAST_PURCHASE_RECEIPT);
        if(PlayerPrefs.HasKey(LOCAL_LAST_PURCHASE_SIGNATURE))
            PlayerPrefs.DeleteKey(LOCAL_LAST_PURCHASE_SIGNATURE);
        if(PlayerPrefs.HasKey(LOCAL_LAST_PURCHASE_CURRENCY_CODE))
            PlayerPrefs.DeleteKey(LOCAL_LAST_PURCHASE_CURRENCY_CODE);
        if(PlayerPrefs.HasKey(LOCAL_LAST_PURCHASE_LOCAL_PRICE))
            PlayerPrefs.DeleteKey(LOCAL_LAST_PURCHASE_LOCAL_PRICE);
        if (PlayerPrefs.HasKey(LOCAL_LAST_PURCHASE_PROGRESS_ID))
            PlayerPrefs.DeleteKey(LOCAL_LAST_PURCHASE_PROGRESS_ID);

        PlayerPrefs.Save();
    }
}
}
