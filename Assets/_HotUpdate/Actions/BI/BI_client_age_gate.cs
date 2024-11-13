using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_age_gate : ActionTask<Blackboard>
{
    public BBParameter<int> age;
    public BBParameter<string> type;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();
        if (type.value == "trigger")
            customData["age"] = null;
        else
            customData["age"] = age.value;

        customData["type"] = type.value;
        customData["age_limit"] = BlackboardUtils.GetOrCreateVariable<int>("/values/misc/AGE_GATE_THRESHOLD").value;
        Analytics.CustomEvent("client_age_gate", customData);
        EndAction();
    }
}

}
