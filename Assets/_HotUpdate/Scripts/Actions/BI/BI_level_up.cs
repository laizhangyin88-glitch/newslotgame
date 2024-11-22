using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_level_up : ActionTask
    {
        public BBParameter<int> level;

        protected override void OnExecute()
        {
            var me = BlackboardUtils.FindVariable<Blackboard>(null, "/me").value;
            var creditBonusList = BlackboardUtils.FindVariable<List<long>>(null, "/values/level/CREDIT_BONUS").value;
            var rpBonusList = BlackboardUtils.FindVariable<List<long>>(null, "/values/level/RP_BONUS").value;

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["earn_coin"] = creditBonusList[level.value - 1];
            customData["rp"] = rpBonusList[level.value - 1];
            customData["target_level"] = level.value;
            customData["earn_gem"] = LevelUtils.GetLevelGemBonus(level.value - 1);
            EventInfo expBoostEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.EXP_MULTIPLY);
            EventInfo expBoostExtendableEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.EXP_MULTIPLY_EXTENDABLE);
            customData["is_exp_boost"] = expBoostEventInfo != null || expBoostExtendableEventInfo != null;
            customData["is_level_up_faster"] = !VipLounge.VipLounge.Utils.IsEnded;
            string contextId = BiEventUtils.GenerateContextID();
            customData["context_id"] = contextId;

            Analytics.CustomEvent("client_level_up", customData);

            string levelUpEventID = string.Format("level_up:{0}", BiEventUtils.AddZeroPadding(2, level.value));
            AdjustManager.Instance.SendEvent(levelUpEventID);

            FIREventManager.Instance.SendEvent(levelUpEventID);

            EndAction();
        }
    }
}
