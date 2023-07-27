using System.Collections.Generic;
using BagelCode.ClientModels;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using SlotMaker;
using UnityEngine;

namespace BagelCode.Scratcher.Tasks.Actions
{
    [Category("★ BagelCode/Meta Games/Collecting Game")]
    public class SetPopupScratcherInformation : ActionTask<ContextElement>
    {
        public BBParameter<GameObject> scratcher;
        public BBParameter<Blackboard> scratcherRewardResult;
        
        public BBParameter<Blackboard> playedScratcherRewardResult;
        public BBParameter<int> scratcherId;
        public BBParameter<bool> isAuto;
        public BBParameter<bool> isReward;
        public BBParameter<int> remainingCount;
        public BBParameter<List<string>> scratcherNameList;
        public BBParameter<List<long>> prizeList;
        public BBParameter<List<int>> winTypeList;
        
        protected override string info
        {
            get { return "Set Popup Scratcher Information"; }
        }

        protected override void OnExecute()
        {
            Blackboard scratcherBB = scratcher.value.GetComponent<Blackboard>();
            
            if (scratcherRewardResult != null && scratcherRewardResult.value != null)
            {
                var bb = BlackboardUtils.GetOrCreateBlackboard(scratcherBB, "_scratcherRewardResult");
                BlackboardUtils.CopyBlackboard(scratcherRewardResult.value, bb);
            }

            if (scratcherNameList.value == null)
                scratcherNameList.value = new List<string>();
            
            if (prizeList.value == null)
                prizeList.value = new List<long>();

            if (winTypeList.value == null)
                winTypeList.value = new List<int>();

            if (playedScratcherRewardResult != null && playedScratcherRewardResult.value != null)
            {
                ScratcherName scratcherNameType = playedScratcherRewardResult.value.GetValue<ScratcherName>("scratcherName");
                string scratcherName = BlackboardQueryUtils.GetScratcherName(scratcherNameType);

                scratcherNameList.value.Add(scratcherName);
                prizeList.value.Add(playedScratcherRewardResult.value.GetValue<long>("credit"));
                winTypeList.value.Add((int)playedScratcherRewardResult.value.GetValue<ScratcherBigWinType>("bigWinType"));

                if (!isReward.value)
                    remainingCount.value--;
            }
            
            scratcherBB.AddVariable("_isAuto", isAuto.value);
            scratcherBB.AddVariable("_isReward", isReward.value);
            scratcherBB.AddVariable("_remainingCount", remainingCount.value);
            scratcherBB.AddVariable("_scratcherId", scratcherId.value);
            scratcherBB.AddVariable("_scratcherNameList", scratcherNameList.value);
            scratcherBB.AddVariable("_prizeList", prizeList.value);
            scratcherBB.AddVariable("_winTypeList", winTypeList.value);
            
            EndAction(true);
        }
    }
}
