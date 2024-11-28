using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using BagelCode.ClientModels;
using SlotMaker;

namespace BagelCode.Tasks.Actions
{

[Category("★ BagelCode/Blackboard/PendingAction")]
public class RemovePendingAction : ActionTask<Blackboard>  
{
    public BBParameter<int> removePendingActionIndex;

    protected override string info
    {
        get { return string.Format("Remove PendingAction({0})", removePendingActionIndex); }
    }

    protected override void OnExecute()
    {
        List<Blackboard> bbList = BlackboardUtils.GetOrCreateBlackboardList(MainBlackboard.Get(), "pendingAction");

        if (bbList != null)
        {
            if(removePendingActionIndex.value >= 0 && removePendingActionIndex.value < bbList.Count)
            {
                BlackboardUtils.RemoveAtBlackboardList(MainBlackboard.Get(), "pendingAction", removePendingActionIndex.value);
            }
        }

        EndAction();
    }
}

}
