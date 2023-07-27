using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_club_join_request_popup : ActionTask<Blackboard>
{
    public BBParameter<int>  biAuthorityNumber;

    protected override string info
    {
        get { return string.Format("BI Client Click Join Request Popup {0}", biAuthorityNumber); }
    }

    protected override void OnExecute()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["user_authority"] = biAuthorityNumber.value;

        Analytics.CustomEvent("client_click_club_join_request_popup", customData);

        EndAction();
    }
}

}
