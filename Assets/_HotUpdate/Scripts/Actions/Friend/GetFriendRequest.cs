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
    public class GetFriendRequest : ActionTask
    {
        public BBParameter<int> saveAs;

        protected override void OnExecute()
        {
            var friendList = BlackboardQueryUtils.GetFriendList();
            int requestFriendCount = 0;

            if (friendList != null)
            {
                for (int i = 0; i < friendList.Count; i++)
                {
                    var friendInfo = friendList[i];
                    bool accepted = friendInfo.GetValue<bool>("accepted");

                    if (!accepted) requestFriendCount++;
                }
            }

            saveAs.value = requestFriendCount;
            EndAction();
        }
    }
}
