using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_boss_raiders_enter : ActionTask<Blackboard>
    {
        public BBParameter<string> contextID;
        public BBParameter<string> type;
        public BBParameter<long> energy;

        protected override void OnExecute()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(true);
            int themeId = ((EventDataBossRaiders)eventInfo.constraints)?.themeId ?? 0;

            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["context_id"] = contextID.value;
            customData["type"] = type.value;
            customData["energy"] = energy.value;
            customData["theme_id"] = themeId;
            customData["boss_raiders_type"] = "default";

            Analytics.CustomEvent("client_boss_raiders_enter", customData);

            EndAction();
        }
    }
}
