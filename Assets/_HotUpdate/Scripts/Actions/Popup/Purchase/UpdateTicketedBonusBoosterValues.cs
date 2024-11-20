using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Popup/Purchase")]
    public class UpdateTicketedBonusBoosterValues : ActionTask<Blackboard>
    {
        public BBParameter<Blackboard> product;
        public BBParameter<Blackboard> boostProduct;
        public BBParameter<InAppMessageType> iamType;

        protected override void OnExecute()
        {
            // Is Purchase Result
            var purchaseResponse = BlackboardUtils.FindVariable<Blackboard>("/purchaseResponse")?.value;

            var purchasedItem = product.value.GetValue<List<Blackboard>>("itemList")[0];
            var purchaseResponseItem = purchaseResponse.GetValue<List<Blackboard>>("itemUseResultList")[0];
            int gameId = purchasedItem.GetValue<int>("gameId");

            long baseBet = IAMUtils.CalculateBetLevelMultiplier(purchasedItem.GetValue<long>("bet"), purchasedItem.GetValue<long>("extraBet"), BlackboardQueryUtils.GetRawBaseBet(gameId));
            var eventMultiplierNumerator = product.value.GetVariable<long>("eventMultiplierNumerator");
            if (eventMultiplierNumerator != null)
                baseBet = NumberUtils.GetMultiplierNumeratorValue(baseBet, eventMultiplierNumerator.value);

            var popupBB = agent.GetComponent<Blackboard>();
            BlackboardUtils.SetOrCreateValue(popupBB, "purchasedCredit", 0L);  //popupBB.AddVariable("purchasedCredit", purchaseResponseItem.GetValue<long>("earnCredit"));
            BlackboardUtils.SetOrCreateValue(popupBB, "gameId", gameId);
            BlackboardUtils.SetOrCreateValue(popupBB, "targetPurchaseID", purchaseResponse.GetValue<int>("purchaseId"));
            BlackboardUtils.SetOrCreateValue(popupBB, "product", boostProduct.value);

            BlackboardUtils.SetOrCreateValue(popupBB, "_ticketIDList", GetRewardTicketIdList(purchaseResponse));
            BlackboardUtils.SetOrCreateValue(popupBB, "baseBet", baseBet);
            BlackboardUtils.SetOrCreateValue(popupBB, "titleTextKey", GetTicketedBonusTitleKey());
            BlackboardUtils.SetOrCreateValue(popupBB, "isInbox", GetIsInbox());

            EndAction();
        }

        private string GetTicketedBonusTitleKey()
        {
            switch (iamType.value)
            {
                case InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP:
                    return "POPUP_TICKETED_BONUS_BOOSTER_BAB_TITLE";
                case InAppMessageType.SUPER_BONUS_PURCHASE_POPUP:
                    return "POPUP_TICKETED_BONUS_BOOSTER_SPB_TITLE";
                case InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP:
                    return "POPUP_TICKETED_BONUS_BOOSTER_INS_TITLE";
            }
            return "POPUP_CONTROL_SPIN_BOOSTER_TITLE";
        }

        private bool GetIsInbox()
        {
            switch (iamType.value)
            {
                case InAppMessageType.BUY_A_BONUS_PURCHASE_POPUP:
                case InAppMessageType.SUPER_BONUS_PURCHASE_POPUP:
                case InAppMessageType.INSTANT_BONUS_PURCHASE_POPUP:
                    return false;
                default:
                    return true;
            }
        }

        private List<int> GetRewardTicketIdList(Blackboard purchaseResponse)
        {
            List<Blackboard> itemUseResultList = purchaseResponse.GetValue<List<Blackboard>>("itemUseResultList");
            List<int> ticketIdList = new List<int>();
            if (itemUseResultList != null && itemUseResultList.Count > 0)
            {
                for (int i = 0; i < itemUseResultList.Count; ++i)
                {
                    var ticketId = itemUseResultList[i].GetVariable<int>("ticketId");
                    if (ticketId != null && ticketId.value != 0)
                        ticketIdList.Add(ticketId.value);
                }
            }
            return ticketIdList;
        }
    }
}