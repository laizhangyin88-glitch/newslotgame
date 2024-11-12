using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;
using BagelCode.ClientModels;
using ParadoxNotion;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{
    [Category("★ BagelCode/Reward")]
    public class OpenRewardResultPopupAsync : ActionTask<Blackboard>
    {
        [BlackboardOnly]
        public BBParameter<RewardType> rewardType;

        protected override string info
        {
            get { return "Open Reward Result Popup Async"; }
        }

        protected override void OnExecute()
        {
            StartCoroutine(RewardResultProcessCoroutine(rewardType.value));
        }

        private IEnumerator RewardResultProcessCoroutine(RewardType rewardType)
        {
            yield return StartCoroutine(MetaCommonRewardUtils.RewardResultProcessCoroutine(agent.gameObject, rewardType, true));
            EndAction();
        }
    }
}
