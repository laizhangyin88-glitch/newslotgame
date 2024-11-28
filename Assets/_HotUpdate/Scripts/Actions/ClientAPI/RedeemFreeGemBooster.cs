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
public class RedeemFreeGemBooster : ActionTask<Blackboard>
{
    public BBParameter<string> product;
    public BBParameter<string> targetPurchaseId;
    public BBParameter<string> freeGemBoosterEventId;

	protected override string info { get { return "Redeem Free Gem Booster"; } }

	protected override void OnExecute()
	{
        var productId = BlackboardUtils.FindVariable<int>(agent, string.Format("{0}/id", product.value) );

        var targetID = BlackboardUtils.FindVariable<int>(agent, targetPurchaseId.value);
        int targetPurchaseID = targetID == null ? 0 : targetID.value;

        var eventId = BlackboardUtils.FindVariable<int>(agent, freeGemBoosterEventId.value);
        
        if(productId == null || productId.value == 0 || eventId == null || eventId.value == 0)
        {
        	EndAction(false);
	        return;
        }

		BagelCodeClientAPI.RedeemFreeGemBooster(productId.value, targetPurchaseID, eventId.value,
			(response) =>
			{
                var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "purchaseResponse");
                ClientAPI2Blackboard.Serialize(bb, response);
                BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
                BlackboardQueryUtils.ApplyPurchaseItems(response.itemUseResultList, response.userSyncInfo);
				ClientAPI2Blackboard.Serialize(agent, response);
                
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
