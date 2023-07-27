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
    public class BI_client_click_season_pass_free_point : ActionTask
    {
        public BBParameter<string> actionType;

        protected override string info
        {
            get { return string.Format("BI Free Point ({0})", actionType); }
        }

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
                    customData["type"] = actionType.value;

                    Analytics.CustomEvent("client_click_season_pass_free_point", customData);
                }
            }

            EndAction();
        }
    }
}
