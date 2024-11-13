using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_fas : ActionTask
{
    public enum FasType
    {
        Pause,
        Resume
    };
    public FasType fasType = FasType.Pause;

    protected override void OnExecute()
    {
        var me = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;

        Analytics.CustomEvent("fas", new Dictionary<string, object>
        {
            { "sub_event_id", (fasType == FasType.Pause ? "pause" : "resume") }
        });

        EndAction();
    }
}

}
