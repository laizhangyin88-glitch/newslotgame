using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Notice")]
public class GetNoticeEndTimestamp : ActionTask<Blackboard> 
{
	public BBParameter<string> noticeInfo;
	[BlackboardOnly]
	public BBParameter<long> endTimestamp;

	protected override void OnExecute()
	{
		var info = BlackboardUtils.FindVariable<Blackboard>(agent, noticeInfo.value);
		if (info != null && info.value != null)
		{
			endTimestamp.value = BlackboardQueryUtils.GetNoticeEndTimestamp(info.value);
		}

		EndAction();
	}

}

}
