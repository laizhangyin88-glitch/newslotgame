using UnityEngine;
using System.Text;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_inbox_iam : ActionTask<Blackboard>
{
    public BBParameter<string> iamIDValue;
    public BBParameter<string> contextID;

    protected override void OnExecute()
    {
        if(string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        var iamID = BlackboardUtils.FindVariable<int>(agent, iamIDValue.value);
        var iamBB = BlackboardQueryUtils.GetIAMBlackboard(iamID.value);
        
        if(iamBB != null)
        {
            var iamType = iamBB.GetValue<ClientModels.InAppMessageType>("type");

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["iam_id"] = iamID.value;
            customData["iam_type"] = iamType.ToString();
            customData["context_id"] = contextID.value;

            Analytics.CustomEvent("client_click_inbox_iam", customData);
        }

        EndAction();
    }
}

}
