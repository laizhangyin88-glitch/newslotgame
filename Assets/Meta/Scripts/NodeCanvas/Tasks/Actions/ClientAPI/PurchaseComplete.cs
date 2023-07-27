using UnityEngine;
using System;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker.Json;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class PurchaseComplete : ActionTask <Blackboard>
{
    public BBParameter<string> productBBValue;
    public BBParameter<long> purchaseProgressID;
    public BBParameter<string> contextID;
    public BBParameter<bool> isEvent;
    public BBParameter<bool> hasSufficientBucks;

    [BlackboardOnly]
    public BBParameter<bool> isSuccess;

    private int purchaseProductID;
    private string purchaseProductSku;

    private GameObject pendingPopupObj = null;
    private PopupWaitPendingPurchaseController pendingController = null;

    protected override string info
    {
        get
        {
            return "Purchase Complete";
        }
    }

    protected override void OnExecute()
    {
        if(BlackboardQueryUtils.CheckPurchaseProhibitedRegion())
        {
            isSuccess.value = false;
            EndAction(true);
            return;
        }

#if DEV
        bool isDebugPurchase = (PlayerPrefs.GetInt("DEBUG_PURCHASE", 1) == 1);

        if(isDebugPurchase)
        {
            DebugComplete();
        }
        else
#endif
        {
#if UNITY_EDITOR
            Debug.LogError("Turn off DEBUG_PURCHASE");
            isSuccess.value = false;
            EndAction(true);
#elif UNITY_WSA && !UNITY_EDITOR
            PurchaseWrapperForWindows();
#else
            Complete();
#endif
        }

    }

    private void DebugComplete()
    {
        var productId = BlackboardUtils.FindVariable<int>(agent, string.Format("{0}/id", productBBValue.value) );

        if (productId != null)
        {
            purchaseProductID = productId.value;
            ClientIAPTransaction(true);

            var platformName = FirstLetterToUpper(ApplicationSettings.GetPlatformName().ToLower());

            var productSku = BlackboardUtils.FindVariable<string>(agent, string.Format("{0}/sku{1}", productBBValue.value, platformName) );

            PurchaseManager.Instance.SetLastPurchaseReceipt(productSku != null ? productSku.value : "", "", "", "", "", purchaseProgressID.value);

            bool isDebugCrash = (PlayerPrefs.GetInt("DEBUG_PURCHASE_CRASH", 1) == 0);
            if(isDebugCrash && agent != null)
            {
                isSuccess.value = false;
                EndAction(true);
            }
            else
            {
                RequestPurchaseComplete( "", "", "", "");
            }

        }
        else
        {
            Debug.LogError("No ItemId found." + agent.gameObject.name);
        }
    }

    private void Complete()
    {
        var productId = BlackboardUtils.FindVariable<int>(agent, string.Format("{0}/id", productBBValue.value) );

        var platformName = FirstLetterToUpper(ApplicationSettings.GetPlatformName().ToLower());

        var productSku = BlackboardUtils.FindVariable<string>(agent, string.Format("{0}/sku{1}", productBBValue.value, platformName) );

        string msStoreID = "";
#if UNITY_WSA
        msStoreID = BlackboardUtils.FindVariable<string>(agent, string.Format("{0}/msStoreId", productBBValue.value) ).value;
#endif

        var isSubscription = BlackboardUtils.FindVariable<bool>(agent, string.Format("{0}/isSubscription", productBBValue.value) );

        if (productId != null)
        {
            purchaseProductID = productId.value;
            purchaseProductSku = productSku != null ? productSku.value : "";
            if (GetUseBucks())
                RequestPurchaseComplete("", "", "", "");
            else
                NativePurchase(msStoreID, isSubscription.value);
        }
        else
        {
            Debug.LogError("No ItemId found." + agent.gameObject.name);
        }
    }

    private void NativePurchase(string msStoreID, bool isSubscription)
    {
#if UNITY_WSA
        PurchaseManager.Instance.Purchase(purchaseProductID, msStoreID, isSubscription,
#else
        PurchaseManager.Instance.Purchase(purchaseProductID, purchaseProductSku, isSubscription,
#endif
        purchaseProgressID.value,
        OnPurchaseSuccess,
        OnPurchaseFailed,
#if !UNITY_EDITOR && UNITY_ANDROID && !PLATFORM_AMAZON
        OnPurchasePending
#else
        OnPurchaseFailed
#endif
        );
    }

    private void OnPurchaseSuccess(PurchaseResult purchaseResult)
    {
        if(purchaseResult == null)
        {
            OnPurchaseFailed("PurchaseResult is null.");
            return;
        }

        EndPending();

        ClientIAPTransaction(true);

        string receipt = purchaseResult.Receipt;

#if UNITY_WSA
        // For compatibility. ex) "9NZ2W9JH0SN2|receipt" -> "com.bagelcode.slots1.coin.4|9NZ2W9JH0SN2|receipt"
        // wsa receipt = sku | storeID | receipt
        receipt = string.Format("{0}|{1}", purchaseProductSku ,receipt);
#endif
        string currencyCode = purchaseResult.CurrencyCode;
        string localPrice = purchaseResult.LocalPrice;
        long purchaseProgressId = 0L;
#if !UNITY_EDITOR && UNITY_ANDROID && !PLATFORM_AMAZON
        purchaseProgressId = purchaseResult.PurchaseProgressId;
#else
        if (agent != null)
            purchaseProgressId = purchaseProgressID?.value ?? 0L;
#endif

        if (ApplicationSettings.LogSystem())
            Debug.LogError("Purchase receipt : " + receipt);

        string signature = purchaseResult.Signature;
        if(ApplicationSettings.LogSystem())
            Debug.LogError("Purchase signature : " + signature);

        PurchaseManager.Instance.SetLastPurchaseReceipt(purchaseProductSku, receipt, signature, currencyCode, localPrice, purchaseProgressId);
#if DEV
        bool isDebugCrash = (PlayerPrefs.GetInt("DEBUG_PURCHASE_CRASH", 1) == 0);
        if(isDebugCrash && agent != null)
        {
            isSuccess.value = false;
            EndAction(true);
        }
        else
#endif
        {
            RequestPurchaseComplete( receipt, signature, currencyCode, localPrice );
        }
    }

    private void OnPurchaseFailed(string errorCode)
    {
        EndPending();
#if DEV
        if(ApplicationSettings.LogSystem())
            Debug.LogError("OnPurchaseError : " + errorCode);
#endif
        ClientIAPTransaction(false, errorCode);

        BlackboardQueryUtils.UpdatePurchaseErrorCode(errorCode);
        PurchaseManager.Instance.ClearLastPurchaseReceipt();

        BagelCodeClientAPI.Purchase_Failed( purchaseProgressID.value, false, errorCode,
        (response) =>
        {
            if(agent != null)
            {
                isSuccess.value = false;
                EndAction(true);
            }
        },
        (error) =>
        {
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "BI_POG_MULTIPLIER_EVENT_ID", 0);
            BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "BI_POG_SALE_EVENT_ID", 0);

            switch(error.errorCode)
            {
                case ClientModels.Error.DAILY_BOOST_ALREADY_EXIST_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DAILY_BOOST_ALREADY_EXIST", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                        ErrorPopupHandler.Instance.OpenError(info);


                    }
                    break;
                case ClientModels.Error.ALREADY_USED_SUBSCRIPTION_ERROR:
                    {
                        bool stringError = false;
                        ErrorPopupInfo info = new ErrorPopupInfo();

                        info.type = ErrorPopupType.OK;
                        info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_ALREADY_USED_SUBSCRIPTION_ERROR", out stringError);
                        info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                        ErrorPopupHandler.Instance.OpenError(info);

                        if(agent != null)
                        {
                            isSuccess.value = false;
                            EndAction(true);
                        }
                    }
                    break;
                default:
                    GlobalErrorHandler.GlobalError(error);
                    break;
            }
        });
    }

    private void OnPurchasePending(string encodedJson)
    {
        BeginPending(encodedJson);
    }

    private void BeginPending(string encodeJson)
    {
        Debug.LogError("BeginPending");

        pendingPopupObj = MetaObjectUtils.MakeScene("Popup Wait Pending Purchase Scene", PopupManager.Instance.transform.Find("Area"));
        pendingController = pendingPopupObj.GetComponent<PopupWaitPendingPurchaseController>();
        pendingController.SetCalcenCallback(OnCancelPending);
        pendingPopupObj.SetActive(true);
    }

    private void OnCancelPending()
    {
        Debug.LogError("OnCancelPending");
        // trigger user cancel

        // manual call clear purchase callbacks.
        PurchaseManager.Instance.EndPurchase();

        EndPending(true);

        string errorCode = "Pending wait. CUSTOM_USER_CANCEL";
#if DEV
        if(ApplicationSettings.LogSystem())
            Debug.LogError("OnPurchaseError : " + errorCode);
#endif
        ClientIAPTransaction(false, errorCode);

        BlackboardQueryUtils.UpdatePurchaseErrorCode(errorCode);
        PurchaseManager.Instance.ClearLastPurchaseReceipt();

        if(agent != null)
        {
            isSuccess.value = false;
            EndAction(true);
        }
    }

    private void EndPending(bool isCancel=false)
    {
        Debug.LogError("EndPending");
        // Destroy Loading Obj.
        // Unsubscribe cancel event.
        if(!isCancel && pendingPopupObj != null && pendingController != null)
        {
            pendingController.OnClose();
            pendingPopupObj = null;
            pendingController = null;
        }
    }

    private string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    private void RequestPurchaseComplete( string receipt, string signature, string currencyCode, string localPrice )
    {
        if(ApplicationSettings.LogSystem())
            Debug.LogError("Purchase RequestPurchaseComplete");

        bool useBucks = GetUseBucks();
        BagelCodeClientAPI.Purchase_Complete( purchaseProgressID.value, receipt, signature, currencyCode, localPrice, contextID.value, useBucks,
        (response) =>
        {
            if(ApplicationSettings.LogSystem())
                Debug.LogError("Purchase RequestPurchaseComplete Response(Success)");

            PurchaseManager.Instance.ReportPurchaseFulfillment(receipt, signature,
            (string success) =>
            {
                ClientItemAcquired();

                BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "purchaseResponse");
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "purchaseResponse");

                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.UpdateUserPurchase();
                BlackboardQueryUtils.ApplyPurchaseItems(response.itemUseResultList, response.userSyncInfo);
                BlackboardUtils.SetOrCreateValue<bool>(MainBlackboard.Get(), "hasPreBbbReward", response.hasPreBbbReward);

                // Vip Lounge
                BlackboardQueryUtils.UpdateVIPLoungeInfo(response.vipLoungeInfo);

                // Level Dash
                LevelUpDash.LevelUpDash.Utils.UpdateExpBoosterEndTimestamp(response.anyPurchaseBoosterEndTimestamp);
                // Vegas Bucks
                BlackboardQueryUtils.UpdateUserBucks(response.userBucks);

                var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, productBBValue.value);

                var productID = productBB.value.GetValue<int>("id");
                var productPrice = productBB.value.GetValue<double>("price");
                var productOrigPrice = productBB.value.GetValue<double>("originalPrice");

                BlackboardUtils.SetOrCreateValue<int>(bb, "productID", productID);
                BlackboardUtils.SetOrCreateValue<double>(bb, "price", productPrice);
                BlackboardUtils.SetOrCreateValue<double>(bb, "originalPrice", productOrigPrice);

                PurchaseManager.Instance.ClearLastPurchaseReceipt();

                if(agent != null)
                {
                    isSuccess.value = true;
                    EndAction(true);
                }
            });

            if (response.needToReloadCampaign)
            {
                ReloadCampaign();
            }
        },
        (error) =>
        {
            if(ApplicationSettings.LogSystem())
                Debug.LogError( string.Format("Purchase RequestPurchaseComplete Response({0})", error.errorCode));

            PurchaseManager.Instance.ClearLastPurchaseReceipt();
            PurchaseManager.Instance.ReportPurchaseFulfillment(receipt, signature,
            (string success) =>
            {
                switch(error.errorCode)
                {
                    case ClientModels.Error.DAILY_BOOST_ALREADY_EXIST_ERROR:
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();

                            info.type = ErrorPopupType.OK;
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_DAILY_BOOST_ALREADY_EXIST", out stringError);
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                            ErrorPopupHandler.Instance.OpenError(info);

                            if(agent != null)
                            {
                                isSuccess.value = false;
                                EndAction(true);
                            }
                        }
                        break;
                    case ClientModels.Error.ALREADY_USED_SUBSCRIPTION_ERROR:
                        {
                            bool stringError = false;
                            ErrorPopupInfo info = new ErrorPopupInfo();

                            info.type = ErrorPopupType.OK;
                            info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "ERROR_ALREADY_USED_SUBSCRIPTION_ERROR", out stringError);
                            info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);
                            ErrorPopupHandler.Instance.OpenError(info);

                            if(agent != null)
                            {
                                isSuccess.value = false;
                                EndAction(true);
                            }
                        }
                        break;
                    default:
                        GlobalErrorHandler.GlobalError(error);
                        break;
                }
            });
        });
    }

#if UNITY_WSA && !UNITY_EDITOR
    private void PurchaseWrapperForWindows()
    {
        BagelCodeClientAPI.GetAzureADCollectionsToken(
        (response) =>
        {
            var userId = BlackboardUtils.FindVariable<string>(null, "/me/userId").value;
            PurchaseManager.Instance.GetAndUpdateMicrosoftStoreIdKey(response.azureAdCollectionsToken, userId,
            (microsoftStoreIdKey) =>
            {
                Complete();
            });
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }
#endif

    // private void OpenSupportPopup()
    // {
    //     bool stringError = false;

    //     string supportUrl = BlackboardUtils.FindVariable<string>(null, "/values/misc/SUPPORT_PAGE_URL").value;

    //     ErrorPopupInfo info = new ErrorPopupInfo();
    //     info.type = ErrorPopupType.OK;
    //     info.text = StringTableUtils.GetString(StringTable.StringTableType.Global, "POPUP_PURCHASE_FAILED_SUPPORT_TEXT", supportUrl, out stringError);
    //     info.buttonText1 = StringTableUtils.GetString(StringTable.StringTableType.Global, "BUTTON_OKAY", out stringError);

    //     ErrorPopupHandler.Instance.OpenError(info);
    // }
    private void ReloadCampaign()
    {
        BagelCodeClientAPI.RequestCampaignList(
        (response) =>
        {
            BlackboardQueryUtils.UpdateInAppMessageList(response.inAppMessageList);
            BlackboardQueryUtils.LoadInAppMessageWebImages(CacheType.FileCache, true);
            BlackboardQueryUtils.UpdateSlotBannerList(response.slotBannerGroupList);
            IAMRouter.Instance.UpdateIAMInfo();
            EventSender.SendGlobalEvent("RefreshDeal");
            MessageDispatcher.Dispatch(MetaEventDefine.ON_META_UI_EVENT, new ParadoxNotion.EventData("OnRefreshSlotList"));
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
    }

    private void ClientIAPTransaction(bool isSuccess, string errorDesc = "")
    {
        if(agent == null) return;

        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, productBBValue.value);
        var lmTypeValue = BlackboardUtils.FindVariable<string>(agent, "_levelMultiplierType");
        BiEventUtils.IAPTransaction(productBB.value,
                                    contextID.value,
                                    isSuccess,
                                    isEvent == null ? false : isEvent.value,
                                    errorDesc,
                                    lmTypeValue == null ? "" : lmTypeValue.value
        );
    }

    private void ClientItemAcquired()
    {
        if(agent == null) return;

        var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, productBBValue.value);
        var lmTypeValue = BlackboardUtils.FindVariable<string>(agent, "_levelMultiplierType");
        BiEventUtils.ItemAcquired(  productBB.value,
                                    contextID.value,
                                    isEvent == null ? false : isEvent.value,
                                    lmTypeValue == null ? "" : lmTypeValue.value
        );
    }

        private bool GetUseBucks()
        {
            if (agent != null)
            {
                var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, productBBValue.value);
                if (productBB == null)
                    return false;

                return hasSufficientBucks.value && BlackboardQueryUtils.GetEnableBucksPurchase(productBB.value);
            }
            return false;
        }
}

}
