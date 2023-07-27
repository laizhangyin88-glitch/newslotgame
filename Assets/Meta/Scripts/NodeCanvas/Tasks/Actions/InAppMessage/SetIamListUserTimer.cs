using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Notice")]
public class SetIamListUserTimer : ActionTask<Blackboard> 
{
	public BBParameter<string> iamList;

	protected override void OnExecute()
	{
		var iamInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, iamList.value);

		if (iamInfoList != null && iamInfoList.value != null)
		{
			int count = iamInfoList.value.Count;

			for (int i = 0; i < count; ++i)
			{
				BlackboardQueryUtils.SetIAMUserTimer(iamInfoList.value[i]);
			}
		}

		EndAction();
	}
}

}
