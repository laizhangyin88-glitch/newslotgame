using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Friend")]
public class CheckFriendRequest : ActionTask<Blackboard>
{
	[BlackboardOnly]
    public BBParameter<string> userId;
    public BBParameter<bool> saveAs;

	protected override string info
	{
		get { return "Check Friend Request"; }
	}

	protected override void OnExecute()
	{
		
        List<Blackboard> requestedList = BlackboardQueryUtils.GetFriendList(false);
		saveAs.value = BlackboardQueryUtils.IsContainsUser(userId.value, requestedList);
		EndAction(true);
	}
}

}
