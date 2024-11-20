using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace SlotMaker.Tasks.Condition
{

[Category("★ SlotMaker/Blackboard")]
public class CheckHasMysteryReward : ConditionTask<Blackboard> 
{
    public BBParameter<string> valueA;

    protected override string info
    {
        get { return string.Format("Check Mystery Reward Result {0}", valueA); }
    }

    protected override bool OnCheck() 
    {
        var rewardList = BlackboardUtils.FindVariable<List<Blackboard>>(agent, valueA.value);

        if(rewardList != null)
        {
            for(int i=0; i<rewardList.value.Count; ++i)
            {
                var isMystery = BlackboardUtils.FindVariable<bool>(rewardList.value[i], "isFromRandom");

                if(isMystery != null && isMystery.value == true)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

}