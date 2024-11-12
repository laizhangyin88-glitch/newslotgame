using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Tasks.Actions.ClientAPI
{

[Category("★ BagelCode/ClientAPI")]
public class InboxList : ActionTask
{
	protected override string info { get { return "Request InboxList"; } }

	protected override void OnExecute()
	{
        BagelCodeClientAPI.InboxList(
		(response) =>
		{
            BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "inboxList");
		    BlackboardUtils.DestroyBlackboardList(MainBlackboard.Get(), "inboxBannerList");
			ClientAPI2Blackboard.Serialize(MainBlackboard.Get(), response);
		    BlackboardQueryUtils.UpdateCollectAllCredit();
            EndAction(true);
		},
		(error) =>
		{
			Debug.Log("Error : " + error.errorCode.ToString());
            GlobalErrorHandler.GlobalError(error);
		});
	}
}

}
