using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_email_popup : ActionTask<Blackboard>
{
    protected override void OnExecute()
    {
        string contextId = PlayerPrefs.GetString("VIP_CLUB_FUNNEL_CONTEXT_ID", "");

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["context_id"] = contextId;

        Analytics.CustomEvent("client_click_email_popup", customData);

        EndAction();
    }
}

}
