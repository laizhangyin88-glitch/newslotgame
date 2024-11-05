using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/Friend")]
    public class GetFriendInviteLinkVersion : ActionTask
	{
		public BBParameter<int> friendCollectCount;
		public BBParameter<string> saveAsString;
		public BBParameter<bool> saveAs;

		protected override void OnExecute()
		{
			if (friendCollectCount.value > 0)
            {
				saveAs.value = false;
            }
			else
            {
				saveAs.value = BlackboardQueryUtils.GetFriendInviteLinkVersion();
				saveAsString.value = "";
			}

			EndAction(true);
		}
	}
}