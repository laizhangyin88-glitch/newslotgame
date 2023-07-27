using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class Lobby : ActionTask
{
	protected override string info { get { return "Request Lobby"; } }

	protected override void OnExecute()
	{
		if (ApplicationSettings.LogTest())
        	Debug.Log("Request Lobby");

		BagelCodeClientAPI.Lobby(
		(response) =>
		{
			ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
            BlackboardUtils.GetOrCreateBlackboard(MainBlackboard.Get(), "shortcut");

            BlackboardQueryUtils.UpdateCelebInfo();
            BlackboardQueryUtils.UpdateGameAndSlotInfoFromLobby();
            IAMRouter.Instance.UpdateIAMInfo();

            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.ApplyUserSyncInfo();
			BlackboardQueryUtils.CreateVIPLoungePrevBadgeCount();

			// For bi client_lobby_status.
			BiEventUtils.SetLobbyStatusData(response);

            EndAction(true);
		},
		(error) =>
		{
            GlobalErrorHandler.GlobalError(error);
		});
	}
}

}
