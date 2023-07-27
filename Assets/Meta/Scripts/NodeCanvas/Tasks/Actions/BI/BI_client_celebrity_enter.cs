using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_celebrity_enter : ActionTask<Blackboard>
{
    public BBParameter<string> enterType;

    protected override string info
    {
        get { return string.Format("BI Celebrity {0}", enterType); }
    }

    protected override void OnExecute()
    {
        var customData = new Dictionary<string, object>();
        customData["action"] = enterType.value;
        Analytics.CustomEvent("client_celebrity_enter", customData);

        EndAction();
    }
}

}
