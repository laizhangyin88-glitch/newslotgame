using UnityEngine;

namespace BagelCode
{

public interface IPurchaseManager
{
    void Initialize(string name);
    void Purchase(int productId, string sku, bool isSubscription, long purchaseId, string callback);
    void ConsumeUnclaimedPurchase(System.Action<string> callback, string callbackName);
    void RestorePurchasedProducts();
    PurchaseResult ParsePurchaseResult(string encodedJson);
    void ReportPurchaseFulfillment(string receipt, string signature, System.Action<string> callback, string callbackName);
    void GetAndUpdateMicrosoftStoreIdKey(string azureAdCollectionsToken, string userId, System.Action<string> callback, string callbackName);
}

}
