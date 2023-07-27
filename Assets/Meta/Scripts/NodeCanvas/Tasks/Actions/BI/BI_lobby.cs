using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_lobby : ActionTask
{
    protected override void OnExecute()
    {
        var me = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;

        Analytics.CustomEvent("client_lobby", new Dictionary<string, object>{});
        AdjustManager.Instance.SendEvent("lobby");

        EndAction();
    }
}

}
