using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_purchase_early_access : ActionTask<Blackboard>
{
    private object IAMType;
    private object IAMId;
    private object isActionIAM;

    protected override void OnExecute()
    {
        var purchaseResponse = BlackboardUtils.FindVariable<Blackboard>(null, "/purchaseResponse").value;
        var itemUseResultList = BlackboardUtils.FindVariable<List<Blackboard>>(null, "/purchaseResponse/itemUseResultList").value;
        var product = BlackboardUtils.FindVariable<Blackboard>(agent, "product").value;
        var offerFreeTrial = BlackboardUtils.FindVariable<bool>(product, "offerFreeTrial").value;

        BiEventUtils.GetIAMData(agent, out IAMId, out IAMType, out isActionIAM);

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["type"] = "early_access";
        BiEventUtils.AppendCommonPurchaseEventData(customData, purchaseResponse, product, IAMType, IAMId, isActionIAM, false, false);
        BiEventUtils.AppendLevelMultiplierEventData(customData, "earlyAccess");

        Analytics.CustomEvent("client_purchase", customData);

        if(offerFreeTrial == false)
        {
            // AdjustManager.Instance.SendRevenueEvent("purchase", product.GetValue<double>("price"), purchaseResponse.GetValue<int>("purchaseId").ToString()); //TODO: Purchase ID as String

            ThirdPartyAnalyticsManager.SendPurchaseEvent(product, purchaseResponse);

            if(purchaseResponse.GetValue<int>("purchaseCount") == 1)
            {
                AdjustManager.Instance.SendEvent("first_purchase");
            }
        }

        EndAction();
    }
}

}

