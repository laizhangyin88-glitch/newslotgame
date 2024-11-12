using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class AE_client_click_epic_album_detail_page : ActionTask
{
    public BBParameter<string> type;
    public BBParameter<int> categoryID;
    public BBParameter<int> gameID;
    public BBParameter<string> contextID;

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["type"] = type.value;
        customData["category_id"] = categoryID.value;
        customData["game_id"] = gameID.value;
        customData["context_id"] = contextID.value;

        Analytics.CustomEvent("client_click_epic_album_detail_page", customData);

        EndAction();
    }
}

}
