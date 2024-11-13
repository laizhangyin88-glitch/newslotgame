using System;
using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/Club")]
    public class ClubFeedClubArenaHelpEnterMetaGame : ActionTask<Blackboard>
    {
        public BBParameter<string> clubFeedInfoValue;
        public BBParameter<string> bundleName;
        public BBParameter<bool> isSuccess;

        protected override void OnExecute()
        {
            var clubFeedInfo = BlackboardUtils.FindVariable<Blackboard>(agent, clubFeedInfoValue.value);
            EventInfo eventInfo = BlackboardQueryUtils.GetMetaGameEventInfo();

            isSuccess.value = false;

            if (clubFeedInfo != null && eventInfo != null)
            {
//                // Asset bundle Check
//#if USE_ASSETBUNDLE
//                var bundle = AssetBundleManager.GetLoadedAssetBundle(bundleName.value);
//                if (bundle == null)
//                {
//                    EndAction();
//                    return;
//                }
//#endif
                // opponentUserId Check
                if (eventInfo.id == clubFeedInfo.value.GetValue<int>("eventId"))
                {
                    ClubArenaUtils.TargetUserId = clubFeedInfo.value.GetValue<string>("opponentUserId");
                    isSuccess.value = true;
                }
            }

            EndAction();
        }
    }
}