using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Invite")]
public class GetInviteFromType : ActionTask<Blackboard> 
{
	public BBParameter<string> inviteInfo;
	[BlackboardOnly]
	public BBParameter<string> fromType;

	protected override void OnExecute()
	{
		var info = BlackboardUtils.FindVariable<Blackboard>(agent, inviteInfo.value);

		if (info != null && info.value != null)
		{
			InviteType type = info.value.GetValue<InviteType>("inviteType");

			switch(type)
			{
				case InviteType.FRIEND:
				{
					fromType.value = "invite";
					break;
				}
				case InviteType.CLUB:
				{
					fromType.value = "club_member_invite";
					break;
				}
				default:
				{
					fromType.value = "invite";
					break;
				}
			}

			EndAction();
		}
		else
		{
			EndAction(false);
		}
	}
}

}
