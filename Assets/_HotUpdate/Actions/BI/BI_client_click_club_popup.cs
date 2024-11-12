using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_club_popup : ActionTask<Blackboard>
{
    public BBParameter<string> targetUserIDValue;
    public BBParameter<string> clubIDValue;
    public BBParameter<string> clubNameValue;
    public BBParameter<string> clubLevelValue;

    protected override void OnExecute()
    {
        var targetUserID    = BlackboardUtils.FindVariable<string>(agent, targetUserIDValue.value);
        var clubID          = BlackboardUtils.FindVariable<long>(agent, clubIDValue.value);
        var clubName        = BlackboardUtils.FindVariable<string>(agent, clubNameValue.value);
        var clubLevel       = BlackboardUtils.FindVariable<int>(agent, clubLevelValue.value);

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["target_user_id"] = targetUserID.value;
        customData["club_id"] = clubID.value;
        customData["club_name"] = clubName.value;
        customData["club_level"] = clubLevel.value;

        Analytics.CustomEvent("client_click_club_popup", customData);

        EndAction();
    }
}

}
