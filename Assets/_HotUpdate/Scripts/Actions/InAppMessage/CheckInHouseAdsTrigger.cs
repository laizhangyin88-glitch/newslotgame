using UnityEngine;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{
    [Category("★ BagelCode/IAM")]
    public class CheckInhouseAdsTrigger : ActionTask<Blackboard>
    {
        protected override string info
        {
            get
            {
                return string.Format("Check Inhouse Ads IAM");
            }
        }

        protected override void OnExecute()
        {
            var triggerType = agent.GetVariable<InAppMessageTriggerType>("triggerType");
            if (triggerType != null)
            {
                switch (triggerType.value)
                {
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_COLLECTING_GAME:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_GEM_JACKPOT:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_SEASON_PASS:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_BOSS_RAIDERS:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_CLUB_ARENA:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_TIME_COLLECT:
                    case InAppMessageTriggerType.INHOUSE_ADS_FOR_SHOP:
                    case InAppMessageTriggerType.INHOUSE_ADS_WITH_PUSH_OFF:
                    {
                        var closeInhouseIamInfo = IAMRouter.Instance.GetValidTriggerIAMInfo(InAppMessageTriggerType.CLOSE_INHOUSE_ADS);
                        if(closeInhouseIamInfo != null)
                            BlackboardUtils.SetOrCreateValue<int>(agent, "closeTriggerIAMId", closeInhouseIamInfo.GetValue<int>("id"));
                    }
                    break;
                }
            }
            
            EndAction();
        }
    }

}
