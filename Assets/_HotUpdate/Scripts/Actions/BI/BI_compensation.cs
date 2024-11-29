using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

// Deprecated

[Category("★ BagelCode/BI")]
public class BI_compensation : ActionTask<Blackboard> 
{
    protected override void OnExecute()
    {
        var inboxInfo = BlackboardUtils.FindVariable<Blackboard>(agent, "inboxInfo").value;
        var response = BlackboardUtils.FindVariable<Blackboard>(null, "/inboxResponse/" + string.Format("ID_{0}", inboxInfo.GetValue<int>("id")));

        Analytics.CustomEvent("client_compensation", new Dictionary<string, object>
        {
            { "earn_coin", response.value.GetValue<long>("credit") },
            { "game_id", inboxInfo.GetValue<int>("gameId") },
            { "bonus_id", inboxInfo.GetValue<int>("bonusId") }
        });

        EndAction();
    }
}

}
