using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_club_join : ActionTask<Blackboard>
{
    public BBParameter<string> fromType;
    public BBParameter<string> contextID;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        // "club_member_list", "club_popup"
        customData["type"] = fromType.value;
        customData["context_id"] = contextID.value;
        Analytics.CustomEvent("client_click_club_join", customData);

        EndAction();
    }
}

}
