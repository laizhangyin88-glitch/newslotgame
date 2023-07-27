using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace SlotMaker.Tasks.Condition
{
    [Category("★ BagelCode/Friend")]
    public class CheckFriendInviteLinkUser : ConditionTask<Blackboard>
    {
        protected override bool OnCheck()
        {
            var level = BlackboardUtils.FindVariable<int>(null, "/me/level");
            var requiredExp = BlackboardUtils.FindVariable<long>(null, "/me/requiredExp");
            var requiredExpMax = BlackboardUtils.FindVariable<long>(null, "/me/requiredExpMax");

            var ssoAccountInfo = BlackboardUtils.FindVariable<Blackboard>(MainBlackboard.Get(), "ssoAccountInfo");

            return level.value == 1 && requiredExp.value == requiredExpMax.value && ssoAccountInfo == null;
        }
    }
}