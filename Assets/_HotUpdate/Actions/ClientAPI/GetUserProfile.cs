using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]

public class GetUserProfile : ActionTask <Blackboard> 
{
	public BBParameter<string> userId;
	public BBParameter<bool> isMe;

	protected override string info 
	{ 
		get 
		{ 
			return string.Format("Request userinfo {0}", userId);
		} 
	}
	
	protected override void OnExecute()
	{
		if (string.IsNullOrEmpty(userId.value) == false)
		{
			BagelCodeClientAPI.GetUserProfile(userId.value,
			(response) =>
			{
                if(agent != null)
                {
                    var bb = BlackboardUtils.GetOrCreateBlackboard(agent, "userInfoResponse"); 
                    ClientAPI2Blackboard.Serialize(bb, response);
                    agent.SetValue("recordList", BlackboardUtils.FindVariable<List<Blackboard>>(bb, "recordList").value);
                    agent.SetValue("facebookFriendCount", BlackboardUtils.FindVariable<int>(bb, "facebookFriendCount").value);
                    agent.SetValue("acceptedFriendCount", BagelCode.BlackboardQueryUtils.GetFriendList(true).Count);

                    BlackboardUtils.SetOrCreateValue(agent, "isBlocked", BlackboardUtils.FindVariable<bool>(bb, "isBlocked").value);

                    //if(!isMe.value)
                    agent.SetValue("_userInfo", BlackboardUtils.GetOrCreateBlackboard(bb, "user"));

                    EndAction(true);
                }
			},
			(error) =>
			{
                GlobalErrorHandler.GlobalError(error);
				// EndAction(false);
			});
		}
		else
		{
			// Debug.LogError("No UserId found." + agent.gameObject.name);
		}
	}
}

}
