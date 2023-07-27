using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_iam_click : ActionTask<Blackboard>
{
	public BBParameter<string> iamId;
    public BBParameter<string> iamType;
    public BBParameter<string> iamName;
	public BBParameter<string> iamTriggerType;
    public BBParameter<string> iamBGURL;
    public BBParameter<string> contextID;

    public BBParameter<bool> isCloseButton;
    public BBParameter<string> componentAction;

    protected override void OnExecute()
    {
    	var id = BlackboardUtils.FindVariable<int>(agent, iamId.value);
        var type = BlackboardUtils.FindVariable<ClientModels.InAppMessageType>(agent, iamType.value);
        var name = BlackboardUtils.FindVariable<string>(agent, iamName.value);
    	var triggerType = BlackboardUtils.FindVariable<ClientModels.InAppMessageTriggerType>(agent, iamTriggerType.value);
        var componentActionBB = BlackboardUtils.FindVariable<Blackboard>(agent, componentAction.value);

    	if(id == null || triggerType == null)
    		EndAction(false);

        if(string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["iam_id"] = id.value;
        customData["iam_type"] = type.value.ToString();
        customData["iam_trigger_type"] = triggerType.value.ToString();
        customData["iam_name"] = name.value;

        customData["context_id"] = contextID.value;
        customData["image_url"] = iamBGURL.value;

        if(isCloseButton.value || componentActionBB == null)
        {
            customData["is_action"] = false;
            customData["action"] = "close";
        }
        else
        {
            bool isClick = true;

            var actionType = BlackboardUtils.FindVariable<ClientModels.ActionType>(componentActionBB.value, "type");
            if(actionType.value == ClientModels.ActionType.NONE)
                isClick = false;

            customData["is_action"] = true;
            customData["action"] = isClick ? "click" : "close";
        }

        Analytics.CustomEvent("client_iam_click", customData);

        EndAction();
    }
}

}
