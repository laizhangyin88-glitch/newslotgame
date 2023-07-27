using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;

using static BagelCode.VipLounge.VipLounge.Utils;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_vip_lounge_enter : ActionTask<Blackboard>
    {
        public BBParameter<string> contextID;
        public BBParameter<string> enterType;

        protected override void OnExecute()
        {
            int badgeCount = BadgeCount;
            long badgeExpireTs = BenefitEndTimestamp;
            long loungeJackpotActiveTimestamp = LoungeJackpotActiveTimestamp;
            bool isBenefitActive = badgeCount > 0;
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["vip_badge"] = badgeCount;
            customData["vip_lounge_point"] = LoungePoint;
            customData["extra_vip_lounge_point"] = ExcessLoungePoint;
            
            if (isBenefitActive)
            {
                customData["vip_badge_expire_ts"] = badgeCount > 1 ? badgeExpireTs - LoungeOpenTimeMillisec : badgeExpireTs;
                customData["vip_lounge_expire_ts"] = badgeExpireTs;
            }
            else
            {
                customData["vip_badge_expire_ts"] = null;
                customData["vip_lounge_expire_ts"] = null;
            }

            customData["is_benefit_active"] = isBenefitActive;
            customData["lounge_jackpot_status"] = GetLoungeJackpotStatus(loungeJackpotActiveTimestamp);
            if (loungeJackpotActiveTimestamp > 0L)
                customData["lounge_jackpot_ready_ts"] = loungeJackpotActiveTimestamp;
            else
                customData["lounge_jackpot_ready_ts"] = null;
            //customData["enter_type"] = enterType.value;
            customData["context_id"] = contextID.value;

            Analytics.CustomEvent("client_vip_lounge_enter", customData);

            EndAction();
        }

        private string GetLoungeJackpotStatus(long loungeJackpotActiveTimestamp)
        {
            if (loungeJackpotActiveTimestamp > 0L)
            {
                if (loungeJackpotActiveTimestamp > TimeUtils.GetTimeStamp())
                    return "ACTIVE";
                else
                    return "READY";
            }
            else
                return "INACTIVE";
        }
    }
}
