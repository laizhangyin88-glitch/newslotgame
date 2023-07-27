using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode.Tasks.Actions 
{

    [Category("★ BagelCode/PassiveEvents")]
    public class GetScratcherPassiveEvent : ActionTask<Blackboard> 
    {
        public BBParameter<int> id;
        public BBParameter<long> endTimestamp;
        public BBParameter<long> multiplierNumerator;
        public BBParameter<long> multiplierPercent;

        protected override string info
        {
            get{ return string.Format("Scratcher Passive Event"); }
        }

        protected override void OnExecute () 
        {
            EventInfo scratcherEventInfo = PassiveEventManager.Instance.GetActiveEventInfo(EventInfoType.COLLECTING_GAME_CHEST_DROP_RATE_MULTIPLY);

            if(scratcherEventInfo != null)
            {
                id.value = scratcherEventInfo.id;
                endTimestamp.value = scratcherEventInfo.endTimestamp;
                multiplierNumerator.value = PassiveEventManager.Instance.GetEventInfoMultiplierNumerator(scratcherEventInfo);
                multiplierPercent.value = multiplierNumerator.value - NumberUtils.GetGlobalDenominator();
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "collectingGameChestDropRateMultiplyEventId", scratcherEventInfo.id);
            }
            else
            {
                id.value = 0;
                endTimestamp.value = 0;
                multiplierNumerator.value = NumberUtils.GetGlobalDenominator();
                multiplierPercent.value = 0;
                BlackboardUtils.SetOrCreateValue<int>(MainBlackboard.Get(), "collectingGameChestDropRateMultiplyEventId", 0);
            }

            EndAction(true);
        }
    }
}
