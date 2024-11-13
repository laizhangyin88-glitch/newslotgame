using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_hot_deal : ActionTask<Blackboard>
{
    public BBParameter<string> contextID;
    public BBParameter<int> iamID;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["context_id"] = contextID.value;
        customData["iam_id"] = iamID.value;

        Analytics.CustomEvent("client_click_hot_deal", customData);

        EndAction();
    }
}

}
