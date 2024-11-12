using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_all_in : ActionTask
{
    public BBParameter<string> contextID;

    protected override void OnExecute()
    {
        var content = ContentBlackboard.Get();
        var game    = BlackboardUtils.FindVariable<Blackboard>(null, "./game");
        if(game == null || game.value == null)
        {
            EndAction();
            return;
        }

        if(string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["bet_zone_id"] = "free";
        customData["game_id"] = game.value.GetValue<int>("gameId");
        customData["bet"] = content.GetValue<long>("betCredit");
        customData["context_id"] = contextID.value;

        BiEventUtils.AppendLevelMultiplierEventData(customData, "coin");

        Analytics.CustomEvent("client_all_in", customData);

        EndAction();
    }
}

}
