using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_club_list_tab : ActionTask<Blackboard>
{
    public BBParameter<string> listType;

    protected override string info
    {
        get { return string.Format("BI Client Click List Tab {0}", listType); }
    }

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["list_type"] = listType.value;

        Analytics.CustomEvent("client_click_club_list_tab", customData);

        EndAction();
    }
}

}
