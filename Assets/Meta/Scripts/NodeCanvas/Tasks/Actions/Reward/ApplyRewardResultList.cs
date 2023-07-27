using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using SlotMaker;

namespace BagelCode.Tasks.Actions.BlackboardQuery
{

[Category("★ BagelCode/Reward")]
public class ApplyRewardResultList : ActionTask<Blackboard>
{
    public BBParameter<string> rewardResultListValue;

    protected override string info
    {
        get { return "Apply Reward Result Info List"; }
    }

    protected override void OnExecute()
    {
        var rewardInfoListBB = BlackboardUtils.FindVariable<List<Blackboard>>(agent, rewardResultListValue.value);

        if(rewardInfoListBB != null)
        {
            for(int i=0; i<rewardInfoListBB.value.Count; ++i)
            {
                BlackboardQueryUtils.ApplyRewardResult(rewardInfoListBB.value[i]);
            }
        }
#if DEV || UNITY_EDITOR
        else
        {
            Debug.LogError(rewardResultListValue + " is Null");
        }
#endif

        EndAction();
    }
}

}
