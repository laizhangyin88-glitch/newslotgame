using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class UserInvitableList : ActionTask<Blackboard>
{
	protected override string info { get { return "Request InvitableUserList"; } }

	protected override void OnExecute()
	{
		BagelCodeClientAPI.InvitableUserList(
		(response) => 
		{
			if(agent != null)
			{
				var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "invitableResponse");
				ClientAPI2Blackboard.Serialize(bb, response);

				EndAction(true);
			}
		},
		(error) =>
		{
			if(agent != null)
				EndAction(false);
		}
		);
	}

}

}
