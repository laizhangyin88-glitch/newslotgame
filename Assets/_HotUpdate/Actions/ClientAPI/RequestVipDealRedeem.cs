using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.ClientAPI
{
    [Category("★ BagelCode/ClientAPI")]
    public class RequestVipDealRedeem : ActionTask <Blackboard>
    {
        public BBParameter<string> vipDealUUID;
        public BBParameter<int> vipDealInfoID;

        public BBParameter<bool> isSuccess;

        protected override string info { get { return "Request Vip Deal Redeem"; } }

        protected override void OnExecute()
        {
            BagelCodeClientAPI.VipDealRedeem(vipDealUUID.value, vipDealInfoID.value,
            (response) =>
            {
                BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "purchaseResponse");
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "purchaseResponse");

                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.ApplyPurchaseItems(response.itemUseResultList, response.userSyncInfo);

                if(agent != null)
                {
                    isSuccess.value = true;
                    EndAction(true);
                }
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
            });
        }
    }
}

