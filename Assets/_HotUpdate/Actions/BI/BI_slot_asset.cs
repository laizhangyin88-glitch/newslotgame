using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_slot_asset : ActionTask
{
    protected override void OnExecute()
    {
        var me = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;
        var game_id = BlackboardUtils.FindVariable<int>(null, "/enterGameInfo/gameId");
        var context_id = BlackboardUtils.FindVariable<string>(null, "/enterGameInfo/contextID");
        long timestamp = BagelCode.TimeUtils.GetTimeStamp();

        PlayerPrefs.SetString("asset_download_time", timestamp.ToString());

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["bet_zone_id"] = "free";
        customData["game_id"] = game_id.value;

        if (context_id != null && !string.IsNullOrEmpty(context_id.value))
        {
            customData["context_id"] = context_id.value;
        }

        Analytics.CustomEvent("client_slot_asset", customData);

        EndAction();
    }
}

}
