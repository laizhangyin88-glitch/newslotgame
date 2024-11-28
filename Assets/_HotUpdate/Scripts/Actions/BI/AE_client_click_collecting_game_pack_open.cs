using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class AE_client_click_collecting_game_pack_open : ActionTask
{
    public BBParameter<int> packID;
    public BBParameter<int> totalPackAmount;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["pack_id"] = (long)packID.value;
        customData["total_pack_amount"] = totalPackAmount.value;

        Analytics.CustomEvent("client_click_collecting_game_pack_open", customData);

        EndAction();
    }
}

}
