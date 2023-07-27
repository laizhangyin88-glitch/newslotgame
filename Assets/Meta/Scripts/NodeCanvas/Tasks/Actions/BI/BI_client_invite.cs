using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using SlotMaker.Json;

namespace BagelCode.Tasks.Actions.BI
{

[Category("★ BagelCode/BI")]
public class BI_client_invite : ActionTask<Blackboard>
{
	private List<string> inviteType = new List<string>(){ "default", "club_member" };

	public BBParameter<string> invitableList;
	public BBParameter<int> type;

    protected override void OnExecute()
    {
		var userList = BlackboardUtils.FindVariable<List<string>>(agent, invitableList.value);

		if (userList != null && userList.value.Count != 0)
		{
	        Dictionary<string, object> customData = new Dictionary<string, object>();

	        customData["type"] = inviteType[type.value];
	        customData["target_user_id"] = SlotSimpleJson.SerializeObject(userList.value);
            customData["slot_enter_context_id"] = BiEventUtils.GetSlotEnterContextID();

	        Analytics.CustomEvent("client_invite", customData);
		}

        EndAction();
    }
}

}
