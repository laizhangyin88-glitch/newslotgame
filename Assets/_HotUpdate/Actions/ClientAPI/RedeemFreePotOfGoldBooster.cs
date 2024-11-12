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
public class RedeemFreePotOfGoldBooster : ActionTask<Blackboard>
{
    public BBParameter<string> product;
    public BBParameter<string> targetPurchaseId;
    public BBParameter<string> freeCoinBoosterEventId;

    public BBParameter<long> pogbEarnCoin;
    public BBParameter<double> pogbMultiplier;

    protected override string info { get { return "Redeem Free Coin Booster"; } }

    protected override void OnExecute()
    {
        var productId = BlackboardUtils.FindVariable<int>(agent, string.Format("{0}/id", product.value) );

        var targetID = BlackboardUtils.FindVariable<int>(agent, targetPurchaseId.value);
        int targetPurchaseID = targetID == null ? 0 : targetID.value;

        var eventId = BlackboardUtils.FindVariable<int>(agent, freeCoinBoosterEventId.value);
        
        if(productId == null || productId.value == 0 || eventId == null || eventId.value == 0)
        {
            EndAction(false);
            return;
        }

        BagelCodeClientAPI.POGBRedeem(productId.value, targetPurchaseID, eventId.value,
            (response) =>
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "purchaseResponse");
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.ApplyPurchaseItems(response.itemUseResultList, response.userSyncInfo);
                ClientAPI2Blackboard.Serialize(agent, response);

                var productPrice = BlackboardUtils.FindVariable<double>(agent, string.Format("{0}/price", product.value) );

                BlackboardUtils.SetOrCreateValue<int>(bb, "productID", productId.value);
                BlackboardUtils.SetOrCreateValue<double>(bb, "price", productPrice.value);

                ItemResultPogBooster pogbResult = response.itemUseResultList[0].result as ItemResultPogBooster;
                if(pogbResult != null)
                {
                    pogbEarnCoin.value = pogbResult.earnCredit;
                    pogbMultiplier.value = pogbResult.multiplier;
                }
                
                EndAction(true);
            },
            (error) =>
            {
                GlobalErrorHandler.GlobalError(error);
                EndAction(false);
            }
        );
    }
}

}
