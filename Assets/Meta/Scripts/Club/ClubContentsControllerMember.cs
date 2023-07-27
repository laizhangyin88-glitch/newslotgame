using BagelCode.ClientModels;
using NodeCanvas.Framework;
using SlotMaker;
using System.Collections.Generic;

namespace BagelCode
{
    public partial class ClubContentsController
    {
        public void OnTabMember()
        {
            // Send BI
            var authority = bb.GetVariable<ClubAuthority>("_authority")?.value ?? ClubAuthority.UNKNOWN;
            var customData = new Dictionary<string, object>();
            customData["user_authority"] = (long)authority;
            customData["is_leader_push_enabled"] = leaderPushInfoBB.GetVariable<bool>("available")?.value ?? false;
            Analytics.CustomEvent("client_click_club_members", customData);
        }
    }
}
