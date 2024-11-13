using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class FriendOnlineList : ActionTask 
{
	protected override string info { get { return "Request FriendOnlineList"; } }

	protected override void OnExecute()
	{
		BagelCodeClientAPI.FriendOnlineList(
		(response) => 
		{
			BlackboardUtils.DestroyBlackboard(MainBlackboard.Get(), "friendOnlineList");
			var bb = BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "friendOnlineList");
			ClientAPI2Blackboard.Serialize(bb, response);

			if(response.error == BagelCode.ClientModels.Error.OK)
			{
				EndAction(true);
			}
			else
			{
				EndAction(false);
			}
		},
		(error) =>
		{
			EndAction(false);
		}
		);
	}

}

}