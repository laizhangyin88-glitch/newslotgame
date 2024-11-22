using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_update_reward : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        var earnCoin = BlackboardUtils.FindVariable<long>(null, "/versionUpdateRewardCredit");

        Analytics.CustomEvent("client_update_reward", new Dictionary<string, object>
        {
            { "earn_coin", earnCoin.value }
        });

        earnCoin.value = 0;

        EndAction();
    }

}

}
