using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

using static BagelCode.VipLounge.VipLounge.Utils;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_vip_lounge_information : ActionTask<Blackboard>
    {
        public BBParameter<string> contextID;
        public BBParameter<int> index;

        protected override void OnExecute()
        {
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["vip_lounge_point"] = GetCurrentLoungePoint();
            customData["vip_badge"] = BadgeCount;
            customData["benefit_end_timestamp"] = BenefitEndTimestamp;
            customData["page"] = index.value;
            customData["context_id"] = contextID.value;

            Analytics.CustomEvent("client_vip_lounge_information", customData);

            EndAction();
        }
    }
}
