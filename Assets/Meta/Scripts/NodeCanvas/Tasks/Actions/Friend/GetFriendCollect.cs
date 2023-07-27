using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

[Category("★ BagelCode/Friend")]
public class GetFriendCollect : ActionTask
{
	public BBParameter<int> saveAs;

	protected override void OnExecute()
	{
		saveAs.value = BlackboardQueryUtils.GetFriendCollect();
		EndAction(true);
	}

}

}
