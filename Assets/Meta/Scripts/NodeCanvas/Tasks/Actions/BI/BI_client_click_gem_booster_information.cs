using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_gem_booster_information : ActionTask<Blackboard>
{
    public BBParameter<string> contextID;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["context_id"] = contextID.value;

        Analytics.CustomEvent("client_click_gem_booster_information", customData);

        EndAction();
    }
}

}
