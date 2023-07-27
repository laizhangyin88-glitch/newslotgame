using UnityEngine;

namespace BagelCode
{

public class PurchaseResult
{
    public PurchaseResult(string receipt, string signature, string currencyCode = "", string localPrice = "", long purchaseProgressId = 0L) {
        Receipt = receipt;
        Signature = signature;
        CurrencyCode = currencyCode;
        LocalPrice = localPrice;
        PurchaseProgressId = purchaseProgressId;
    }
    public string Receipt { get; }
    public string Signature { get; }
    public string CurrencyCode { get; }
    public string LocalPrice { get; }
    public long PurchaseProgressId { get; }
}

}
