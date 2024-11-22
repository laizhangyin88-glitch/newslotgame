using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_update_nudge : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        int recentVersion = BlackboardUtils.FindVariable<int>(null, "/recentClientNumberVersion").value;

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["nudge_client_version"] = recentVersion.ToString();

        Analytics.CustomEvent("client_click_update_nudge", customData);

        EndAction();
    }
}

}
