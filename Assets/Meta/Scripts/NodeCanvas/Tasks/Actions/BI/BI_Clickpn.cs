using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_Clickpn : ActionTask
{
    protected override void OnExecute()
    {
        var info = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "/pushInfo");

        if (info != null && info.value != null)
        {
            BlackboardQueryUtils.BI_Click_PN(
                info.value.GetValue<string>("type"),
                info.value.GetValue<string>("comment"),
                info.value.GetValue<long>("ts"),
                info.value.GetValue<string>("templateId"),
                info.value.GetValue<string>("couponCode")
            );

            // string pnId = string.Empty;
            // Variable pnIdVariable = BlackboardUtils.FindVariable<string>("/lastNotificationId");
            // if (pnIdVariable != null && pnIdVariable.value != null)
            // {
            //     pnId = pnIdVariable.value as string;
            // }

            // string onesignalId = string.Empty;
            // Variable onesignalIdVariable = BlackboardUtils.FindVariable<string>("/oneSignalId");
            // if (onesignalIdVariable != null && onesignalIdVariable.value != null)
            // {
            //     onesignalId = onesignalIdVariable.value as string;
            // }

            // Analytics.CustomEvent("client_click_pn", new Dictionary<string, object>
            // {
            //     { "message_type", info.value.GetValue<string>("type") },
            //     { "comment", info.value.GetValue<string>("comment") },
            //     { "distance_from_receiving_pn", info.value.GetValue<long>("ts") },
            //     { "template_id", info.value.GetValue<string>("templateId")},
            //     { "pn_id", pnId },
            //     { "onesignal_id", onesignalId },
            //     { "coupon_code", info.value.GetValue<string>("couponCode")}
            // });

            // BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "pushInfo");
        }

        EndAction();
    }
}

}
