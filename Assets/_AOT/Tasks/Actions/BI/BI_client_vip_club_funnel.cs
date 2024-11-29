using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Tasks.Actions.BI
{

    [Category("★ BagelCode/BI")]
    public class BI_client_vip_club_funnel : ActionTask<Blackboard>
    {
        public BBParameter<BIVIPClubFunnelStep> step;
        public BBParameter<bool> isAutoFill;

        protected override string info
        {
            get
            {
                return string.Format("client_vip_club_funnel({0})", step);
            }
        }

        // todo meta defines로 이사 (참조하는 FSM 다 고쳐야됨)
        public enum BIVIPClubFunnelType
        {
            SETTING = 0,
            FIRST_LOGIN_OPTION = 1,
            VIP_BONUS = 2,
            FB_CONNECT_NUDGE = 3,
            FB_CONNECT_ACTION = 4,
            FRIEND_SUGGESTION = 5,
            PROFILE_VIP_CLUB = 6,
            PROFILE_JOIN_VIP = 7,
            PROFILE_FB_CONNECT = 8,
            OPEN_POPUP = 9,
            EMAIL_CONNECT_NUDGE = 10,
            CHALLENGE_JOIN_VIP = 11,
            FRIENDS_INVITE = 12,
            JOIN_VIP_VIA_ACTION_POPUP = 13,
        }

        public enum BIVIPClubFunnelStep
        {
            JOIN_VIP_CLUB_POPUP,
            FB_CONNECT_POPUP,
            EMAIL_CONNECT_POPUP,
            FB_CONNECT_DONE,
            EMAIL_CONNECT_VERIFICATION,
            EMAIL_CONNECT_DONE,
            APPLE_CONNECT_POPUP,
            APPLE_CONNECT_DONE,
        }

        protected override void OnExecute()
        {
            int type = PlayerPrefs.GetInt("VIP_CLUB_FUNNEL_TYPE", -1);
            string contextId = PlayerPrefs.GetString("VIP_CLUB_FUNNEL_CONTEXT_ID", "");
            
            Dictionary<string, object> customData = new Dictionary<string, object>();

            customData["type"] = type != -1 ? ((BIVIPClubFunnelType) type).ToString().ToLower() : "";
            customData["step"] = step.value.ToString().ToLower();
            customData["context_id"] = contextId;

            if(step.value == BIVIPClubFunnelStep.EMAIL_CONNECT_DONE)
                customData["is_auto_fill"] = isAutoFill.value;
            else
                customData["is_auto_fill"] = null;
    
            Analytics.CustomEvent("client_vip_club_funnel", customData);
            EndAction();
        }
    }

}
