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
public class SetNoticeListUserTimer : ActionTask<Blackboard> 
{
	public BBParameter<string> noticeList;

	protected override void OnExecute()
	{
		var noticeInfoList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, noticeList.value);

		if (noticeInfoList != null && noticeInfoList.value != null)
		{
			int count = noticeInfoList.value.Count;

			for (int i = 0; i < count; ++i)
			{
				NoticeTypes type = noticeInfoList.value[i].GetValue<NoticeTypes>("type");

				if (type == NoticeTypes.POPUP || type == NoticeTypes.ACTION_POPUP || type == NoticeTypes.TEXT_POPUP)
				{
					BlackboardQueryUtils.SetNoticeUserTimer(noticeInfoList.value[i]);
				}
			}
		}

		EndAction();
	}
}

}
