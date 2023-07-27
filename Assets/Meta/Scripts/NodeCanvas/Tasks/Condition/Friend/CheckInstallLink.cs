using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using BagelCode.ClientModels;

namespace SlotMaker.Tasks.Condition
{
    [Category("★ BagelCode/Friend")]
    public class CheckInstallLink : ConditionTask<Blackboard>
    {
        protected override bool OnCheck()
        {
            GameObject deepLinkGO = GameObject.Find("DeepLink");
            if (deepLinkGO == null)
                return false;

            Blackboard deepLinkBB = deepLinkGO.GetComponent<Blackboard>();
            if (deepLinkBB == null)
                return false;

            var actionBB = BlackboardUtils.FindVariable<Blackboard>(deepLinkBB, "_action");
            if (actionBB == null)
                return false;

            ActionType type = BlackboardUtils.FindValue<ActionType>(actionBB.value, "type");
            return type == ActionType.SNS_INVITE || type == ActionType.FB_MESSAGE_INVITE || type == ActionType.SNS_INVITE_WITH_TIER;
        }
    }
}