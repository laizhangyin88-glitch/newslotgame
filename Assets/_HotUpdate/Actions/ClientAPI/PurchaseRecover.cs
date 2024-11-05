using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class PurchaseRecover : ActionTask <Blackboard>
{
    private const string ERROR = "Error";

    protected override string info
    {
        get
        {
            return string.Format("Consume Unclaimed Product & Recover");
        }
    }

    protected override void OnExecute()
    {
        ConsumeUnclaimedPurchase();
    }

    private void ConsumeUnclaimedPurchase()
    {
#if !UNITY_EDITOR && UNITY_ANDROID && !PLATFORM_AMAZON
        if (ApplicationSettings.LogSystem())
            Debug.LogError("Native Recover");
        PurchaseManager.Instance.ConsumeUnclaimedPurchase(ConsumeCallback);
#else
        PurchaseManager.Instance.LocalConsumeUnclaimedPurchase(ConsumeCallback);
#endif
    }

    private void ConsumeCallback(string encodedJson)
    {
        if (encodedJson == null || encodedJson.Equals("NO_UNCLAIMED_PURCHASE"))
        {
            // Nothing to do
            Debug.Log("NO_UNCLAIMED_PURCHASE");
            EndAction(true);
        }
        else if (encodedJson.Equals("NO_MATCHING_UNCLAIMED_PURCHASE"))
        {
            // Force consume
            var purchaseCSvar = BlackboardUtils.GetOrCreateVariable<bool>(null, "/needPurchaseCS");
            purchaseCSvar.value = true;

            Debug.Log("NoMatchingUnclaimedPurchase");
            EndAction(true);
        }
        else if (encodedJson.Length >= ERROR.Length && encodedJson.Substring(0, ERROR.Length).Equals(ERROR))
        {
            // some error
            EndAction(true);
        }
        else
        {
            string receipt = "";
            string signature = "";
            string productSku = "";
            string currencyCode = "";
            string localPrice = "";
            long purchaseProgressId = 0;

            try
            {
                byte[] decodedData = Convert.FromBase64String(encodedJson);
                string decodedJson = System.Text.Encoding.UTF8.GetString(decodedData);

                var jsonObj = SlotSimpleJson.DeserializeObject<Dictionary<string, object>>(decodedJson);
                receipt         = jsonObj["receipt"].ToString();
                productSku      = jsonObj["sku"].ToString();
                signature       = jsonObj["signature"].ToString();

                if (jsonObj.ContainsKey("purchaseProgressId"))
                    purchaseProgressId = System.Convert.ToInt64(jsonObj["purchaseProgressId"]);

                if(jsonObj.ContainsKey("currencyCode"))
                    currencyCode = jsonObj["currencyCode"].ToString();
                if(jsonObj.ContainsKey("localPrice"))
                    localPrice = jsonObj["localPrice"].ToString();

                if (ApplicationSettings.LogSystem())
                    Debug.LogError( string.Format("Purchase recover. CurrencyCode : ({0}), Price : {1}", currencyCode, localPrice) );

#if UNITY_WSA && !UNITY_EDITOR
                ConsumeUnclaimedPurchaseWrapperForWindows(productSku, receipt, signature, currencyCode, localPrice, purchaseProgressId);
#else
                RequestRecover(productSku, receipt, signature, currencyCode, localPrice, purchaseProgressId);
#endif
            }
            catch (Exception ex)
            {
                if(string.IsNullOrEmpty(encodedJson))
                    throw new Exception("Purcase Recover: encodedJson is null");
                else
                    throw new Exception( string.Format("Purcase Recover: {0}", encodedJson) );

                PurchaseManager.Instance.ReportPurchaseFulfillment(receipt, signature,
                (string success) =>
                {
                    PurchaseManager.Instance.ClearLastPurchaseReceipt();
                    EndAction(true);
                });
            }


        }
    }

    private void RequestRecover(string productSku, string receipt, string signature, string currencyCode, string localPrice, long purchaseProgressId)
    {
        if (ApplicationSettings.LogSystem())
        {
            if(!string.IsNullOrEmpty(productSku))
                Debug.LogError(string.Format("Purchase Recover : {0}, {1}, {2}", productSku, receipt, signature));
        }

        BagelCodeClientAPI.Purchase_Recover(productSku, receipt, signature, currencyCode, localPrice, purchaseProgressId,
        (response) =>
        {
            if (ApplicationSettings.LogSystem())
                Debug.LogError("Purchase Recover : Success");

            PurchaseManager.Instance.ReportPurchaseFulfillment(receipt, signature,
            (string success) =>
            {
                PurchaseManager.Instance.ClearLastPurchaseReceipt();

                if(response.itemUseResultList != null && response.itemUseResultList.Count > 0)
                {
                    var recoverProductvar = BlackboardUtils.GetOrCreateVariable<bool>(null, "/enableRecoverProduct");
                    recoverProductvar.value = true;

                    string firstItemName = "";

                    for(int i=0; i< response.itemUseResultList.Count; ++i)
                    {
                        firstItemName = BlackboardQueryUtils.GetItemName(response.itemUseResultList[i].itemType);

                        if(!string.IsNullOrEmpty(firstItemName))
                            break;
                    }

                    var recoverProductMessagevar = BlackboardUtils.GetOrCreateVariable<string>(null, "/recoverProductMessage");
                    var unknownRecoverProductvar = BlackboardUtils.GetOrCreateVariable<bool>(null, "/unknownRecoverProduct");

                    if(string.IsNullOrEmpty(firstItemName))
                    {
                        unknownRecoverProductvar.value = true;
                        recoverProductMessagevar.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_PURCHASE_RECOVER_UNKNOWN_TEXT");
                    }
                    else
                    {
                        unknownRecoverProductvar.value = false;
                        recoverProductMessagevar.value = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_PURCHASE_RECOVER_DEFAULT_TEXT", firstItemName);
                    }
                }
                else
                {
                    var recoverCSvar = BlackboardUtils.GetOrCreateVariable<bool>(null, "/needPurchaseCS");
                    recoverCSvar.value = true;
                }

                EndAction(true);
            });
        },
        (error) =>
        {
            if (ApplicationSettings.LogSystem())
                Debug.LogError( string.Format("Purchase Recover : {0}", error.errorCode) );

            switch(error.errorCode)
            {
                case ClientModels.Error.IMPOSSIBLE_PURCHASE_RECOVERY_ERROR:
                    var recoverCSvar = BlackboardUtils.GetOrCreateVariable<bool>(null, "/needPurchaseCS");
                    recoverCSvar.value = true;
                    break;
            }

            PurchaseManager.Instance.ReportPurchaseFulfillment(receipt, signature,
            (string success) =>
            {
                PurchaseManager.Instance.ClearLastPurchaseReceipt();
                EndAction(true);
            });
        });
    }

#if UNITY_WSA && !UNITY_EDITOR
    private void ConsumeUnclaimedPurchaseWrapperForWindows(string productSku, string receipt, string signature, string currencyCode, string localPrice, long purchaseProgressId)
    {
        BagelCodeClientAPI.GetAzureADCollectionsToken(
        (response) =>
        {
            var userId = BlackboardUtils.FindVariable<string>(null, "/me/userId").value;
            PurchaseManager.Instance.GetAndUpdateMicrosoftStoreIdKey(response.azureAdCollectionsToken, userId,
            (microsoftStoreIdKey) =>
            {
                RequestRecover(productSku, receipt, signature, currencyCode, localPrice, purchaseProgressId);
            });
        },
        (error) =>
        {
            // GlobalErrorHandler.GlobalError(error);
            EndAction(true);
        });
    }
#endif
}

}
