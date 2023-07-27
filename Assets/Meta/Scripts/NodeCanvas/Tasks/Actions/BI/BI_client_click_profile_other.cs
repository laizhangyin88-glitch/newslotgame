using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_click_profile_other : ActionTask<Blackboard>
{
    public BBParameter<string> fromType;
    public BBParameter<string> actionType;

    public BBParameter<string> userIDValue;

    protected override string info
    {
        get
        {
            return string.Format("client_click_profile_other({0})", actionType);
        }
    }

    protected override void OnExecute()
    {
        var userID = BlackboardUtils.FindVariable<string>(agent, userIDValue.value);
        var meID = BlackboardUtils.FindVariable<string>(MainBlackboard.Get(), "me/userId");

        if(meID.value != userID.value)
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = fromType.value;
            customData["action_type"] = actionType.value;
            customData["target_user_id"] = userID.value;

            Analytics.CustomEvent("client_click_profile_other", customData);
        }

        EndAction();
    }
}

}
