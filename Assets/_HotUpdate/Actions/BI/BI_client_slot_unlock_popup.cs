using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_slot_unlock_popup : ActionTask<Blackboard>
{
    public BBParameter<int> gameID;

    protected override string info
    {
        get { return string.Format("BI Client Slot Unlcok {0}", gameID); }
    }

    protected override void OnExecute()
    {
        var customData = new Dictionary<string, object>();
        customData["game_id"] = gameID.value;
        Analytics.CustomEvent("client_slot_unlock_popup", customData);

        EndAction();
    }
}

}
