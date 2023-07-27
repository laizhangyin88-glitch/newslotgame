using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_tier_up : ActionTask
{
    public BBParameter<int> tier;

    protected override void OnExecute()
    {
        Analytics.CustomEvent("client_tier_up", new Dictionary<string, object>
        {
            { "target_tier", tier.value }
        });

        string adjustEventId = string.Format("tier_up:{0}", BiEventUtils.AddZeroPadding(2, tier.value));
        AdjustManager.Instance.SendEvent(adjustEventId);

        EndAction();
    }
}

}
