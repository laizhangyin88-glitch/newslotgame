using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_loading_funnel : ActionTask<Blackboard>
{
    public BBParameter<string> step;
    public BBParameter<string> loadingType;
    public BBParameter<string> contextID;

    protected override string info
    {
        get { return string.Format("BI client_loading_funnel({0}, {1})", step, loadingType); }
    }

    protected override void OnExecute()
    {
        if (string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        Dictionary<string, object> customData = new Dictionary<string, object>();

        // step: "ui_click", "common_download_start", "common_download_end", "content_download_start", "content_download_end", "finish", "retry"
        // type: meta_game_common", "meta_game_content"

        customData["type"] = loadingType.value;
        customData["step"] = step.value;
        customData["context_id"] = contextID.value;

        Analytics.CustomEvent("client_loading_funnel", customData);

        EndAction();
    }
}

}
