using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class RateUs : ActionTask <Blackboard> 
{
    public BBParameter<int> rating;

	protected override string info 
	{ 
		get 
		{ 
			return string.Format("Rate Us");
		} 
	}
	protected override void OnExecute()
	{
        int clientVersion = ApplicationSettings.GetClientVersionNumber();
        BlackboardUtils.FindVariable<int>(null, "/me/lastRatedClientNumberVersion").value = clientVersion;
        BagelCodeClientAPI.Rate(rating.value,
        (response) =>
        {
            ClientAPI2Blackboard.Serialize(agent, response);
            BlackboardQueryUtils.UpdateUserSyncInfo(response.userSyncInfo, response.serverTime);
            BlackboardQueryUtils.AddCoins(response.earnCredit);
            EndAction(true);
        },
        (error) =>
        {
            GlobalErrorHandler.GlobalError(error);
        });
	}
}

}
