using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

public enum BIPolicyType
{
	trigger,
	agree
}

[Category("★ BagelCode/BI")]
public class BI_privacy_policy : ActionTask
{
	public BBParameter<BIPolicyType> type;

    protected override void OnExecute()
    {
        var version = BlackboardUtils.FindVariable<int>(MainBlackboard.Get(), "/values/misc/POLICY_VERSION");

        if (version == null)
        {
            EndAction(false);
            return;
        }

        var eventData = new Dictionary<string, object>();
        eventData["type"] = type.value.ToString();
    	eventData["version"] = version.value;

        Analytics.CustomEvent("client_privacy_policy_popup", eventData);
        EndAction();
    }
}

}	
