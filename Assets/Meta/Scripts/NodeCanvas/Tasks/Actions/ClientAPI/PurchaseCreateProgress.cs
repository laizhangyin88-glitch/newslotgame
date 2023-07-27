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
    public class PurchaseCreateProgress : ActionTask<Blackboard>
    {
        public BBParameter<string> productBBValue;
        public BBParameter<string> targetPurchaseIDValue;
        public BBParameter<string> contextID;

        public BBParameter<bool> saveAsShopEventFlag;

        // Passive Event ID.
        public BBParameter<int> piggyBankEventID;
        public BBParameter<int> piggyBankSaleEventID;
        public BBParameter<int> potOfGoldBoosterEventID;
        public BBParameter<int> creditWheelEventID;
        public BBParameter<int> creditAllWheelEventID;
        public BBParameter<int> vipDealInfoID;
        public BBParameter<string> vipDealUUID;
        public BBParameter<int> userGroupID;
        public BBParameter<int> metaGameEventID;
        public BBParameter<int> coinShopMultiplierEventID;
        public BBParameter<int> dailyWheelEventID;
        public BBParameter<int> gemShopMultiplierEventID;
        public BBParameter<int> gemBabShopMultiplyEventID;
        public BBParameter<int> gemBabPromotionShopMultiplyEventID;
        public BBParameter<int> voucherShopMultiplyEventID;
        public BBParameter<int> gemWheelEventID;
        public BBParameter<int> gemAllWheelEventID;
        public BBParameter<int> tierUpShopMultiplyEventID;
        public BBParameter<int> spinDealID;
        public BBParameter<List<int>> ticketIDList;
        public BBParameter<int> seasonPassEventID;

        public BBParameter<long> saveAsPurchaseProgressID;
        public BBParameter<bool> isSuccess;
        public BBParameter<bool> hasSufficientBucks;

        private string lmTypeValue = "";

        protected override string info
        {
            get
            {
                return string.Format("BI Item Click\nCreate Purchase Progress {0}\nBI Iap Create Progress", productBBValue);
            }
        }

        protected override void OnExecute()
        {
            // BI Click Event.
            saveAsShopEventFlag.value = IsShopEvent();
            lmTypeValue = BlackboardUtils.FindVariable<string>(agent, "_levelMultiplierType")?.value ?? "";
            ClientItemClick();

            saveAsPurchaseProgressID.value = 0;
            isSuccess.value = false;
            if (BlackboardQueryUtils.CheckPurchaseProhibitedRegion())
            {
                EndAction(true);
                return;
            }

            var productId = BlackboardUtils.FindVariable<int>(agent, string.Format("{0}/id", productBBValue.value));

            var targetID = BlackboardUtils.FindVariable<int>(agent, targetPurchaseIDValue.value);
            int targetPurchaseID = targetID == null ? 0 : targetID.value;

            var iamIDVariable = BlackboardUtils.FindVariable<int>(agent, "iamId");
            int iamID = iamIDVariable == null ? 0 : iamIDVariable.value;
            var iamTriggerTypeVaraible = BlackboardUtils.FindVariable<InAppMessageTriggerType>(agent, "triggerType");
            string iamTriggerType = iamTriggerTypeVaraible == null ? "" : iamTriggerTypeVaraible.value.ToString();

            if (productId != null)
            {
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "BI_POG_MULTIPLIER_EVENT_ID", piggyBankEventID.value);
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "BI_POG_SALE_EVENT_ID", piggyBankSaleEventID.value);

                BagelCodeClientAPI.Purchase_CreateProgress(productId.value,
                                                            targetPurchaseID,
                                                            piggyBankEventID.value,
                                                            potOfGoldBoosterEventID.value,
                                                            creditWheelEventID.value,
                                                            creditAllWheelEventID.value,
                                                            metaGameEventID.value,
                                                            coinShopMultiplierEventID.value,
                                                            gemShopMultiplierEventID.value,
                                                            dailyWheelEventID.value,
                                                            gemBabShopMultiplyEventID.value,
                                                            gemBabPromotionShopMultiplyEventID.value,
                                                            voucherShopMultiplyEventID.value,
                                                            gemWheelEventID.value,
                                                            gemAllWheelEventID.value,
                                                            tierUpShopMultiplyEventID.value,
                                                            spinDealID.value,
                                                            ticketIDList.value,
                                                            seasonPassEventID.value,
                                                            saveAsShopEventFlag.value,
                                                            vipDealUUID.value,
                                                            vipDealInfoID.value,
                                                            userGroupID.value,
                                                            iamID,
                                                            iamTriggerType,
                (response) =>
                {
                    if (agent != null)
                    {
                        saveAsPurchaseProgressID.value = response.purchaseProgressId;
                        isSuccess.value = true;
                        hasSufficientBucks.value = response.hasSufficientBucks;
                        ClientIapCreateProgress(true);
                        EndAction(true);
                    }
                },
                (error) =>
                {
                    BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "BI_POG_MULTIPLIER_EVENT_ID", 0);
                    BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "BI_POG_SALE_EVENT_ID", 0);

                    if (agent != null)
                    {
                        isSuccess.value = false;
                        ClientIapCreateProgress(false, error.errorCode.ToString());
                        EndAction(true);
                    }
                });
            }
            else
            {
                Debug.LogError("No ItemId found." + agent.gameObject.name);
            }
        }

        private void ClientItemClick()
        {
            if (string.IsNullOrEmpty(contextID.value))
                contextID.value = BiEventUtils.GenerateContextID();

            var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, productBBValue.value);
            
            BiEventUtils.ItemClick( productBB.value,
                                    contextID.value,
                                    saveAsShopEventFlag == null ? false : saveAsShopEventFlag.value,
                                    lmTypeValue
            );
        }

        private bool IsShopEvent()
        {
            if (coinShopMultiplierEventID != null && coinShopMultiplierEventID.value > 0) return true;
            if (gemShopMultiplierEventID != null && gemShopMultiplierEventID.value > 0) return true;
            if (creditWheelEventID != null && creditWheelEventID.value > 0) return true;
            if (dailyWheelEventID != null && dailyWheelEventID.value > 0) return true;
            if (piggyBankEventID != null && piggyBankEventID.value > 0) return true;
            if (piggyBankSaleEventID != null && piggyBankSaleEventID.value > 0) return true;
            if (potOfGoldBoosterEventID != null && potOfGoldBoosterEventID.value > 0) return true;
            if (creditAllWheelEventID != null && creditAllWheelEventID.value > 0) return true;
            if (gemBabShopMultiplyEventID != null && gemBabShopMultiplyEventID.value > 0) return true;
            if (gemBabPromotionShopMultiplyEventID != null && gemBabPromotionShopMultiplyEventID.value > 0) return true;
            if (voucherShopMultiplyEventID != null && voucherShopMultiplyEventID.value > 0) return true;

            return false;
        }

        private void ClientIapCreateProgress(bool isSuccess, string errorCode = "")
        {
            if (string.IsNullOrEmpty(contextID.value))
                contextID.value = BiEventUtils.GenerateContextID();

            var productBB = BlackboardUtils.FindVariable<Blackboard>(agent, productBBValue.value);

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextID.value;

            if (productBB != null && productBB.value != null)
            {
                var price = BlackboardUtils.FindVariable<double>(productBB.value, "price");
                if (price != null) customData["price"] = (int)((price.value + 0.00001) * 100);

                var productID = BlackboardUtils.FindVariable<int>(productBB.value, "id");
                if (productID != null) customData["product_id"] = productID.value;

                string type = "";
                var itemList = BlackboardUtils.FindVariable<List<Blackboard>>(productBB.value, "itemList");
                if (itemList != null)
                {
                    var itemType = BlackboardUtils.FindVariable<ItemType>(itemList.value[0], "itemType");
                    if (itemType != null) type = BiEventUtils.GetStringFromItemType(itemType.value);
                }
                else
                {
                    var itemType = BlackboardUtils.FindVariable<string>(productBB.value, "itemType");
                    if (itemType != null) type = itemType.value;
                }
                customData["type"] = type;
            }

            customData["shop_event_flag"] = IsShopEvent();
            customData["status"] = isSuccess ? "success" : "fail";
            customData["error_code"] = isSuccess ? "" : errorCode;

            BiEventUtils.AppendLevelMultiplierEventData(customData, string.IsNullOrEmpty(lmTypeValue) ? BiEventUtils.GetLevelMultiplierFromItemType(productBB.value) : lmTypeValue);

            Analytics.CustomEvent("client_iap_create_progress", customData);
        }
    }
}
