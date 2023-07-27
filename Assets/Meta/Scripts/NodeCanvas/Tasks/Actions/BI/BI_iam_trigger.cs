using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_iam_trigger : ActionTask<Blackboard>
{
    public BBParameter<string> iamInfo;

	public BBParameter<string> iamTriggerType;
    public BBParameter<string> iamBGURL;
    public BBParameter<string> contextID;

    protected override void OnExecute()
    {
        var iamInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, iamInfo.value);
        if(iamInfoBB == null || iamInfoBB.value == null)
        {
#if DEV
            Debug.LogError("iamInfo is null. fix me plz.");
#endif
            EndAction(true);
            return;
        }

    	var id = BlackboardUtils.FindVariable<int>(iamInfoBB.value, "id");
        var type = BlackboardUtils.FindVariable<ClientModels.InAppMessageType>(iamInfoBB.value, "type");
        var name = BlackboardUtils.FindVariable<string>(iamInfoBB.value, "name");
        int promotedGameID = BlackboardUtils.FindVariable<int>(iamInfoBB.value, "promotedGameId")?.value ?? 0;
        int priority = BlackboardUtils.FindVariable<int>(iamInfoBB.value, "priority")?.value ?? 0;

        var triggerType = BlackboardUtils.FindVariable<ClientModels.InAppMessageTriggerType>(agent, iamTriggerType.value);

    	if(id == null || triggerType == null)
        {
#if DEV
            Debug.LogError("(id or triggerType) is null. fix me plz.");
#endif
    		EndAction(true);
            return;
        }

        if(string.IsNullOrEmpty(contextID.value))
            contextID.value = BiEventUtils.GenerateContextID();

        Dictionary<string, object> customData = new Dictionary<string, object>();

        customData["iam_id"] = id.value;
        customData["iam_type"] = type.value.ToString();
        customData["iam_name"] = name.value;
        customData["iam_trigger_type"] = triggerType.value.ToString();
        customData["is_action"] = triggerType.value == ClientModels.InAppMessageTriggerType.UNKNOWN ? true : false;
        customData["context_id"] = contextID.value;
        customData["image_url"] = iamBGURL.value;
        customData["game_id"] = promotedGameID;
        customData["priority"] = priority;
        BiEventUtils.AppendLevelMultiplierEventData(customData, "coin");
        Analytics.CustomEvent("client_iam_trigger", customData);

        EndAction();
    }
}

}
