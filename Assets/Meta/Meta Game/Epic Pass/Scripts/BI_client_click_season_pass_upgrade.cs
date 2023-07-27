using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions.BI
{
    [Category("★ BagelCode/BI")]
    public class BI_client_click_season_pass_upgrade : ActionTask
    {
        public BBParameter<string> contextID;

        protected override void OnExecute()
        {
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo(EventInfoType.SEASON_PASS);
            if(eventInfo != null &&  eventInfo.constraints != null)
            {
                EventDataSeasonPass epicPassEventInfo = eventInfo.constraints as EventDataSeasonPass;
                if(epicPassEventInfo != null)
                {
                    Dictionary<string, object> customData = new Dictionary<string, object>();

                    customData["passive_event_id"] = (long)eventInfo.id;
                    customData["season_pass_setting_id"] = (long)epicPassEventInfo.seasonPassSettingId;
                    customData["season_pass_setting_name"] = epicPassEventInfo.seasonPassSettingName;
                    customData["context_id"] = contextID.value;
                    customData["pass_level"] = EpicPassUtils.Level;
                    customData["required_pass_point"] = EpicPassUtils.RequiredPoint;
                    customData["own_pass_point"] = EpicPassUtils.Point;

                    Analytics.CustomEvent("client_click_season_pass_upgrade", customData);
                }
            }

            EndAction();
        }
    }
}
