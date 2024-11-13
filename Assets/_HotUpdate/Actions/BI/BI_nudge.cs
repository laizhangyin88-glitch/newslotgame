using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_nudge : ActionTask
{
    public BBParameter<string> targetUserId;

    protected override void OnExecute()
    {
        Analytics.CustomEvent("client_nudge", new Dictionary<string, object>
        {
            { "target_user_id", targetUserId.value }
        });

        EndAction();
    }
}

}
