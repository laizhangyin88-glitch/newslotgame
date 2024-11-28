using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Friend")]
public class GetFriendCoinRequest : ActionTask
{
	public BBParameter<int> saveAs;

	protected override void OnExecute()
	{		
		saveAs.value = BlackboardQueryUtils.GetFriendCoinRequest();
		EndAction();
	}

}

}

