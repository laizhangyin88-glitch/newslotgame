using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

namespace BagelCode.Task.Actions
{

    [Category("★ BagelCode/Friend")]
    public class SetFriendInviteLinkVersion : ActionTask
    {
        protected override void OnExecute()
        {
            BlackboardQueryUtils.SetFriendInviteLinkVersion();
            EndAction(true);
        }
    }
}