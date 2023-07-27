using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{
    [Category("★ BagelCode/Reward")]
    public class ApplyRewardResult : ActionTask<Blackboard>
    {
        public BBParameter<string> rewardResult;

        protected override string info
        {
            get { return "Apply Reward Result Info"; }
        }

        protected override void OnExecute()
        {
            var rewardInfoBB = BlackboardUtils.FindVariable<Blackboard>(agent, rewardResult.value);

            if (rewardInfoBB != null)
            {
                BlackboardQueryUtils.ApplyRewardResult(rewardInfoBB.value, false);
            }
#if DEV || UNITY_EDITOR
            else
            {
                Debug.LogError(rewardResult + " is Null");
            }
#endif

            EndAction();
        }
    }
}
